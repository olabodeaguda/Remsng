using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class SyncManagers : ISyncManagers
    {
        private static HttpClient client = new HttpClient();
        private SyncRepository syncDao;
        private IConfiguration configuration;
        private ILogger logger;
        public SyncManagers(DbContext db, ILoggerFactory loggerFactory, IConfigurationRoot _configuration)
        {
            logger = loggerFactory.CreateLogger("Sync Service");
            syncDao = new SyncRepository(db);

            configuration = _configuration;
        }

        public async Task SyncUp()
        {
            try
            {
                IConfigurationSection config = configuration.GetSection("domainSection");
                var d = config["domainId"];
                if (d != null)
                {
                    if (Guid.TryParse(d.ToString(), out Guid domainId))
                    {
                        List<SyncDataModel> data = await syncDao.Get();
                        if (data.Count > 0)
                        {
                            data = data.Select(x => { x.domainId = domainId; return x; }).ToList();
                            string list = JsonConvert.SerializeObject(data.ToArray());
                            List<SyncDataModel> isSyncResult = await SyncTORemote(list);
                            if (isSyncResult.Count > 0)
                            {
                                bool result = await syncDao.UpdateSyncStatus(isSyncResult.Select(x => x.Id).ToArray());
                                if (result)
                                {
                                    logger.LogInformation("ids has been successfully sync to remote", isSyncResult.Select(x => x.Id).ToArray());
                                }
                            }
                        }
                        else
                        {
                            logger.LogInformation("no record to sync");
                        }
                    }
                    else
                    {
                        logger.LogError("Can not retrieve domain credentials");
                    }
                }
                else
                {
                    logger.LogError("Can not retrieve domain credentials");
                }
            }
            catch (Exception x)
            {
                logger.LogError("Sync Final Exception", x);
            }
        }

        public async Task SyncDown()
        {
            List<SyncDataModel> data = await syncDao.GetApprovalUpdate();
            if (data.Count > 0)
            {
                List<SyncDataModel> lst = await SyncFromRemote(JsonConvert.SerializeObject(data.Select(x => x.Id).ToArray()));
                List<SyncDataModel> approveBills = lst.Where(x => x.paymentStatus != "COMPLETED").ToList();
                if (approveBills.Count > 0)
                {
                    bool result = await syncDao.UpdatePaymentStatus(approveBills);
                    if (result)
                    {
                        logger.LogInformation($"{JsonConvert.SerializeObject(approveBills)} has been approved");
                    }
                }
            }
        }

        private async Task<List<SyncDataModel>> SyncTORemote(string data)
        {
            IConfigurationSection config = configuration.GetSection("domainSection");
            var d = config["SyncUrl"];

            if (d != null)
            {
                string url = $"{d}/api/v1/sync/push";

                HttpResponseMessage httpResponse = await client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                string responseString = await httpResponse.Content.ReadAsStringAsync();
                Response response = JsonConvert.DeserializeObject<Response>(responseString);

                if (httpResponse.StatusCode == HttpStatusCode.Created)
                {

                    if (response.code == MsgCode_Enum.SUCCESS)
                    {
                        logger.LogInformation("Sync successfully", data);
                        List<SyncDataModel> lst = JsonConvert.DeserializeObject<List<SyncDataModel>>(response.data.ToString());
                        return lst;
                    }
                    else
                    {
                        logger.LogError(response.description, data);
                        throw new Exception(response.description);
                    }
                }
                else
                {
                    logger.LogError($"Sync to remote server Fails {response.description}", data);
                    throw new Exception($"Sync to remote server Fails {response.description}");
                }
            }
            else
            {
                logger.LogError($"App could not retrieve sync credentials", data);
                throw new Exception($"App could not retrieve sync credentials");
            }
        }

        private async Task<List<SyncDataModel>> SyncFromRemote(string data)
        {
            IConfigurationSection config = configuration.GetSection("domainSection");
            var d = config["SyncUrl"];

            if (d != null)
            {
                string url = $"{d}/api/v1/sync/update";

                HttpResponseMessage httpResponse = await client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                string responseString = await httpResponse.Content.ReadAsStringAsync();
                Response response = JsonConvert.DeserializeObject<Response>(responseString);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {

                    if (response.code == MsgCode_Enum.SUCCESS)
                    {
                        logger.LogInformation("Sync successfully", data);
                        List<SyncDataModel> lst = JsonConvert.DeserializeObject<List<SyncDataModel>>(response.data.ToString());
                        return lst;
                    }
                    else
                    {
                        logger.LogError(response.description, data);
                        throw new Exception(response.description);
                    }
                }
                else
                {
                    logger.LogError($"Sync to remote server Fails {response.description}", data);
                    throw new Exception($"Sync to remote server Fails {response.description}");
                }
            }
            else
            {
                logger.LogError($"App could not retrieve sync credentials", data);
                throw new Exception($"App could not retrieve sync credentials");
            }
        }

    }
}


using Newtonsoft.Json;
using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using RemsNG.Utilities;
using Microsoft.Extensions.Logging;

namespace RemsNG.Services
{
    public class RunDemandNoticeService : IRunDemandNoticeService
    {
        private readonly TaxpayerDao taxpayerDao;
        private readonly DemandNoticeDao demandNoticeDao;
        private readonly DemandNoticeTaxpayersDao demandNoticeTaxpayersDao;
        private readonly ErrorDao errorDao;
        private readonly ILogger logger;
        private readonly DemandNoticeItemDao demandNoticeItemDao;
        public RunDemandNoticeService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("Demand Notice Jobs");
            taxpayerDao = new TaxpayerDao(_db);
            demandNoticeDao = new DemandNoticeDao(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersDao(_db);
            errorDao = new ErrorDao(_db, loggerFactory);
            demandNoticeItemDao = new DemandNoticeItemDao(_db);
        }

        public async Task RegisterTaxpayer()
        {
            DemandNotice demandNotice = await demandNoticeDao.DequeueDemandNotice();

            try
            {
                if (demandNotice != null)
                {
                    String query = EncryptDecryptUtils.FromHexString(demandNotice.query);
                    DemandNoticeRequest demandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequest>(query);
                    List<Taxpayer> taxpayers = await taxpayerDao.Get(demandNoticeRequest);
                    // get all demandnotice by taxpayer and year in domainnotce taxpayer table
                    if (taxpayers.Count > 0)
                    {
                        List<DemandNoticeTaxpayersDetail> dt =
                            await demandNoticeTaxpayersDao.getTaxpayerId(taxpayers.Select(x => string.Format("'{0}'", x.id)).ToArray(), demandNotice.billingYear);

                        //make sure demand notice have not been raised for the taxpayers by year and taxpayersId

                        foreach (var tm in taxpayers)
                        {
                            var itExist = dt.FirstOrDefault(x => x.taxpayerId == tm.id);
                            if (itExist != null)
                            {
                                Error error = new Error()
                                {
                                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                    errorvalue = $"Demand notice have already been raised for {tm.surname} {tm.firstname} {tm.lastname} " +
                                      $"for billing year {demandNotice.billingYear}",
                                    ownerId = demandNotice.id
                                };
                                bool result = await errorDao.Add(error);
                                continue;
                            }
                            else
                            {
                                //add taxpayers
                                DemandNoticeTaxpayersDetail dntd = new DemandNoticeTaxpayersDetail();
                                dntd.billingYr = demandNotice.billingYear;
                                dntd.dnId = demandNotice.id;
                                dntd.createdBy = "Background";
                                dntd.taxpayerId = tm.id;

                                Response response = await demandNoticeTaxpayersDao.Add(dntd);

                                if (response.code == MsgCode_Enum.SUCCESS)
                                {
                                    //run arrears 
                                    //run penalties
                                    await RunDemandNoticeItem(dntd); //run items 
                                }
                                else
                                {
                                    // log error
                                    Error error = new Error()
                                    {
                                        errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                        errorvalue = response.description,
                                        ownerId = dntd.dnId
                                    };
                                    bool result = await errorDao.Add(error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception x)
            {
                logger.LogError(x.Message);
            }
        }

        private async Task RunArrears(DemandNoticeTaxpayersDetail dntd)
        {
            //get previous year demand notice
            // check for unpaid outstanding payment

        }

        private async Task RunDemandNoticeItem(DemandNoticeTaxpayersDetail dntd)
        {
            Response response = await demandNoticeItemDao.Add(dntd);
            if (response.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(response.description, dntd);
            }
        }
    }
}

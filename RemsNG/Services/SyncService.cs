using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemsNG.Dao;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class SyncService : ISyncService
    {
        private SyncDao syncDao;
        private IConfiguration configuration;
        public SyncService(RemsDbContext db, ILoggerFactory loggerFactory, IConfigurationRoot _configuration)
        {
            syncDao = new SyncDao(db, loggerFactory);
            configuration = _configuration;
        }

        public async Task Sync()
        {
            IConfigurationSection config = configuration.GetSection("domainSection");
            var d = config.GetValue(typeof(Guid), "domainId");
            if (d != null)
            {
                Guid domainId;
                if (Guid.TryParse(d.ToString(), out domainId))
                {
                    var data = await syncDao.Get();
                    if (data.Count > 0)
                    {
                        data = data.Select(x => { x.domainId = domainId; return x; }).ToList();
                        string list = JsonConvert.SerializeObject(data.ToArray());

                    }
                }
            }
        }
    }
}

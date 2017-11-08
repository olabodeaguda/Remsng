using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class RunDemandNoticeService : IRunDemandNoticeService
    {
        private TaxpayerDao taxpayerDao;
        private DemandNoticeDao demandNoticeDao;
        public RunDemandNoticeService(RemsDbContext _db)
        {
            taxpayerDao = new TaxpayerDao(_db);
            demandNoticeDao = new DemandNoticeDao(_db);
        }

        public async Task RegisterTaxpayer(DemandNoticeRequest demandNoticeRequest)
        {
            DemandNotice demandNotice = await demandNoticeDao.DequeueDemandNotice();

            if (demandNotice != null)
            {
                List<Taxpayer> taxpayers = await taxpayerDao.Get(demandNoticeRequest);

                if (taxpayers.Count > 0)
                {
                    //make sure demand notice have not been raised for the taxpayers by year and taxpayersId

                }

            }
        }




    }
}

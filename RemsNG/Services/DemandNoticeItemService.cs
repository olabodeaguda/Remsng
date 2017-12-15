using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Models;

namespace RemsNG.Services
{
    public class DemandNoticeItemService : IDemandNoticeItemService
    {
        DemandNoticeItemDao dnDao;
        public DemandNoticeItemService(RemsDbContext _db)
        {
            dnDao = new DemandNoticeItemDao(_db);
        }

        public async Task<List<DemandNoticeItem>> ByBillingNumber(string billingno)
        {
            return await dnDao.ByBillingNumber(billingno);
        }
    }
}

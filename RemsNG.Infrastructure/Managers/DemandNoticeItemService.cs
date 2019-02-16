using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemandNoticeItemManagers : IDemandNoticeItemManagers
    {
        private readonly DemandNoticeItemRepository dnDao;
        public DemandNoticeItemManagers(DbContext _db)
        {
            dnDao = new DemandNoticeItemRepository(_db);
        }

        public async Task<List<DemandNoticeItemModel>> ByBillingNumber(string billingno)
        {
            return await dnDao.ByBillingNumber(billingno);
        }
    }
}

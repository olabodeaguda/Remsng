using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticeDownloadHistoryRepository : IDemandNoticeDownloadHistoryRepository
    {
        private readonly DbContext db;
        private readonly ILogger logger;
        public DemandNoticeDownloadHistoryRepository(DbContext _db, ILoggerFactory loggerFactory)
        {
            db = _db;
            logger = loggerFactory.CreateLogger("Demand Notice download History");
        }

        public async Task Add(DemandNoticeDownloadHistoryModel dndh)
        {
            db.Set<DemandNoticeDownloadHistory>().Add(new DemandNoticeDownloadHistory()
            {
                BillingNumber = dndh.BillingNumber,
                Charges = dndh.Charges,
                CreatedBy = dndh.CreatedBy,
                DateCreated = dndh.DateCreated,
                GrandTotal = dndh.GrandTotal,
                Id = dndh.Id,
                Lastmodifiedby = dndh.Lastmodifiedby,
                LastModifiedDate = dndh.LastModifiedDate
            });
            int count = await db.SaveChangesAsync();
            if (count == 0)
            {
                logger.LogInformation("nothing was added", dndh);
            }
        }
    }
}

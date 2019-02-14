using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticeDownloadHistoryRepository : AbstractRepository
    {
        private ILogger logger;
        public DemandNoticeDownloadHistoryRepository(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("Demand Notice download History");
        }

        public async Task Add(DemandNoticeDownloadHistoryModel dndh)
        {
            db.DemandNoticeDownloadHistories.Add(new Entities.DemandNoticeDownloadHistory()
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

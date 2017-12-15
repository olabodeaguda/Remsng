using Microsoft.Extensions.Logging;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DemandNoticeDownloadHistoryDao:AbstractDao
    {
        private ILogger logger;
        public DemandNoticeDownloadHistoryDao(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("Demand Notice download History");
        }

        public async Task Add(DemandNoticeDownloadHistory dndh)
        {
            db.DemandNoticeDownloadHistories.Add(dndh);
            int count = await db.SaveChangesAsync();
            if (count == 0)
            {
                logger.LogInformation("nothing was added", dndh);
            }
        }
    }
}

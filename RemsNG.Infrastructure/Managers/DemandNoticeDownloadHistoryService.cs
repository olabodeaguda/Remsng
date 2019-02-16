using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemandNoticeDownloadHistoryManagers : IDemandNoticeDownloadHistoryManagers
    {
        DemandNoticeDownloadHistoryRepository dndh;
        public DemandNoticeDownloadHistoryManagers(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            dndh = new DemandNoticeDownloadHistoryRepository(_db, loggerFactory);
        }

        public async Task Add(DemandNoticeDownloadHistoryModel demandNoticeDownloadHistory)
        {
            await dndh.Add(demandNoticeDownloadHistory);
        }
    }
}

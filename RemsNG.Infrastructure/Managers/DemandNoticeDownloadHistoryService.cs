using Microsoft.Extensions.Logging;
using RemsNG.Dao;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class DemandNoticeDownloadHistoryService : IDemandNoticeDownloadHistory
    {
        DemandNoticeDownloadHistoryDao dndh;
        public DemandNoticeDownloadHistoryService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            dndh = new DemandNoticeDownloadHistoryDao(_db, loggerFactory);
        }

        public async Task Add(DemandNoticeDownloadHistory demandNoticeDownloadHistory)
        {
            await dndh.Add(demandNoticeDownloadHistory);
        }
    }
}

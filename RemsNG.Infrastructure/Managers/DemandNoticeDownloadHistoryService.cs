using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Dao;
using RemsNG.Data.Repository;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
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

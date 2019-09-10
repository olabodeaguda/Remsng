using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class DemandNoticeDownloadHistoryManager : IDemandNoticeDownloadHistoryManager
    {
        private readonly IDemandNoticeDownloadHistoryRepository dndh;
        public DemandNoticeDownloadHistoryManager(IDemandNoticeDownloadHistoryRepository demandNoticeDownloadHistoryRepository)
        {
            dndh = demandNoticeDownloadHistoryRepository;
        }

        public async Task Add(DemandNoticeDownloadHistoryModel demandNoticeDownloadHistory)
        {
            await dndh.Add(demandNoticeDownloadHistory);
        }
    }
}

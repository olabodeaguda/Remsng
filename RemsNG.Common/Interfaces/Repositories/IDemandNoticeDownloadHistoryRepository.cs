using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDemandNoticeDownloadHistoryRepository
    {
        Task Add(DemandNoticeDownloadHistoryModel dndh);
    }
}

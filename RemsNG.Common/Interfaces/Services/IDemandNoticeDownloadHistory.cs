using RemsNG.Common.Models;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDemandNoticeDownloadHistory
    {
        Task Add(DemandNoticeDownloadHistoryModel dndh);
    }
}

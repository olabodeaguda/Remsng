using RemsNG.Common.Models;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDemandNoticeDownloadHistoryManager
    {
        Task Add(DemandNoticeDownloadHistoryModel dndh);
    }
}

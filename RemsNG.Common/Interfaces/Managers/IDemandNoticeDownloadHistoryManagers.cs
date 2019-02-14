using RemsNG.Common.Models;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDemandNoticeDownloadHistoryManagers
    {
        Task Add(DemandNoticeDownloadHistoryModel dndh);
    }
}

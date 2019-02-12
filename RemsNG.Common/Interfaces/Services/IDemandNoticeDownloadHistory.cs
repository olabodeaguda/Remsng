using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDemandNoticeDownloadHistory
    {
        Task Add(DemandNoticeDownloadHistory dndh);
    }
}

using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface ISyncService
    {
        Task SyncUp();
        Task SyncDown();
    }
}

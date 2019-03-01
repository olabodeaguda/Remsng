using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ISyncManager
    {
        Task SyncUp();
        Task SyncDown();
    }
}

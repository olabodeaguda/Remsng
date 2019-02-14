using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ISyncManagers
    {
        Task SyncUp();
        Task SyncDown();
    }
}

using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IRunDemandNoticeManagers
    {
        Task RegisterTaxpayer();
        Task GenerateBulkDemandNotice();
    }
}

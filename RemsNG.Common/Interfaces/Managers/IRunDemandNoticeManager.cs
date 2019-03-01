using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IRunDemandNoticeManager
    {
        Task RegisterTaxpayer();
        Task GenerateBulkDemandNotice();
    }
}

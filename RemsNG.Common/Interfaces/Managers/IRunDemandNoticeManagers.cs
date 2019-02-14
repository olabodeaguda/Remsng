using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IRunDemandNoticeManagers
    {
        Task RegisterTaxpayer();
        // Task TaxpayerPenalty();
        Task GenerateBulkDemandNotice();
        Task ReconcileDemandNotice();
    }
}

using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IRunDemandNoticeService
    {
        Task RegisterTaxpayer();
        // Task TaxpayerPenalty();
        Task GenerateBulkDemandNotice();
        Task ReconcileDemandNotice();
    }
}

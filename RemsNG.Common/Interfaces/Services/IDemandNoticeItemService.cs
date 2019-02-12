using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDemandNoticeItemService
    {
        Task<List<DemandNoticeItem>> ByBillingNumber(string billingno);
    }
}

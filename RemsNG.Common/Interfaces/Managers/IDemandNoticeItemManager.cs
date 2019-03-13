using RemsNG.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDemandNoticeItemManager
    {
        Task<List<DemandNoticeItemModel>> ByBillingNumber(long billingno);
        Task<Response> AddDemandNoticeItem(DemandNoticeTaxpayersModel dntd);
        Task<Response> AddDemandNoticeItem(DemandNoticeTaxpayersModel[] Dntaxpayers);
    }
}

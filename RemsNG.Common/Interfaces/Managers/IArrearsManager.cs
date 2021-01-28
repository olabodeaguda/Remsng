using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IArrearsManager
    {
        Task<bool> RunTaxpayerArrears(Guid[] dnTaxpayerIds);
        Task<bool> RunTaxpayerArrears(Guid[] demandNoticeIds, DemandNoticeRequestModel model, int billingYr);
        Task<bool> RemoveTaxpayerArrears(Guid[] dnTaxpayerIds);
        Task<bool> AddArrears(Guid dntId, decimal amount, Guid itemId);
        Task<bool> RunTaxpayerArrears(DemandNoticeTaxpayersModel[] model, DemandNoticeRequestModel req);
    }
}

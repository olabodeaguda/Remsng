using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IItemPenaltyService
    {
        Task<Response> Add(ItemPenalty item);
        Task<Response> Update(ItemPenalty item);
        Task<Response> UpdateStatus(ItemPenalty item);
        Task<object> GetById(Guid id);
        Task<object> ListByItemId(Guid itemId);
        Task<object> ListByItemId(Guid itemId, PageModel pageModel);
        Task<Response> RunTaxpayerPenalty(Guid[] taxpayerIds);
        Task<List<DemandNoticeItemPenalty>> ActivePenalty(Guid taxpayerId);
    }
}

using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IItemPenaltyService
    {
        Task<Response> Add(ItemPenalty item);
        Task<Response> Update(ItemPenalty item);
        Task<Response> UpdateStatus(ItemPenalty item);
        Task<object> GetById(Guid id);
        Task<object> ListByItemId(Guid itemId);
        Task<object> ListByItemId(Guid itemId, PageModel pageModel);
    }
}

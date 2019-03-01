using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IItemManager
    {
        Task<Response> Add(ItemModel item);
        Task<Response> Update(ItemModel item);
        Task<Response> UpdateStatus(ItemModel item);
        Task<object> ListByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<object> ListByLcdaId(Guid lcdaId);
        Task<object> GetItemByIdAsync(Guid id);
        Task<object> GetByTaxPayersId(Guid taxpayersId);
    }
}

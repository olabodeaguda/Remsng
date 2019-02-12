using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IItemService
    {
        Task<Response> Add(Item item);
        Task<Response> Update(Item item);
        Task<Response> UpdateStatus(Item item);
        Task<object> ListByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<object> ListByLcdaId(Guid lcdaId);
        Task<object> GetItemByIdAsync(Guid id);
        Task<object> GetByTaxPayersId(Guid taxpayersId);
    }
}

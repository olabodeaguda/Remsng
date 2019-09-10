using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IItemRepository
    {
        Task<Response> Add(ItemModel item);
        Task<Response> Update(ItemModel item);
        Task<Response> UpdateStatus(ItemModel item);
        Task<object> ListByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<object> ListByLcdaId(Guid lcdaId);
        Task<object> GetItemByIdAsync(Guid id);
        Task<List<ItemModel>> GetByTaxPayersId(Guid taxpayersId);
    }
}

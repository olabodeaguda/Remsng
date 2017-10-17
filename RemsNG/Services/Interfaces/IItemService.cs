using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IItemService
    {
        Task<Response> Add(Item item);
        Task<Response> Update(Item item);
        Task<Response> UpdateStatus(Item item);
        Task<object> ListByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<object> ListByLcdaId(Guid lcdaId);
        Task<object> GetItemByIdAsync(Guid id);
    }
}

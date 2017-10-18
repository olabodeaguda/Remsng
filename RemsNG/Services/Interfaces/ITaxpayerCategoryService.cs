using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface ITaxpayerCategoryService
    {
        Task<Response> Add(TaxpayerCategory taxpayerCategory);
        Task<Response> Update(TaxpayerCategory taxpayerCategory);
        Task<Response> Delete(Guid id);
        Task<TaxpayerCategory> GetById(Guid id);
        Task<List<TaxpayerCategory>> GetListByLcdaIdAsync(Guid lcdaId);
        Task<object> GetListByLcdaIdAsync(Guid lcdaId, PageModel pageModel);
        Task<object> GetByNameAndLcdaId(Guid lcdaid, string name);
    }
}

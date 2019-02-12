using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
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
        Task<TaxpayerCategory> GetTaxpayerCategory(Guid taxpayerId);
    }
}

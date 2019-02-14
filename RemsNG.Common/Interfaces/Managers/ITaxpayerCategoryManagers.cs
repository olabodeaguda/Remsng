using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ITaxpayerCategoryManagers
    {
        Task<Response> Add(TaxpayerCategoryModel taxpayerCategory);
        Task<Response> Update(TaxpayerCategoryModel taxpayerCategory);
        Task<Response> Delete(Guid id);
        Task<TaxpayerCategoryModel> GetById(Guid id);
        Task<List<TaxpayerCategoryModel>> GetListByLcdaIdAsync(Guid lcdaId);
        Task<object> GetListByLcdaIdAsync(Guid lcdaId, PageModel pageModel);
        Task<object> GetByNameAndLcdaId(Guid lcdaid, string name);
        Task<TaxpayerCategoryModel> GetTaxpayerCategory(Guid taxpayerId);
    }
}

using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface ITaxpayerCatgoryRepository
    {
        Task<Response> Add(TaxpayerCategoryModel taxpayerCategory);
        Task<Response> Update(TaxpayerCategoryModel taxpayerCategory);
        Task<Response> Delete(Guid id);
        Task<TaxpayerCategoryModel> GetById(Guid id);
        Task<List<TaxpayerCategoryModel>> GetListByLcdaIdAsync(Guid lcdaId);
        Task<object> GetListByLcdaIdAsync(Guid lcdaId, PageModel pageModel);
        Task<object> GetByNameAndLcdaId(Guid lcdaid, string name);
        Task<TaxpayerCategoryModel> GetTaxpayerCategory(Guid taxpayerId);
        Task<string[]> GetCategory();
    }
}

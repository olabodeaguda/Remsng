using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface ICompanyItemRepository
    {
        Task<Response> Add(CompanyItemModel model);
        Task<Response> Update(CompanyItemModel model);
        Task<Response> UpdateStatus(Guid id, string companystatus);
        Task<List<CompanyItemModel>> ByTaxpayer(Guid taxpayerId);
        Task<List<CompanyItemModel>> ByTaxpayer(Guid[] taxpayerIds);
        Task<CompanyItemModel> ById(Guid id);
        Task<object> ByTaxpayerpaginated(Guid id, PageModel pageModel);
    }
}

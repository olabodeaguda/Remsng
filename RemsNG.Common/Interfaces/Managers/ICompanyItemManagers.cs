using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ICompanyItemManagers
    {
        Task<Response> Add(CompanyItemModel companyItem);
        Task<Response> Update(CompanyItemModel companyItem);
        Task<Response> UpdateStatus(Guid id, string companystatus);
        Task<List<CompanyItemModel>> ByTaxpayer(Guid companyId);
        Task<CompanyItemModelExt> ById(Guid id);
        Task<object> ByTaxpayerpaginated(Guid id, PageModel pageModel);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface ICompanyItemService
    {
        Task<Response> Add(CompanyItem companyItem);
        Task<Response> Update(CompanyItem companyItem);
        Task<Response> UpdateStatus(Guid id, string companystatus);
        Task<List<CompanyItemExt>> ByTaxpayer(Guid companyId);
        Task<CompanyItemExt> ById(Guid id);
        Task<object> ByTaxpayerpaginated(Guid id, PageModel pageModel);
    }
}

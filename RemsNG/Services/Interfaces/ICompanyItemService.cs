using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
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

using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        Task<Response> Add(CompanyModel company);
        Task<Response> Update(CompanyModel company);
        Task<Response> UpdateStatus(CompanyModel company);
        Task<CompanyModel> ById(Guid id);
        Task<List<CompanyExtModel>> ByLcda(Guid lcdaId);
        Task<object> ByLcda(Guid lcdaId, PageModel pageModel);
        Task<List<CompanyModel>> ByStretId(Guid streetId);

    }
}

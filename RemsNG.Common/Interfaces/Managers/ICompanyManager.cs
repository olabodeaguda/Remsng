using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ICompanyManager
    {
        Task<Response> Add(CompanyModel company);
        Task<Response> Update(CompanyModel company);
        Task<Response> UpdateStatus(CompanyModel company);
        Task<CompanyModel> ById(Guid id);
        Task<List<CompanyExtModel>> ByLcda(Guid lcdaId);
        Task<object> ByLcda(Guid lcdaId, PageModel pageModel);
        Task<List<CompanyModel>> ByStreetId(Guid streetId);
    }
}

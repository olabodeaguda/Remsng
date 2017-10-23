using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface ICompany
    {
        Task<Response> Add(Company company);
        Task<Response> Update(Company company);
        Task<Response> UpdateStatus(Company company);
        Task<Company> ById(Guid id);
        Task<List<CompanyExt>> ByLcda(Guid lcdaId);
        Task<object> ByLcda(Guid lcdaId, PageModel pageModel);
    }
}

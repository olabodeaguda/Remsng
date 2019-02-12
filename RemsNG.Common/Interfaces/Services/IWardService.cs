using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IWardService
    {
        Task<List<WardModel>> all();
        Task<List<WardModel>> ActiveWard();
        Task<bool> Add(WardModel ward);
        Task<object> Paginated(Models.PageModel pageModel);
        Task<object> Paginated(Models.PageModel pageModel, Guid lgdaId);
        Task<WardModel> GetWard(Guid id);
        Task<List<WardModel>> GetWardByLGDAId(Guid lgdaId);
        Task<bool> Update(WardModel ward);
        Task<WardModel> GetWard(string wardName, Guid lgdaid);
        Task<DomainModel> GetDomain(Guid wardId);
    }
}

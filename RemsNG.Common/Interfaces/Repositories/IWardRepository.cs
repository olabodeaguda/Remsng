using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IWardRepository
    {
        Task<List<WardModel>> All();
        Task<List<WardModel>> ActiveWard();
        Task<bool> Add(WardModel ward);
        Task<object> Paginated(PageModel pageModel, Guid lgdaId);
        Task<object> Paginated(PageModel pageModel);
        Task<WardModel> GetWard(Guid id);
        Task<WardModel> GetWard(string wardName, Guid lgdaid);
        Task<List<WardModel>> GetWardByLGDAId(Guid lgdaId);
        Task<bool> Update(WardModel ward);
        Task<DomainModel> GetDomain(Guid wardId);
    }
}

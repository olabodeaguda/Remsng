using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IStreetRepository
    {
        Task<Response> Add(StreetModel street);
        Task<Response> Update(StreetModel street);
        Task<Response> ChangeStatus(Guid id, string streetStatus);
        Task<StreetModel> ById(Guid streetId);
        Task<List<StreetModel>> ByWard(Guid wardId);
        Task<object> ByWardpaginated(Guid wardId, PageModel pageModel);
        Task<int> ByWardCount(Guid wardId);
        Task<List<StreetModel>> ByLcda(Guid lcdaId);
        Task<DomainModel> GetDomain(Guid streetId);
        Task<List<StreetModel>> SearchStreet(Guid wardId, string searchName);
    }
}

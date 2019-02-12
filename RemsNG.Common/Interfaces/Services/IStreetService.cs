using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IStreetService
    {
        Task<Response> ChangeStatus(Guid id, string streetStatus);
        Task<Response> Update(Street street);
        Task<Response> Add(Street street);
        Task<Street> ById(Guid streetId);
        Task<List<Street>> ByWard(Guid wardId);
        Task<int> ByWardCount(Guid wardId);
        Task<object> ByWardpaginated(Guid wardId, Models.PageModel pageModel);
        Task<List<Street>> ByLcda(Guid lcdaId);
        Task<Domain> GetDomain(Guid streetId);
        Task<List<Street>> Search(Guid wardId, string searchName);
    }
}

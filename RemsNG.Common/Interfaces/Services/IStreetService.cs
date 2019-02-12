using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IStreetService
    {
        Task<Response> ChangeStatus(Guid id, string streetStatus);
        Task<Response> Update(StreetModel street);
        Task<Response> Add(StreetModel street);
        Task<StreetModel> ById(Guid streetId);
        Task<List<StreetModel>> ByWard(Guid wardId);
        Task<int> ByWardCount(Guid wardId);
        Task<object> ByWardpaginated(Guid wardId, Models.PageModel pageModel);
        Task<List<StreetModel>> ByLcda(Guid lcdaId);
        Task<DomainModel> GetDomain(Guid streetId);
        Task<List<StreetModel>> Search(Guid wardId, string searchName);
    }
}

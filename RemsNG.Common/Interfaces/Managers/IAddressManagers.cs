using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IAddressManagers
    {
        Task<Response> Add(AddressModel address);
        Task<Response> Update(AddressModel address);
        Task<Response> Remove(AddressModel address);
        Task<List<AddressModel>> ByOwnersId(Guid id, Guid lcdaId);
        Task<AddressModel> ById(Guid id);
        Task<List<AddressModel>> ByOwnersId(Guid id);
        Task<string> AddressByOwnerId(Guid ownerId);
    }
}

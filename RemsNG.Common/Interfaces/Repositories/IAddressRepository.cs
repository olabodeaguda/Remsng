using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IAddressRepository
    {
        Task<Response> Add(AddressModel address);
        Task<Response> Update(AddressModel address);
        Task<Response> Remove(AddressModel address);
        Task<List<AddressModel>> ByOwnersId(Guid id, Guid lcdaId);
        Task<List<AddressModel>> ByOwnersId(Guid id);
        Task<AddressModel> ById(Guid id);

    }
}

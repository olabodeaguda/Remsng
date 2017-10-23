using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IAddress
    {
        Task<Response> Add(Address address);
        Task<Response> Update(Address address);
        Task<Response> Remove(Address address);
        Task<List<Address>> ByOwnersId(Guid id, Guid lcdaId);
        Task<Address> ById(Guid id);
    }
}

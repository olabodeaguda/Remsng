using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IErrorService
    {
        Task<bool> Add(Error error);
        Task<List<Error>> ByOwnerIdAsync(Guid ownerId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IErrorService
    {
        Task<bool> Add(Error error);
        Task<List<Error>> ByOwnerIdAsync(Guid ownerId);
    }
}

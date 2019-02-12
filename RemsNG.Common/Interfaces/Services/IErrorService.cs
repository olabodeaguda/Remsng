using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IErrorService
    {
        Task<bool> Add(ErrorModel error);
        Task<List<ErrorModel>> ByOwnerIdAsync(Guid ownerId);
    }
}

using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IErrorRepository
    {
        Task<bool> Add(ErrorModel error);
        Task<List<ErrorModel>> ByOwnerIdAsync(Guid ownerId);
    }
}

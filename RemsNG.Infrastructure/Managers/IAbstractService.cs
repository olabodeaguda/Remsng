using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IAbstractService
    {
        Task<bool> ExecuteQueryAsync(string query);
    }
}

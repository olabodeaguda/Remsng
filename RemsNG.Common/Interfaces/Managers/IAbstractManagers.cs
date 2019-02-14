using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IAbstractManagers
    {
        Task<bool> ExecuteQueryAsync(string query);
    }
}

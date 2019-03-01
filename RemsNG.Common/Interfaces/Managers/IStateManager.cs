using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IStateManagers
    {
        Task<List<StateModel>> All();
        Task<StateModel> ByLcda(Guid lcdaId);
        Task<string> StateNameByLcda(Guid lcdaId);
    }
}

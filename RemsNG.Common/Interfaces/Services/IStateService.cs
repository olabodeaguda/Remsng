using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IStateService
    {
        Task<List<State>> All();
        Task<State> ByLcda(Guid lcdaId);
        Task<string> StateNameByLcda(Guid lcdaId);
    }
}

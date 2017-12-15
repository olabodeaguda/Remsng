using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IStateService
    {
        Task<List<State>> All();
        Task<State> ByLcda(Guid lcdaId);
        Task<string> StateNameByLcda(Guid lcdaId);
    }
}

using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IStateService
    {
        Task<List<StateModel>> All();
        Task<StateModel> ByLcda(Guid lcdaId);
        Task<string> StateNameByLcda(Guid lcdaId);
    }
}

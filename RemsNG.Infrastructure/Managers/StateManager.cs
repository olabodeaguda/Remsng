using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class StateManagers : IStateManagers
    {
        private readonly IStateRepository stateDao;
        public StateManagers(IStateRepository stateRepository)
        {
            stateDao = stateRepository;
        }

        public async Task<List<StateModel>> All()
        {
            return await stateDao.All();
        }

        public async Task<StateModel> ByLcda(Guid lcdaId)
        {
            return await stateDao.ByLcda(lcdaId);
        }

        public async Task<string> StateNameByLcda(Guid lcdaId)
        {
            StateModel state = await stateDao.ByLcda(lcdaId);
            if (state != null)
            {
                return state.StateName;
            }
            return string.Empty;
        }



    }
}

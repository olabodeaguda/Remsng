using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class StateManagers : IStateManagers
    {
        private readonly StateRepository stateDao;
        public StateManagers(RemsDbContext _db)
        {
            stateDao = new StateRepository(_db);
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

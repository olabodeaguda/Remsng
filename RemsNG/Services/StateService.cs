using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class StateService : IStateService
    {
        private readonly StateDao stateDao;
        public StateService(RemsDbContext _db)
        {
            stateDao = new StateDao(_db);
        }

        public async Task<List<State>> All()
        {
            return await stateDao.All();
        }

        public async Task<State> ByLcda(Guid lcdaId)
        {
            return await stateDao.ByLcda(lcdaId);
        }

        public async Task<string> StateNameByLcda(Guid lcdaId)
        {
            State state = await stateDao.ByLcda(lcdaId);
            if (state != null)
            {
                return state.stateName;
            }
            return string.Empty;
        }



    }
}

using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class StateRepository : AbstractRepository
    {
        public StateRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<State>> All()
        {
            return await db.States.OrderBy(x => x.StateName).ToListAsync();
        }

        public async Task<State> ByLcda(Guid lcdaId)
        {
            string query = $" select distinct st.* from tbl_domain as dm " +
                $"inner join tbl_lcda as lc on lc.domainId = dm.id " +
                $"inner join tbl_state as st on st.id = dm.stateId " +
                $"where lc.id ='{lcdaId}'";

            return await db.States.FromSql(query).FirstOrDefaultAsync();
        }
    }
}

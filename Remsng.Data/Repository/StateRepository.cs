﻿using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class StateRepository : AbstractRepository
    {
        public StateRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<StateModel>> All()
        {
            var result = await db.States.OrderBy(x => x.StateName).ToListAsync();
            return result.Select(x => new StateModel()
            {
                CountryId = x.CountryId,
                Id = x.Id,
                StateCode = x.StateCode,
                StateName = x.StateName
            }).ToList();
        }

        public async Task<StateModel> ByLcda(Guid lcdaId)
        {
            string query = $" select distinct st.* from tbl_domain as dm " +
                $"inner join tbl_lcda as lc on lc.domainId = dm.id " +
                $"inner join tbl_state as st on st.id = dm.stateId " +
                $"where lc.id ='{lcdaId}'";
            var x = await db.States.FromSql(query).FirstOrDefaultAsync();
            if (x == null)
            {
                return null;
            }
            return new StateModel()
            {
                CountryId = x.CountryId,
                Id = x.Id,
                StateCode = x.StateCode,
                StateName = x.StateName
            };
        }
    }
}

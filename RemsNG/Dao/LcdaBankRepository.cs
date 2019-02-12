using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class LcdaBankRepository : AbstractRepository
    {
        public LcdaBankRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<BankLcda>> Get(Guid lcdaId)
        {
            return await db.LcdaBanks.FromSql("sp_getLcdaBank @p0", new object[] { lcdaId }).ToListAsync();
        }
    }
}

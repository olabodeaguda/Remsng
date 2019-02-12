using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using Microsoft.EntityFrameworkCore;

namespace RemsNG.Dao
{
    public class LcdaBankDao : AbstractRepository
    {
        public LcdaBankDao(RemsDbContext _db) : base(_db)
        {
        }

        public  async Task<List<LcdaBank>> Get(Guid lcdaId)
        {
            return await db.LcdaBanks.FromSql("sp_getLcdaBank @p0", new object[] { lcdaId }).ToListAsync();
        }
    }
}

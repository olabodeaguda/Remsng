using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using Microsoft.EntityFrameworkCore;

namespace RemsNG.Dao
{
    public class LcdaPropertyDao : AbstractDao
    {
        public LcdaPropertyDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<LcdaProperty>> ByLcda(Guid lcdaId)
        {
            return await db.LcdaProperties.Where(x => x.lcdaId == lcdaId && x.propertyStatus == "ACTIVE").ToListAsync();
        }
    }
}

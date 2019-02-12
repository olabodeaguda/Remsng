using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class LcdaPropertyRepository : AbstractRepository
    {
        public LcdaPropertyRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<LcdaProperty>> ByLcda(Guid lcdaId)
        {
            return await db.LcdaProperties.Where(x => x.LcdaId == lcdaId && x.PropertyStatus == "ACTIVE").ToListAsync();
        }
    }
}

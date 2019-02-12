using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class LcdaPropertyRepository : AbstractRepository
    {
        public LcdaPropertyRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<LcdaPropertyModel>> ByLcda(Guid lcdaId)
        {
            var result = await db.LcdaProperties.Where(x => x.LcdaId == lcdaId && x.PropertyStatus == "ACTIVE").ToListAsync();
            return result.Select(x => new LcdaPropertyModel()
            {
                Id = x.Id,
                LcdaId = x.LcdaId,
                PropertyKey = x.PropertyKey,
                PropertyStatus = x.PropertyStatus,
                PropertyValue = x.PropertyValue
            }).ToList();
        }
    }
}

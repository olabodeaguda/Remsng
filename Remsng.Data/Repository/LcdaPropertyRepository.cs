using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class LcdaPropertyRepository : ILcdaPropertyRepository
    {
        private readonly DbContext db;
        public LcdaPropertyRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<List<LcdaPropertyModel>> ByLcda(Guid lcdaId)
        {
            var result = await db.Set<LcdaProperty>()
                .Where(x => x.LcdaId == lcdaId && x.PropertyStatus == "ACTIVE").ToListAsync();
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

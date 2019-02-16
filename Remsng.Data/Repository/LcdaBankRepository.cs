using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class LcdaBankRepository : AbstractRepository
    {
        public LcdaBankRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<List<BankLcdaModel>> Get(Guid lcdaId)
        {
            var result = await db.Set<BankLcda>()
                .FromSql("sp_getLcdaBank @p0", new object[] { lcdaId }).ToListAsync();
            return result.Select(x => new BankLcdaModel()
            {
                bankAccount = x.BankAccount,
                bankId = x.BankId.Value,
                // bankName = x.BankName
                id = x.Id,
                lcdaId = x.LcdaId.Value
            }).ToList();
        }
    }
}

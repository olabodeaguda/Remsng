using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class LcdaBankRepository : ILcdaBankRepository
    {
        private readonly DbContext db;
        public LcdaBankRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<List<BankLcdaModel>> Get(Guid lcdaId)
        {
            var result = await db.Set<BankLcda>()
                .Include(x => x.Bank)
                .Include(x => x.Lcda)
                .Where(p => p.LcdaId == lcdaId).Select(s => new BankLcdaModel()
                {
                    bankAccount = s.BankAccount,
                    bankId = s.BankId,
                    bankName = s.Bank.BankName,
                    id = s.Id,
                    lcdaId = s.LcdaId
                }).ToListAsync();

            return result;

            //await db.Set<BankLcda>()
            //.FromSql("sp_getLcdaBank @p0", new object[] { lcdaId }).ToListAsync();

            //return result.Select(x => new BankLcdaModel()
            //{
            //    bankAccount = x.BankAccount,
            //    bankId = x.BankId,
            //    // bankName = x.BankName
            //    id = x.Id,
            //    lcdaId = x.LcdaId
            //}).ToList();
        }
    }
}

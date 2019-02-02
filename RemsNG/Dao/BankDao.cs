using Microsoft.EntityFrameworkCore;
using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class BankDao : AbstractDao
    {
        public BankDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<Bank>> GetBankAsync()
        {
            return await db.Banks.FromSql("select * from tbl_bank order by bankName").ToListAsync();
        }

        public async Task<BankCategory[]> GetBankCategory(Guid lcdaId)
        {

            return new BankCategory[] { };
        }

    }
}

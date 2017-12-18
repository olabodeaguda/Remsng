using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using Microsoft.EntityFrameworkCore;

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

    }
}

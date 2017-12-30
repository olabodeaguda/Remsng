using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RemsNG.Dao
{
    public class AbstractDao
    {
        protected readonly RemsDbContext db;
        public AbstractDao(RemsDbContext _db)
        {
            this.db = _db;
        }

        public async Task<bool> ExecuteQueryAsync(string query)
        {
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
        }
    }
}

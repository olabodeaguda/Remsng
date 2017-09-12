using Microsoft.EntityFrameworkCore;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DomainDao : AbstractDao
    {
        public DomainDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<Domain>> GetUserDomainByUsername(string username)
        {
            return await Task.Run(() =>
            {
                return db.Domains.FromSql("sp_getUserDomainByUsername @p0", new object[] { username }).ToList();
            });
        }
    }
}

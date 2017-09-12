using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class AbstractDao
    {
        protected readonly RemsDbContext db;
        public AbstractDao(RemsDbContext _db)
        {
            this.db = _db;
        }
    }
}

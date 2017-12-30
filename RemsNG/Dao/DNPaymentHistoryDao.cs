using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;

namespace RemsNG.Dao
{
    public class DNPaymentHistoryDao : AbstractDao
    {
        public DNPaymentHistoryDao(RemsDbContext _db) : base(_db)
        {}

        
    }
}

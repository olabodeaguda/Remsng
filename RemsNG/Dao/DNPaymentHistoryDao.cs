using Microsoft.EntityFrameworkCore;
using RemsNG.ORM;
using System;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DNPaymentHistoryDao : AbstractDao
    {
        public DNPaymentHistoryDao(RemsDbContext _db) : base(_db)
        { }

        public async Task<Prepayment> Get(Guid taxpayerId)
        {
            return await db.Prepayments.FirstOrDefaultAsync(x => x.taxpayerId == taxpayerId && x.prepaymentStatus == "ACTIVE");
        }
    }
}

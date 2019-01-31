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

        public async Task<Prepayment> AddPrepaymentForAlreadyRegisterdAmount(Prepayment prepayment)
        {
            var prep = await db.Prepayments
                .FirstOrDefaultAsync(x => x.taxpayerId == prepayment.taxpayerId
                && prepayment.amount == x.amount && x.prepaymentStatus != "CLOSED");

            if (prep == null)
            {
                prepayment.datecreated = DateTime.Now;
                db.Prepayments.Add(prepayment);
                await db.SaveChangesAsync();
                return prepayment;
            }
            return prepayment;
        }
    }
}

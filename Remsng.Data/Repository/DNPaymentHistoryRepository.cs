using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using Remsng.Data.Entities;
using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DNPaymentHistoryRepository : AbstractRepository
    {
        public DNPaymentHistoryRepository(RemsDbContext _db) : base(_db)
        { }

        public async Task<PrepaymentModel> Get(Guid taxpayerId)
        {
            var r = await db.Prepayments.FirstOrDefaultAsync(x => x.taxpayerId == taxpayerId && x.prepaymentStatus == "ACTIVE");
            if (r == null)
            {
                return null;
            }

            return new PrepaymentModel()
            {
                amount = r.amount,
                datecreated = r.datecreated,
                id = r.id,
                prepaymentStatus = r.prepaymentStatus,
                taxpayerId = r.taxpayerId
            };
        }

        public async Task<PrepaymentModel> AddPrepaymentForAlreadyRegisterdAmount(PrepaymentModel prepayment)
        {
            var prep = await db.Prepayments
                .FirstOrDefaultAsync(x => x.taxpayerId == prepayment.taxpayerId
                && prepayment.amount == x.amount && x.prepaymentStatus != "CLOSED");

            if (prep == null)
            {
                Prepayment pm = new Prepayment()
                {
                    taxpayerId = prepayment.taxpayerId,
                    amount = prepayment.amount,
                    datecreated = prepayment.datecreated,
                    id = prepayment.id,
                    prepaymentStatus = prepayment.prepaymentStatus
                };
                pm.datecreated = DateTime.Now;
                db.Prepayments.Add(pm);
                await db.SaveChangesAsync();
                prepayment.id = pm.id;
                return prepayment;
            }
            return prepayment;
        }
    }
}

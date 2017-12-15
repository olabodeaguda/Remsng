using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using Microsoft.EntityFrameworkCore;
using RemsNG.Models;
using RemsNG.Utilities;

namespace RemsNG.Dao
{
    public class DNAmountDueMgtDao : AbstractDao
    {
        public DNAmountDueMgtDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<DNAmountDueModel>> ByBillingNo(string billingno)
        {
            return await db.DNAmountDueModels.FromSql("sp_getBillingNumberTotalDue @p0", new object[] { billingno }).ToListAsync();
        }

        public async Task<Response> UpdateAmount(DNAmountDueModel dnamount)
        {
            int count = 0;
            if (dnamount.category == "ARREARS")
            {
                string query = $"Update tbl_demandNoticeArrears set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";

                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category == "PENALTY")
            {
                string query = $"Update tbl_demandNoticePenalty set totalAmount={dnamount.itemAmount} where id='{dnamount.id}'";
                count = await db.Database.ExecuteSqlCommandAsync(query);
            }
            else if (dnamount.category == "ITEMS")
            {
                string query = $"Update tbl_demandNoticeItem set itemAmount={dnamount.itemAmount} where id='{dnamount.id}'";
                count = await db.Database.ExecuteSqlCommandAsync(query);
            }

            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Update was not successful.Please try again!!!"
                };
            }

        }

    }
}

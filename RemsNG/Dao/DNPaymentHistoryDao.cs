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

        private string PaymentQuery(List<DNAmountDueModel> paymentDueList, DemandNoticePaymentHistory dnph)
        {
            string query = "";

            if (paymentDueList.Count > 0)
            {
                foreach (var tm in paymentDueList)
                {
                    switch (tm.category)
                    {
                        case "ARREARS":
                            query = query + $"";
                            break;
                        case "PENALTY":
                            query = query + $"";
                            break;
                        case "ITEMS":
                            query = query + $"";
                            break;
                        default:
                            break;
                    }
                }
            }

            return query;
        }
    }
}

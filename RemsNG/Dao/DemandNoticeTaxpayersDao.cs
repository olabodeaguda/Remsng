using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;

namespace RemsNG.Dao
{
    public class DemandNoticeTaxpayersDao : AbstractDao
    {
        public DemandNoticeTaxpayersDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<DemandNoticeTaxpayersDetails>> getTaxpayerId(string[] ids, int billingYr)
        {
            string tIds = "";
            for (int i = 0; i < ids.Length; i++)
            {
                tIds = tIds + $"'{ids[0]}'";
                if (i < (ids.Length - 1))
                {
                    tIds = tIds + ",";
                }
            }

            string query = $"select taxpayerId from tbl_demandNoticeTaxpayers where taxpayerId in ({tIds}) and billingYr = {billingYr}";
            return await db.Set<DemandNoticeTaxpayersDetails>().FromSql(query).ToListAsync();
        }

    }
}

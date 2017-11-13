using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;
using RemsNG.Utilities;

namespace RemsNG.Dao
{
    public class DemandNoticeItemDao : AbstractDao
    {
        public DemandNoticeItemDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(DemandNoticeTaxpayersDetail dntd)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addTaxpayerDemandNoticeItem @p0,@p1,@p2", new object[] {
                dntd.dnId,
                dntd.taxpayerId,
                dntd.billingYr
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }
    }
}

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
    public class DemandNoticePenaltyDao : AbstractDao
    {
        public DemandNoticePenaltyDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> AddUnpaidPenaltyAsync(DN_ArrearsModel dN_ArrearsModel)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_moveTaxpayersPenalty @p0, @p1, @p2, @p3", new object[] {
                dN_ArrearsModel.billingNo,
                dN_ArrearsModel.taxpayerId,
                dN_ArrearsModel.billingYr,
                dN_ArrearsModel.createdBy
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

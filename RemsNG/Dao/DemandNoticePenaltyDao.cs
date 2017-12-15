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

        public string AddQuery(DemandNoticeItemPenalty dnp)
        {
            return $"INSERT INTO tbl_demandNoticePenalty" +
                $" (id ,billingNo,taxpayerId ,totalAmount,amountPaid,itemId" +
                $" ,originatedYear,billingYear,itemPenaltyStatus,createdBy,dateCreated)" +
                $" VALUES ('{Guid.NewGuid()}','{dnp.billingNo}','{dnp.taxpayerId}','{dnp.totalAmount}','{dnp.amountPaid}','{dnp.itemId}'" +
                $",'{dnp.originatedYear}','{dnp.billingYr}','{dnp.itemPenaltyStatus}','APPLICATION','{DateTime.Now}');";
        }

        public Response RunQuery(string query)
        {
            int count = db.Database.ExecuteSqlCommand(query);
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{count} demand notice has been penalized on {DateTime.Now}"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"Zero demand notice has been penalized on {DateTime.Now}"
                };
            }
        }

        public async Task<Response> AddUnpaidPenaltyAsync(DN_ArrearsModel dN_ArrearsModel)
        {
            try
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
            catch (Exception x)
            {

                throw;
            }
        }

        public async Task<List<DemandNoticeItem>> OverDueDemandNotice()
        {
            try
            {
                List<DemandNoticeItem> demandNotice = await db.DemandNoticeItems.FromSql("sp_penaltyTracker").ToListAsync();
                return demandNotice;
            }
            catch (Exception x)
            {

                throw;
            }
        }

        public async Task<List<DemandNoticeItemPenalty>> ByBillingNumber(string billingno)
        {
            List<DemandNoticeItemPenalty> lstdbItem = await db.DemandNoticeItemPenaties
                .FromSql($"select tbl_demandNoticePenalty.*,0 as billingYr from tbl_demandNoticePenalty where billingNo = '{billingno}'").ToListAsync();
            return lstdbItem;
        }

    }
}

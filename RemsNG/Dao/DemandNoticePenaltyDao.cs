using Microsoft.EntityFrameworkCore;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            catch (Exception)
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
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<DemandNoticeItemPenalty>> ByBillingNumber(string billingno)
        {
            string query = $"select tbl_demandNoticePenalty.*,0 as billingYr from tbl_demandNoticePenalty where billingNo = '{billingno}'";
            List<DemandNoticeItemPenalty> lstdbItem = await db.DemandNoticeItemPenaties
                .FromSql(query).ToListAsync();
            return lstdbItem;
        }

        public async Task<List<DemandNoticeItemPenalty>> ByTaxpayerId(Guid taxpayerId)
        {
            string query = $"select tbl_demandNoticePenalty.*,0 as billingYr from tbl_demandNoticePenalty where taxpayerId = '{taxpayerId}'";
            List<DemandNoticeItemPenalty> lstdbItem = await db.DemandNoticeItemPenaties
                .FromSql(query).ToListAsync();
            return lstdbItem;
        }

        public async Task<List<DemandNoticeItemPenalty>> ByTaxpayerId(Guid taxpayerId, int billingYr)
        {
            string query = $"select tbl_demandNoticePenalty.*,0 as billingYr from tbl_demandNoticePenalty " +
                $"where taxpayerId = '{taxpayerId}' and billingYear = {billingYr}";
            List<DemandNoticeItemPenalty> lstdbItem = await db.DemandNoticeItemPenaties
                .FromSql(query).ToListAsync();
            return lstdbItem;
        }

        public async Task<List<DemandNoticeItemPenaltyExt>> ReportByCategory(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            DateTime endDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            List<DemandNoticeItemPenaltyExt> lst =
                await db.DemandNoticeItemPenaltyExts.FromSql("sp_getPenaltyByCategoryDate @p0,@p1",
                new object[] { startDate, endDate }).ToListAsync();

            return lst;
        }

    }
}


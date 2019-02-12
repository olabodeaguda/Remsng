using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DemandNoticeItemRepository : AbstractRepository
    {
        public DemandNoticeItemRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(DemandNoticeTaxpayers dntd)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addTaxpayerDemandNoticeItem @p0,@p1,@p2,@p3", new object[] {
                dntd.DnId,
                dntd.TaxpayerId,
                dntd.BillingYr,
                dntd.CreatedBy
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

        public async Task<List<DemandNoticeItem>> ByBillingNumber(string billingno)
        {
            List<DemandNoticeItem> lstdbItem = await db.DemandNoticeItems.
                    FromSql($"select tbl_demandNoticeItem.*,0.0 as penaltyAmount,'nil' as duration,-1 " +
                    $" as billingYr from tbl_demandNoticeItem where billingNo = '{billingno}' " +
                    $"and itemStatus not in ('CANCEL')").ToListAsync();
            // $"and itemStatus in ('PENDING','PART_PAYMENT','PAID','CANCEL')").ToListAsync();
            return lstdbItem;
        }

        public async Task<List<DemandNoticeItem>> UnpaidBillsByTaxpayerId(Guid taxpayerId, string billNumber, int billingYr)
        {
            string query = $"select tbl_demandNoticeItem.*,0.0 as penaltyAmount,'nil' as duration,billingYr as billingYr from tbl_demandNoticeItem " +
                $"inner join tbl_demandNoticeTaxpayers on tbl_demandNoticeTaxpayers.taxpayerId = tbl_demandNoticeItem.taxpayerId " +
                $"where billingYr={billingYr} and tbl_demandNoticeTaxpayers.demandNoticeStatus in ('PENDING','PART_PAYMENT') " +
                $"and tbl_demandNoticeItem.taxpayerId = '{taxpayerId}' and billingNo <> '{billNumber}'";

            List<DemandNoticeItem> lstdbItem = await db.DemandNoticeItems.
                    FromSql(query).ToListAsync();
            return lstdbItem;
        }

        public async Task<List<DemandNoticeItemModelExt>> ReportByCategory(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            DateTime endDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            List<DemandNoticeItemModelExt> lst =
                await db.DemandNoticeItemExts.FromSql("sp_getByCategoryDate @p0,@p1",
                new object[] { startDate, endDate }).ToListAsync();

            return lst;
        }
    }
}

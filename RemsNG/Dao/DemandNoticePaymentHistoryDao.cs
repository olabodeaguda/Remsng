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
    public class DemandNoticePaymentHistoryDao : AbstractDao
    {
        public DemandNoticePaymentHistoryDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> AddAsync(DemandNoticePaymentHistory dnph)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addPayment @p0,@p1,@p2,@p3,@p4,@p5" +
                ",@p6,@p7,@p8", new object[] {
                dnph.ownerId,
                dnph.billingNumber,
                dnph.amount,
                dnph.charges,
                dnph.paymentMode,
                dnph.referenceNumber,
                dnph.bankId,
                dnph.createdBy,dnph.dateCreated
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

        public async Task<Response> UpdateAsync(DemandNoticePaymentHistory dnph)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addPayment  @p0,@p1,@p2,@p3,@p4,@p5", new object[] {
                dnph.id,
                dnph.bankId,
                dnph.referenceNumber,
                dnph.amount,
                dnph.charges,
                dnph.paymentMode,
                dnph.createdBy
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    data = dbResponse.msg
                };
            }
        }

        public async Task<Response> UpdateStatusAsync(DemandNoticePaymentHistory dnph)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updatePaymentHistoryStatus @p0,@p1", new object[] {
                dnph.id,
                dnph.paymentStatus
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    data = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    data = dbResponse.msg
                };
            }
        }

        public async Task<List<DemandNoticePaymentHistory>> ByBillingNumber(string billingnumber)
        {
            string query = "select dnph.*,bank.bankName from tbl_demandNoticePaymentHistory as dnph " +
                $"inner join tbl_bank bank on bank.id = dnph.bankId where billingNumber = '{billingnumber}'";

            return await db.DemandNoticePaymentHistories.FromSql(query).ToListAsync();
        }

        public async Task<List<DemandNoticePaymentHistory>> ByBillingNumbers(string billingnumber)
        {
            string query = "select dnph.*,bank.bankName from tbl_demandNoticePaymentHistory as dnph " +
                $"inner join tbl_bank bank on bank.id = dnph.bankId where paymentStatus = 'APPROVED' and billingNumber in ({billingnumber})";

            return await db.DemandNoticePaymentHistories.FromSql(query).ToListAsync();
        }

        public async Task<DemandNoticePaymentHistory> ById(Guid id)
        {
            string query = "select dnph.*,bank.bankName from tbl_demandNoticePaymentHistory as dnph " +
                 $"inner join tbl_bank bank on bank.id = dnph.bankId where dnph.id = '{id}'";

            return await db.DemandNoticePaymentHistories.FromSql(query).FirstOrDefaultAsync();
        }

        public async Task<DemandNoticePaymentHistory> ByIdExtended(Guid id)
        {
            string query = "select dnph.*,bank.bankName from tbl_demandNoticePaymentHistory as dnph " +
                $"inner join tbl_bank bank on bank.id = dnph.bankId where dnph.id = '{id}'";
            
            return await db.DemandNoticePaymentHistories.FromSql(query).FirstOrDefaultAsync();
        }

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.Set<DemandNoticePaymentHistoryExt>()
                .FromSql("sp_paymenthistoryByLcda @p0,@p1,@p2",
                new object[] { lcdaId, pageModel.PageSize, pageModel.PageNum }).ToListAsync();
            int totalCount = 0;
            if (results.Count > 0)
            {
                totalCount = results[0].totalSize;
            }

            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

    }
}

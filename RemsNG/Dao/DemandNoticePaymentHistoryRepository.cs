﻿using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DemandNoticePaymentHistoryRepository : AbstractRepository
    {
        public DemandNoticePaymentHistoryRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> AddAsync(DemandNoticePaymentHistory dnph)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addPayment @p0,@p1,@p2,@p3,@p4,@p5" +
                ",@p6,@p7,@p8,@p9", new object[] {
                dnph.OwnerId,
                dnph.BillingNumber,
                dnph.Amount,
                dnph.Charges,
                dnph.PaymentMode,
                dnph.ReferenceNumber,
                dnph.BankId,
                dnph.CreatedBy,
                dnph.DateCreated,
                dnph.IsWaiver
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
                dnph.Id,
                dnph.BankId,
                dnph.ReferenceNumber,
                dnph.Amount,
                dnph.Charges,
                dnph.PaymentMode,
                dnph.CreatedBy
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
                dnph.Id,
                dnph.PaymentStatus
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

        public async Task<List<DemandNoticePaymentHistoryModelExt>> ByBillingNumber(string billingnumber)
        {
            //string query = $"select dnph.*,bank.bankName from tbl_demandNoticePaymentHistory as dnph " +
            //    $"inner join tbl_bank bank on bank.id = dnph.bankId where billingNumber = '{billingnumber}'";

            string query = $"select dnph.*,-1 as totalSize,dnp.billingYr as billingYear, " +
                $"dnp.taxpayersName from tbl_demandNoticePaymentHistory as dnph " +
                $"inner join tbl_demandNoticeTaxpayers as dnp on dnp.billingNumber = dnph.billingNumber where dnph.billingNumber = '{billingnumber}'";

            return await db.DemandNoticePaymentHistoryExts.FromSql(query).ToListAsync();
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
            var results = await db.Set<DemandNoticePaymentHistoryModelExt>()
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

        public async Task<List<DemandNoticePaymentHistory>> ApprovedPaymentHistory(Guid ownerId, int currentYr)
        {
            string query = $"select tbl_demandNoticePaymentHistory.*,bank.bankName from tbl_demandNoticePaymentHistory " +
                $"inner join tbl_bank bank on bank.id = tbl_demandNoticePaymentHistory.bankId " +
                $"inner join tbl_demandNoticeTaxpayers on tbl_demandNoticeTaxpayers.taxpayerId = tbl_demandNoticePaymentHistory.ownerId " +
                $"where tbl_demandNoticePaymentHistory.ownerId = '{ownerId}' and tbl_demandNoticeTaxpayers.billingYr = {currentYr} " +
                $"and paymentStatus = 'APPROVED'";
            var result = await db.DemandNoticePaymentHistories.FromSql(query).ToListAsync();
            return result
                .GroupBy(x => x.Id)
                .Select(p => p.First())
                .ToList();
        }
    }
}
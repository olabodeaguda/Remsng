﻿using System;
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
                ",@p6,@p7", new object[] {
                dnph.ownerId,
                dnph.billingNumber,
                dnph.amount,
                dnph.charges,
                dnph.paymentMode,
                dnph.referenceNumber,
                dnph.bankId,
                dnph.createdBy
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
            string query = $"select * from tbl_demandNoticePaymentHistory where billingNumber = '{billingnumber}'";
            return await db.DemandNoticePaymentHistories.FromSql(query).ToListAsync();
        }

        public async Task<DemandNoticePaymentHistory> ById(Guid id)
        {
            string query = $"select top 1 * from tbl_demandNoticePaymentHistory where id = '{id}'";
            return await db.DemandNoticePaymentHistories.FromSql(query).FirstOrDefaultAsync();
        }
    }
}
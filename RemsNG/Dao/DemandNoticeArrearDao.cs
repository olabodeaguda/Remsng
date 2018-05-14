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
    public class DemandNoticeArrearDao : AbstractDao
    {
        public DemandNoticeArrearDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> AddUnpaidArrearsAsync(DN_ArrearsModel dN_ArrearsModel)
        {
            try
            {
                DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_MovedDemandNoticeArrears @p0, @p1, @p2, @p3, @p4", new object[] {
                dN_ArrearsModel.billingNo,
                dN_ArrearsModel.taxpayerId,
                dN_ArrearsModel.billingYr,
                dN_ArrearsModel.arrearstatus,
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

        public async Task<Response> AddUnpaidDemandNoticeToArrearsAsync(DN_ArrearsModel dN_ArrearsModel)
        {
            try
            {
                DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_previousDemandNoticeArrears @p0, @p1, @p2, @p3, @p4", new object[] {
                dN_ArrearsModel.billingNo,
                dN_ArrearsModel.taxpayerId,
                dN_ArrearsModel.previousBillingYr,
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

        public async Task<Response> AddUnpaidDemandNoticeToArrearsAsync2(DN_ArrearsModel dN_ArrearsModel)
        {
            try
            {
                DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_cancelpreviousDemandNoticeArrears @p0, @p1, @p2, @p3, @p4", new object[] {
                dN_ArrearsModel.billingNo,
                dN_ArrearsModel.taxpayerId,
                dN_ArrearsModel.previousBillingYr,
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

        public async Task<Response> AddUnpaidDemandNoticeToArrears2Async(DN_ArrearsModel dN_ArrearsModel, string duration)
        {
            try
            {
                DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_MoveDemandNoticeToArrears @p0, @p1, @p2, @p3, @p4, @p5", new object[] {
                dN_ArrearsModel.billingNo,
                dN_ArrearsModel.taxpayerId,
                dN_ArrearsModel.previousBillingYr,
                dN_ArrearsModel.billingYr,
                dN_ArrearsModel.createdBy,
                duration
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

        public async Task<List<DemandNoticeArrears>> ByBillingNumber(string billingno)
        {
            List<DemandNoticeArrears> lstdbItem = await db.DemandNoticeArrearss
                .FromSql($"select tbl_demandNoticeArrears.*, 0 as billingYr from tbl_demandNoticeArrears where billingNo = {billingno}").ToListAsync();
            return lstdbItem;
        }

        public async Task<bool> AddArrears(DemandNoticeArrears dna)
        {
            string query = $"insert into tbl_demandNoticeArrears(id,billingNo,taxpayerId,totalAmount," +
                $" amountPaid,itemId,originatedYear," +
                $"billingYear,arrearsStatus,createdBy,dateCreated) values(" +
                $"'{Guid.NewGuid()}','{dna.billingNo}','{dna.taxpayerId}','{dna.totalAmount}',0," +
                $"'{dna.itemId}','{dna.originatedYear}','{dna.billingYr}','{dna.arrearsStatus}'," +
                $"'{dna.createdBy}',getdate())";

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
        }
    }
}

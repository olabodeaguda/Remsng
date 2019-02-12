using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DemandNoticeArrearRepository : AbstractRepository
    {
        public DemandNoticeArrearRepository(RemsDbContext _db) : base(_db)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<DemandNoticeArrears>> ByBillingNumber(string billingno)
        {
            List<DemandNoticeArrears> lstdbItem = await db.DemandNoticeArrearss
                .FromSql($"select tbl_demandNoticeArrears.*, 0 as billingYr from tbl_demandNoticeArrears " +
                $"where billingNo = '{billingno}'  and arrearsStatus in ('PENDING','PART_PAYMENT')").ToListAsync();
            List<DemandNoticeArrears> lst = new List<DemandNoticeArrears>();
            foreach (var tm in lstdbItem)
            {
                var t = lst.FirstOrDefault(x => x.billingNo == tm.billingNo && x.totalAmount == tm.totalAmount);
                if (t == null)
                {
                    lst.Add(tm);
                }
            }
            return lst;
        }

        public async Task<List<DemandNoticeArrears>> ByTaxpayer(Guid taxpayerId)
        {
            string query = $"select tbl_demandNoticeArrears.*, 0 as billingYr " +
                $"from tbl_demandNoticeArrears where taxpayerId = '{taxpayerId}' and arrearsStatus in ('PENDING','PART_PAYMENT')";

            return await db.DemandNoticeArrearss.FromSql(query).ToListAsync();
        }

        public string AddQuery(DemandNoticeArrears dna)
        {
            return $"insert into tbl_demandNoticeArrears(id,billingNo,taxpayerId,totalAmount," +
                $" amountPaid,itemId,originatedYear," +
                $"billingYear,arrearsStatus,createdBy,dateCreated) values(" +
                $"'{Guid.NewGuid()}','{dna.billingNo}','{dna.taxpayerId}','{dna.totalAmount}',0," +
                $"'{dna.itemId}','{dna.originatedYear}','{dna.billingYr}','{dna.arrearsStatus}'," +
                $"'{dna.createdBy}',getdate());";
        }

        public async Task<bool> AddArrears(DemandNoticeArrears dna)
        {
            string query = AddQuery(dna);

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> AddArrears(string query)
        {
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<DemandNoticeArrearsModelExt>> ReportByCategory(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            DateTime endDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            List<DemandNoticeArrearsModelExt> lst =
                await db.DemandNoticeArrearsExts.FromSql("sp_getArrearsByCategoryDate @p0,@p1",
                new object[] { startDate, endDate }).ToListAsync();

            return lst;
        }

    }
}

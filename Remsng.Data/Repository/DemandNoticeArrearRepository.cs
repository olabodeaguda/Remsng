﻿using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
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

        public async Task<List<DemandNoticeArrearsModel>> ByBillingNumber(string billingno)
        {
            List<DemandNoticeArrears> lstdbItem = await db.DemandNoticeArrearss
                .FromSql($"select tbl_demandNoticeArrears.*, 0 as billingYr from tbl_demandNoticeArrears " +
                $"where billingNo = '{billingno}'  and arrearsStatus in ('PENDING','PART_PAYMENT')").ToListAsync();
            List<DemandNoticeArrears> lst = new List<DemandNoticeArrears>();
            foreach (var tm in lstdbItem)
            {
                var t = lst.FirstOrDefault(x => x.BillingNo == tm.BillingNo && x.TotalAmount == tm.TotalAmount);
                if (t == null)
                {
                    lst.Add(tm);
                }
            }
            return lst.Select(x => new DemandNoticeArrearsModel()
            {
                AmountPaid = x.AmountPaid,
                ArrearsStatus = x.ArrearsStatus,
                BillingNo = x.BillingNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                ItemId = x.ItemId,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OriginatedYear = x.OriginatedYear,
                TaxpayerId = x.TaxpayerId,
                TotalAmount = x.TotalAmount
            }).ToList();
        }

        public async Task<List<DemandNoticeArrearsModel>> ByTaxpayer(Guid taxpayerId)
        {
            string query = $"select tbl_demandNoticeArrears.*, 0 as billingYr " +
                $"from tbl_demandNoticeArrears where taxpayerId = '{taxpayerId}' and arrearsStatus in ('PENDING','PART_PAYMENT')";

            var entity = await db.DemandNoticeArrearss.FromSql(query).ToListAsync();
            return entity.Select(x => new DemandNoticeArrearsModel()
            {
                AmountPaid = x.AmountPaid,
                ArrearsStatus = x.ArrearsStatus,
                BillingNo = x.BillingNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                ItemId = x.ItemId,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OriginatedYear = x.OriginatedYear,
                TaxpayerId = x.TaxpayerId,
                TotalAmount = x.TotalAmount
            }).ToList();
        }

        public string AddQuery(DemandNoticeArrearsModel dna)
        {
            return $"insert into tbl_demandNoticeArrears(id,billingNo,taxpayerId,totalAmount," +
                $" amountPaid,itemId,originatedYear," +
                $"billingYear,arrearsStatus,createdBy,dateCreated) values(" +
                $"'{Guid.NewGuid()}','{dna.BillingNo}','{dna.TaxpayerId}','{dna.TotalAmount}',0," +
                $"'{dna.ItemId}','{dna.OriginatedYear}','{dna.BillingYear}','{dna.ArrearsStatus}'," +
                $"'{dna.CreatedBy}',getdate());";
        }

        public async Task<bool> AddArrears(DemandNoticeArrearsModel dna)
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

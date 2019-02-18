﻿using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticeItemRepository : AbstractRepository
    {
        public DemandNoticeItemRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(DemandNoticeTaxpayers dntd)
        {
            // get companyItems
            // add demandnoticetaxpayers
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addTaxpayerDemandNoticeItem @p0,@p1,@p2,@p3", new object[] {
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

        public async Task<Response> Add(DemandNoticeItemModel[] items)
        {
            DemandNoticeItem[] dnItems = items.Select(x => new DemandNoticeItem()
            {
                AmountPaid = x.AmountPaid,
                BillingNo = x.BillingNo,
                CreatedBy = x.CreatedBy,
                DateCreated = DateTime.Now,
                Id = x.Id,
                ItemAmount = x.ItemAmount,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                ItemStatus = x.ItemStatus,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                DnTaxpayersDetailsId = x.DnTaxpayersDetailsId,
                TaxpayerId = x.TaxpayerId
            }).ToArray();

            db.AddRange(dnItems);
            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Item has been added successfully"
            };
        }

        public async Task<List<DemandNoticeItemModel>> ByBillingNumber(string billingno)
        {
            //List<DemandNoticeItem> lstdbItem = await db.Set<DemandNoticeItem>().
            //        FromSql($"select tbl_demandNoticeItem.*,0.0 as penaltyAmount,'nil' as duration,-1 " +
            //        $" as billingYr from tbl_demandNoticeItem where billingNo = '{billingno}' " +
            //        $"and itemStatus not in ('CANCEL')").ToListAsync();
            // $"and itemStatus in ('PENDING','PART_PAYMENT','PAID','CANCEL')").ToListAsync();

            var result = await db.Set<DemandNoticeItem>().Where(x => x.ItemStatus == "CANCEL").Select(x => new DemandNoticeItemModel()
            {
                AmountPaid = x.AmountPaid,
                BillingNo = x.BillingNo,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DnTaxpayersDetailsId = x.DnTaxpayersDetailsId,
                Id = x.Id,
                ItemAmount = x.ItemAmount,
                ItemStatus = x.ItemStatus,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                TaxpayerId = x.TaxpayerId
            }).ToListAsync();

            return result;
        }

        public async Task<List<DemandNoticeItemModel>> UnpaidBillsByTaxpayerId(Guid taxpayerId, string billNumber, int billingYr)
        {
            var r = await db.Set<DemandNoticeItem>()
                 .Join(db.Set<DemandNoticeTaxpayers>(),
                 dnt => dnt.TaxpayerId, dni => dni.TaxpayerId,
                 (dni, dnt) => new DemandNoticeItemModel()
                 {
                     AmountPaid = dni.AmountPaid,
                     BillingNo = dni.BillingNo,
                     CreatedBy = dni.CreatedBy,
                     DateCreated = dni.DateCreated,
                     DnTaxpayersDetailsId = dni.DnTaxpayersDetailsId,
                     Id = dni.Id,
                     ItemAmount = dni.ItemAmount,
                     ItemStatus = dni.ItemStatus,
                     ItemId = dni.ItemId,
                     ItemName = dni.ItemName,
                     Lastmodifiedby = dni.Lastmodifiedby,
                     LastModifiedDate = dni.LastModifiedDate,
                     TaxpayerId = dni.TaxpayerId
                 }).Where(x => x.TaxpayerId == taxpayerId && x.BillingNo == billNumber).ToListAsync();

            return r;
        }

        public async Task<List<DemandNoticeItemModel>> ReportByCategory(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            DateTime endDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            string[] status = { "PENDING", "PART_PAYMENT", "PAID" };
            List<DemandNoticeItemModel> lst = await db.Set<DemandNoticeItem>()
              .Include(x => x.TaxPayer)
              .ThenInclude(x => x.Company)
              .ThenInclude(x => x.TaxPayerCatgeory)
              .Include(x => x.DemandNotice)
              .ThenInclude(x => x.Ward)
              .Where(x => x.DateCreated >= startDate && x.DateCreated <= endDate && status.Any(t => t == x.ItemStatus))
              .Select(p => new DemandNoticeItemModel()
              {
                  AmountPaid = p.AmountPaid,
                  BillingNo = p.BillingNo,
                  CreatedBy = p.CreatedBy,
                  DateCreated = p.DateCreated,
                  DnTaxpayersDetailsId = p.DnTaxpayersDetailsId,
                  Id = p.Id,
                  ItemAmount = p.ItemAmount,
                  ItemId = p.ItemId,
                  ItemName = p.ItemName,
                  ItemStatus = p.ItemStatus,
                  Lastmodifiedby = p.Lastmodifiedby,
                  LastModifiedDate = p.LastModifiedDate,
                  TaxpayerId = p.TaxpayerId,
                  wardName = p.DemandNotice.Ward.WardName,
                  category = p.TaxPayer.Company.TaxPayerCatgeory.TaxpayerCategoryName
              }).ToListAsync();

            return lst;
        }
    }
}

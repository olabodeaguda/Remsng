using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticeArrearRepository : IDemandNoticeArrearRepository
    {
        private readonly DbContext db;
        public DemandNoticeArrearRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<List<DemandNoticeArrearsModel>> ByBillingNumber(long billingno)
        {
            string[] statuss = { "PENDING", "PART_PAYMENT" };

            List<DemandNoticeArrear> lst = await db.Set<DemandNoticeArrear>()
                .Where(x => x.BillingNo == billingno && statuss.Any(p => p == x.ArrearsStatus)).ToListAsync();

            return lst.Select(x => new DemandNoticeArrearsModel()
            {
                AmountPaid = x.AmountPaid,
                ArrearsStatus = x.ArrearsStatus,
                BillingNo = x.BillingNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                //ItemId = x.ItemId,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OriginatedYear = x.OriginatedYear,
                TaxpayerId = x.TaxpayerId,
                TotalAmount = x.TotalAmount
            }).ToList();
        }

        public async Task<List<DemandNoticeArrearsModel>> ByBillingNumber(Guid taxpayerId)
        {
            string[] statuss = { "PENDING", "PART_PAYMENT" };

            List<DemandNoticeArrear> lst = await db.Set<DemandNoticeArrear>()
                .Where(x => x.TaxpayerId == taxpayerId && statuss.Any(p => p == x.ArrearsStatus)).ToListAsync();

            return lst.Select(x => new DemandNoticeArrearsModel()
            {
                AmountPaid = x.AmountPaid,
                ArrearsStatus = x.ArrearsStatus,
                BillingNo = x.BillingNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                //ItemId = x.ItemId,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OriginatedYear = x.OriginatedYear,
                TaxpayerId = x.TaxpayerId,
                TotalAmount = x.TotalAmount
            }).ToList();
        }

        public async Task<List<DemandNoticeArrearsModel>> ByTaxpayer(Guid taxpayerId)
        {
            string[] statuss = { "PENDING", "PART_PAYMENT" };
            List<DemandNoticeArrear> lst = await db.Set<DemandNoticeArrear>()
                .Where(x => x.TaxpayerId == taxpayerId && statuss.Any(p => p == x.ArrearsStatus)).ToListAsync();

            return lst.Select(x => new DemandNoticeArrearsModel()
            {
                AmountPaid = x.AmountPaid,
                ArrearsStatus = x.ArrearsStatus,
                BillingNo = x.BillingNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                //ItemId = x.ItemId,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OriginatedYear = x.OriginatedYear,
                TaxpayerId = x.TaxpayerId,
                TotalAmount = x.TotalAmount
            }).ToList();
        }

        public async Task<DemandNoticeArrearsModel[]> ByTaxpayer(Guid[] taxpayerIds)
        {
            string[] statuss = { "PENDING", "PART_PAYMENT" };
            DemandNoticeArrearsModel[] lst = await db.Set<DemandNoticeArrear>()
                .Where(x => taxpayerIds.Any(p => p == x.TaxpayerId) && statuss.Any(p => p == x.ArrearsStatus))
                .Select(x => new DemandNoticeArrearsModel()
                {
                    AmountPaid = x.AmountPaid,
                    ArrearsStatus = x.ArrearsStatus,
                    BillingNo = x.BillingNo,
                    BillingYear = x.BillingYear,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    Id = x.Id,
                    //ItemId = x.ItemId,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    OriginatedYear = x.OriginatedYear,
                    TaxpayerId = x.TaxpayerId,
                    TotalAmount = x.TotalAmount
                }).ToArrayAsync();
            return lst;
        }

        public async Task<bool> AddArrears(DemandNoticeArrearsModel x)
        {
            if (x == null)
            {
                return false;
            }

            DemandNoticeArrear entities = new DemandNoticeArrear
            {
                AmountPaid = x.AmountPaid,
                ArrearsStatus = x.ArrearsStatus,
                BillingNo = x.BillingNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                CurrentAmount = x.CurrentAmount,
                DateCreated = x.DateCreated,
                Id = x.Id,
                OriginatedYear = x.OriginatedYear,
                TaxpayerId = x.TaxpayerId,
                TotalAmount = x.TotalAmount,
                ItemId = x.ItemId
            };
            db.Set<DemandNoticeArrear>().Add(entities);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<DemandNoticeArrearsModel>> ReportByCategory(Guid[] taxpayerIds)
        {
            string[] status = { "PAID", "PENDING", "PART_PAYMENT" };

            var result = await db.Set<DemandNoticeArrear>()
                .Include(x => x.TaxPayer)
                .Include(x => x.TaxPayer.Company)
                .ThenInclude(x => x.TaxPayerCatgeory)
                .Include(s => s.TaxPayer.Street)
                .ThenInclude(w => w.Ward)
                .Where(x => taxpayerIds.Any(s => s == x.TaxpayerId) && status.Any(p => p == x.ArrearsStatus))
                .Select(e => new DemandNoticeArrearsModel()
                {
                    AmountPaid = e.AmountPaid,
                    ArrearsStatus = e.ArrearsStatus,
                    BillingNo = e.BillingNo,
                    CreatedBy = e.CreatedBy,
                    Category = e.TaxPayer.Company.TaxPayerCatgeory.TaxpayerCategoryName,
                    DateCreated = e.DateCreated,
                    Id = e.Id,
                    //ItemId = e.ItemId,
                    Lastmodifiedby = e.Lastmodifiedby,
                    LastModifiedDate = e.LastModifiedDate,
                    OriginatedYear = e.OriginatedYear,
                    TaxpayerId = e.TaxpayerId,
                    TotalAmount = e.TotalAmount,
                    WardName = e.TaxPayer.Street.Ward.WardName
                })
                .ToListAsync();

            return result;
        }


        public async Task<List<DemandNoticeArrearsModel>> ReportByCategory(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            DateTime endDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            var result = await db.Set<DemandNoticeArrear>()
                .Include(x => x.TaxPayer)
                .Include(x => x.TaxPayer.Company)
                .ThenInclude(x => x.TaxPayerCatgeory)
                .Include(s => s.TaxPayer.Street)
                .ThenInclude(w => w.Ward)
                .Where(x => x.DateCreated >= startDate && x.DateCreated <= endDate)
                .Select(e => new DemandNoticeArrearsModel()
                {
                    AmountPaid = e.AmountPaid,
                    ArrearsStatus = e.ArrearsStatus,
                    BillingNo = e.BillingNo,
                    CreatedBy = e.CreatedBy,
                    Category = e.TaxPayer.Company.TaxPayerCatgeory.TaxpayerCategoryName,
                    DateCreated = e.DateCreated,
                    Id = e.Id,
                    //ItemId = e.ItemId,
                    Lastmodifiedby = e.Lastmodifiedby,
                    LastModifiedDate = e.LastModifiedDate,
                    OriginatedYear = e.OriginatedYear,
                    TaxpayerId = e.TaxpayerId,
                    TotalAmount = e.TotalAmount,
                    WardName = e.TaxPayer.Street.Ward.WardName
                })
                .ToListAsync();

            return result;
        }

        public async Task<bool> AddArrears(DemandNoticeArrearsModel[] models)
        {
            if (models.Length <= 0)
            {
                return false;
            }
            DemandNoticeArrear[] entities = models.Select(x => new DemandNoticeArrear
            {
                AmountPaid = x.AmountPaid,
                ArrearsStatus = x.ArrearsStatus,
                BillingNo = x.BillingNo,
                BillingYear = x.BillingYear,
                CreatedBy = x.CreatedBy,
                CurrentAmount = x.CurrentAmount,
                DateCreated = x.DateCreated,
                Id = x.Id,
                OriginatedYear = x.OriginatedYear,
                TaxpayerId = x.TaxpayerId,
                TotalAmount = x.TotalAmount,
                ItemId = x.ItemId
            }).ToArray();
            db.Set<DemandNoticeArrear>().AddRange(entities);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateArrearsStatus(DemandNoticeArrearsModel[] models, string status)
        {
            if (models.Length <= 0)
            {
                return false;
            }
            foreach (var x in models)
            {
                DemandNoticeArrear dd = await db.Set<DemandNoticeArrear>().FindAsync(x.Id);
                if (dd != null)
                {
                    dd.ArrearsStatus = status;
                }
            }

            await db.SaveChangesAsync();

            return true;
        }
    }
}

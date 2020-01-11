using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticePenaltyRepository : IDemandNoticePenaltyRepository
    {
        private readonly DbContext db;

        public DemandNoticePenaltyRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<DemandNoticePenaltyModel> CreatePenalty(DemandNoticePenaltyModel dnp)
        {
            DemandNoticePenalty demandNoticePenalty = new DemandNoticePenalty()
            {
                AmountPaid = dnp.AmountPaid,
                BillingNo = dnp.BillingNo,
                BillingYear = dnp.BillingYear,
                CreatedBy = dnp.CreatedBy,
                DateCreated = dnp.DateCreated,
                Id = Guid.NewGuid(),
                ItemPenaltyStatus = dnp.ItemPenaltyStatus,
                Lastmodifiedby = dnp.Lastmodifiedby,
                OriginatedYear = dnp.OriginatedYear,
                TaxpayerId = dnp.TaxpayerId,
                TotalAmount = dnp.TotalAmount
            };
            db.Set<DemandNoticePenalty>().Add(demandNoticePenalty);
            await db.SaveChangesAsync();
            dnp.Id = demandNoticePenalty.Id;
            return dnp;
        }


        public async Task<bool> CreatePenalty(DemandNoticePenaltyModel[] models)
        {
            DemandNoticePenalty[] demandNoticePenalty = models.Select(dnp => new DemandNoticePenalty()
            {
                AmountPaid = dnp.AmountPaid,
                BillingNo = dnp.BillingNo,
                BillingYear = dnp.BillingYear,
                CreatedBy = dnp.CreatedBy,
                DateCreated = dnp.DateCreated,
                Id = Guid.NewGuid(),
                ItemPenaltyStatus = dnp.ItemPenaltyStatus,
                Lastmodifiedby = dnp.Lastmodifiedby,
                OriginatedYear = dnp.OriginatedYear,
                TaxpayerId = dnp.TaxpayerId,
                TotalAmount = dnp.TotalAmount,
                CurrentAmount = dnp.CurrentAmount
            }).ToArray();
            db.Set<DemandNoticePenalty>().AddRange(demandNoticePenalty);
            await db.SaveChangesAsync();

            return true;
        }

        public string AddQuery(DemandNoticePenaltyModel dnp)
        {
            return $"INSERT INTO tbl_demandNoticePenalty" +
                $" (id ,billingNo,taxpayerId ,totalAmount,amountPaid" +
                $" ,originatedYear,billingYear,itemPenaltyStatus,createdBy,dateCreated)" +
                $" VALUES ('{Guid.NewGuid()}','{dnp.BillingNo}','{dnp.TaxpayerId}','{dnp.TotalAmount}','{dnp.AmountPaid}'" +
                $",'{dnp.OriginatedYear}','{dnp.BillingYear}','{dnp.ItemPenaltyStatus}','APPLICATION','{DateTime.Now}');";
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

        public async Task<List<DemandNoticePenaltyModel>> ByTaxpayerId(Guid taxpayerId)
        {
            //string query = $"select tbl_demandNoticePenalty.*,0 as billingYr from tbl_demandNoticePenalty where taxpayerId = '{taxpayerId}'";
            //List<DemandNoticeItemPenaltyModelExt> lstdbItem = await db.Set<DemandNoticeItemPenaltyModelExt>()
            //    .FromSql(query).ToListAsync();

            var result = await db.Set<DemandNoticePenalty>()
                .Join(db.Set<TaxPayer>()
                .Include(x => x.Company)
                .ThenInclude(x => x.TaxPayerCatgeory), dnp => dnp.TaxpayerId, tp => tp.Id, (dnp, tp) => new { dnp, tp })
                .Join(db.Set<Street>().Include(x => x.Ward), res => res.tp.StreetId, str => str.Id,
                (res, str) => new DemandNoticePenaltyModel
                {
                    AmountPaid = res.dnp.AmountPaid,
                    BillingNo = res.dnp.BillingNo,
                    BillingYear = res.dnp.BillingYear,
                    CreatedBy = res.dnp.CreatedBy,
                    DateCreated = res.dnp.DateCreated,
                    Id = res.dnp.Id,
                    ItemPenaltyStatus = res.dnp.ItemPenaltyStatus,
                    Lastmodifiedby = res.dnp.Lastmodifiedby,
                    LastModifiedDate = res.dnp.LastModifiedDate,
                    OriginatedYear = res.dnp.OriginatedYear,
                    TaxpayerId = res.dnp.TaxpayerId,
                    TotalAmount = res.dnp.TotalAmount,
                    wardName = str.Ward.WardName,
                    category = res.tp.Company.TaxPayerCatgeory.TaxpayerCategoryName
                })
                .Where(x => x.TaxpayerId == taxpayerId).ToListAsync();

            return result;
        }

        public async Task<List<DemandNoticePenaltyModel>> ByTaxpayerId(Guid taxpayerId, int billingYr)
        {
            var result = await db.Set<DemandNoticePenalty>()
               .Join(db.Set<TaxPayer>()
               .Include(x => x.Company)
               .ThenInclude(x => x.TaxPayerCatgeory), dnp => dnp.TaxpayerId, tp => tp.Id, (dnp, tp) => new { dnp, tp })
               .Join(db.Set<Street>().Include(x => x.Ward), res => res.tp.StreetId, str => str.Id,
               (res, str) => new DemandNoticePenaltyModel
               {
                   AmountPaid = res.dnp.AmountPaid,
                   BillingNo = res.dnp.BillingNo,
                   BillingYear = res.dnp.BillingYear,
                   CreatedBy = res.dnp.CreatedBy,
                   DateCreated = res.dnp.DateCreated,
                   Id = res.dnp.Id,
                   ItemPenaltyStatus = res.dnp.ItemPenaltyStatus,
                   Lastmodifiedby = res.dnp.Lastmodifiedby,
                   LastModifiedDate = res.dnp.LastModifiedDate,
                   OriginatedYear = res.dnp.OriginatedYear,
                   TaxpayerId = res.dnp.TaxpayerId,
                   TotalAmount = res.dnp.TotalAmount,
                   wardName = str.Ward.WardName,
                   category = res.tp.Company.TaxPayerCatgeory.TaxpayerCategoryName
               })
               .Where(x => x.TaxpayerId == taxpayerId && x.BillingYear == billingYr).ToListAsync();
            return result;
        }

        public async Task<List<DemandNoticePenaltyModel>> ReportByCategory(Guid[] taxpayerIds)
        {
            string[] status = { "PENDING", "PART_PAYMENT", "PAID" };

            var result = await db.Set<DemandNoticePenalty>()
                .Join(db.Set<TaxPayer>()
                .Include(x => x.Company)
                .ThenInclude(x => x.TaxPayerCatgeory), dnp => dnp.TaxpayerId, tp => tp.Id, (dnp, tp) => new { dnp, tp })
                .Join(db.Set<Street>().Include(x => x.Ward), res => res.tp.StreetId, str => str.Id,
                (res, str) => new DemandNoticePenaltyModel
                {
                    AmountPaid = res.dnp.AmountPaid,
                    BillingNo = res.dnp.BillingNo,
                    BillingYear = res.dnp.BillingYear,
                    CreatedBy = res.dnp.CreatedBy,
                    DateCreated = res.dnp.DateCreated,
                    Id = res.dnp.Id,
                    ItemPenaltyStatus = res.dnp.ItemPenaltyStatus,
                    Lastmodifiedby = res.dnp.Lastmodifiedby,
                    LastModifiedDate = res.dnp.LastModifiedDate,
                    OriginatedYear = res.dnp.OriginatedYear,
                    TaxpayerId = res.dnp.TaxpayerId,
                    TotalAmount = res.dnp.TotalAmount,
                    wardName = str.Ward.WardName,
                    category = res.tp.Company.TaxPayerCatgeory.TaxpayerCategoryName
                })
                .Where(x => taxpayerIds.Any(s => s == x.TaxpayerId) && status.Any(p => p == x.ItemPenaltyStatus)).ToListAsync();


            //List<DemandNoticeItemPenaltyModelExt> lst =
            //    await db.Set<DemandNoticeItemPenaltyModelExt>().FromSql("sp_getPenaltyByCategoryDate @p0,@p1",
            //    new object[] { startDate, endDate }).ToListAsync();

            return result;
        }


        public async Task<List<DemandNoticePenaltyModelExt>> ReportByCategoryExt(Guid[] taxpayerIds)
        {
            string[] status = { "PENDING", "PART_PAYMENT", "PAID" };

            var result = await db.Set<DemandNoticePenalty>()
                .Join(db.Set<TaxPayer>()
                .Include(s => s.Address)
                .Include(d => d.Street)
                .Include(x => x.Company)
                .ThenInclude(x => x.TaxPayerCatgeory), dnp => dnp.TaxpayerId, tp => tp.Id, (dnp, tp) => new { dnp, tp })
                .Join(db.Set<Street>().Include(x => x.Ward), res => res.tp.StreetId, str => str.Id,
                (res, str) => new DemandNoticePenaltyModelExt
                {
                    AmountPaid = res.dnp.AmountPaid,
                    BillingNo = res.dnp.BillingNo,
                    BillingYear = res.dnp.BillingYear,
                    CreatedBy = res.dnp.CreatedBy,
                    DateCreated = res.dnp.DateCreated,
                    Id = res.dnp.Id,
                    ItemPenaltyStatus = res.dnp.ItemPenaltyStatus,
                    Lastmodifiedby = res.dnp.Lastmodifiedby,
                    LastModifiedDate = res.dnp.LastModifiedDate,
                    OriginatedYear = res.dnp.OriginatedYear,
                    TaxpayerId = res.dnp.TaxpayerId,
                    TotalAmount = res.dnp.TotalAmount,
                    wardName = str.Ward.WardName,
                    category = res.tp.Company.TaxPayerCatgeory.TaxpayerCategoryName,
                    Address = $"{res.tp.Address.Addressnumber} {res.tp.Street.StreetName}",
                    TaxpayerName = $"{res.tp.Surname} {res.tp.Firstname} {res.tp.Lastname}"
                })
                .Where(x => taxpayerIds.Any(s => s == x.TaxpayerId) && status.Any(p => p == x.ItemPenaltyStatus)).ToListAsync();


            //List<DemandNoticeItemPenaltyModelExt> lst =
            //    await db.Set<DemandNoticeItemPenaltyModelExt>().FromSql("sp_getPenaltyByCategoryDate @p0,@p1",
            //    new object[] { startDate, endDate }).ToListAsync();

            return result;
        }


        public async Task<List<DemandNoticePenaltyModel>> ReportByCategory(DateTime fromDate, DateTime toDate)
        {
            DateTime startDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            DateTime endDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

            string[] status = { "PENDING", "PART_PAYMENT", "PAID" };

            var result = await db.Set<DemandNoticePenalty>()
                .Join(db.Set<TaxPayer>()
                .Include(x => x.Company)
                .ThenInclude(x => x.TaxPayerCatgeory), dnp => dnp.TaxpayerId, tp => tp.Id, (dnp, tp) => new { dnp, tp })
                .Join(db.Set<Street>().Include(x => x.Ward), res => res.tp.StreetId, str => str.Id,
                (res, str) => new DemandNoticePenaltyModel
                {
                    AmountPaid = res.dnp.AmountPaid,
                    BillingNo = res.dnp.BillingNo,
                    BillingYear = res.dnp.BillingYear,
                    CreatedBy = res.dnp.CreatedBy,
                    DateCreated = res.dnp.DateCreated,
                    Id = res.dnp.Id,
                    ItemPenaltyStatus = res.dnp.ItemPenaltyStatus,
                    Lastmodifiedby = res.dnp.Lastmodifiedby,
                    LastModifiedDate = res.dnp.LastModifiedDate,
                    OriginatedYear = res.dnp.OriginatedYear,
                    TaxpayerId = res.dnp.TaxpayerId,
                    TotalAmount = res.dnp.TotalAmount,
                    wardName = str.Ward.WardName,
                    category = res.tp.Company.TaxPayerCatgeory.TaxpayerCategoryName
                })
                .Where(x => x.DateCreated >= startDate && x.DateCreated <= endDate && status.Any(p => p == x.ItemPenaltyStatus)).ToListAsync();


            //List<DemandNoticeItemPenaltyModelExt> lst =
            //    await db.Set<DemandNoticeItemPenaltyModelExt>().FromSql("sp_getPenaltyByCategoryDate @p0,@p1",
            //    new object[] { startDate, endDate }).ToListAsync();

            return result;
        }

        public async Task<DemandNoticePenaltyModel[]> ByTaxpayerId(Guid[] taxpayerIds)
        {
            string[] statuss = { "PENDING", "PART_PAYMENT" };
            var result = await db.Set<DemandNoticePenalty>()
               .Join(db.Set<TaxPayer>()
               .Include(x => x.Company)
               .ThenInclude(x => x.TaxPayerCatgeory), dnp => dnp.TaxpayerId, tp => tp.Id, (dnp, tp) => new { dnp, tp })
               .Join(db.Set<Street>().Include(x => x.Ward), res => res.tp.StreetId, str => str.Id,
               (res, str) => new DemandNoticePenaltyModel
               {
                   AmountPaid = res.dnp.AmountPaid,
                   BillingNo = res.dnp.BillingNo,
                   BillingYear = res.dnp.BillingYear,
                   CreatedBy = res.dnp.CreatedBy,
                   DateCreated = res.dnp.DateCreated,
                   Id = res.dnp.Id,
                   ItemPenaltyStatus = res.dnp.ItemPenaltyStatus,
                   Lastmodifiedby = res.dnp.Lastmodifiedby,
                   LastModifiedDate = res.dnp.LastModifiedDate,
                   OriginatedYear = res.dnp.OriginatedYear,
                   TaxpayerId = res.dnp.TaxpayerId,
                   TotalAmount = res.dnp.TotalAmount,
                   wardName = str.Ward.WardName,
                   category = res.tp.Company.TaxPayerCatgeory.TaxpayerCategoryName
               })
               .Where(x => taxpayerIds.Any(t => t == x.TaxpayerId) && statuss.Any(r => r == x.ItemPenaltyStatus)).ToArrayAsync();
            return result;
        }
        public async Task<bool> UpdatePenaltyStatus(DemandNoticePenaltyModel[] models, string status)
        {
            if (models.Length <= 0)
            {
                return false;
            }
            foreach (var dnp in models)
            {
                DemandNoticePenalty dd = await db.Set<DemandNoticePenalty>().FindAsync(dnp.Id);
                if (dd != null)
                {
                    dd.ItemPenaltyStatus = status;
                }
            }

            await db.SaveChangesAsync();

            return true;
        }
    }
}


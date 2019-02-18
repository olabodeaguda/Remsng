using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticeArrearRepository : AbstractRepository
    {
        public DemandNoticeArrearRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<List<DemandNoticeArrearsModel>> ByBillingNumber(string billingno)
        {
            //List<DemandNoticeArrears> lst =
            //await db.Set<DemandNoticeArrears>()
            //    .Where(x => x.BillingNo == billingno && x.ArrearsStatus.)
            //    .FromSql($"select tbl_demandNoticeArrears.*, 0 as billingYr from tbl_demandNoticeArrears " +
            //    $"where billingNo = '{billingno}'  and arrearsStatus in ('PENDING','PART_PAYMENT')").ToListAsync();
            //List<DemandNoticeArrears> lst = new List<DemandNoticeArrears>();
            //foreach (var tm in lstdbItem)
            //{
            //    var t = lst.FirstOrDefault(x => x.BillingNo == tm.BillingNo && x.TotalAmount == tm.TotalAmount);
            //    if (t == null)
            //    {
            //        lst.Add(tm);
            //    }
            //}
            string[] statuss = { "PENDING", "PART_PAYMENT" };

            List<DemandNoticeArrears> lst = await db.Set<DemandNoticeArrears>()
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
            //string query = $"select tbl_demandNoticeArrears.*, 0 as billingYr " +
            //    $"from tbl_demandNoticeArrears where taxpayerId = '{taxpayerId}' and arrearsStatus in ('PENDING','PART_PAYMENT')";
            //var entity = await db.Set<DemandNoticeArrears>().FromSql(query).ToListAsync();

            string[] statuss = { "PENDING", "PART_PAYMENT" };
            List<DemandNoticeArrears> lst = await db.Set<DemandNoticeArrears>()
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

            var result = await db.Set<DemandNoticeArrears>()
                .Include(x => x.TaxPayer)
                .Include(x => x.TaxPayer.Company)
                .ThenInclude(x => x.TaxPayerCatgeory)
                .Include(s => s.TaxPayer.Street)
                .ThenInclude(w => w.Ward)
                .Where(x => x.DateCreated >= startDate && x.DateCreated <= endDate)
                .Select(e => new DemandNoticeArrearsModelExt()
                {
                    amountPaid = e.AmountPaid,
                    arrearsStatus = e.ArrearsStatus,
                    billingNo = e.BillingNo,
                    createdBy = e.CreatedBy,
                    category = e.TaxPayer.Company.TaxPayerCatgeory.TaxpayerCategoryName,
                    dateCreated = e.DateCreated,
                    id = e.Id,
                    itemId = e.ItemId,
                    lastmodifiedby = e.Lastmodifiedby,
                    lastModifiedDate = e.LastModifiedDate,
                    originatedYear = e.OriginatedYear,
                    taxpayerId = e.TaxpayerId,
                    totalAmount = e.TotalAmount,
                    wardName = e.TaxPayer.Street.Ward.WardName
                })
                .ToListAsync();

            //List<DemandNoticeArrearsModelExt> lst =
            //    await db.Set<DemandNoticeArrearsModelExt>().FromSql("sp_getArrearsByCategoryDate @p0,@p1",
            //    new object[] { startDate, endDate }).ToListAsync();



            return result;
        }

    }
}

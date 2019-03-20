using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticePaymentHistoryRepository : AbstractRepository
    {
        public DemandNoticePaymentHistoryRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> AddAsync(DemandNoticePaymentHistoryModel dnph)
        {
            DemandNoticePaymentHistory demandNoticePaymentHistory = new DemandNoticePaymentHistory
            {
                Amount = dnph.Amount,
                BankId = dnph.BankId,
                BillingNumber = dnph.BillingNumber,
                Charges = dnph.Charges,
                CreatedBy = dnph.CreatedBy,
                DateCreated = dnph.DateCreated,
                Id = Guid.NewGuid(),
                IsWaiver = dnph.IsWaiver,
                Lastmodifiedby = dnph.Lastmodifiedby,
                LastModifiedDate = dnph.LastModifiedDate,
                OwnerId = dnph.OwnerId,
                PaymentMode = dnph.PaymentMode,
                PaymentStatus = "PENDING",
                ReferenceNumber = dnph.ReferenceNumber,
                SyncStatus = false,
                OtherNames = dnph.OtherNames
            };

            db.Set<DemandNoticePaymentHistory>().Add(demandNoticePaymentHistory);
            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Payment has been recoreded sucessfully"
            };
        }

        public async Task<Response> UpdateAsync(DemandNoticePaymentHistoryModel dnph)
        {
            var model = await db.Set<DemandNoticePaymentHistory>().FindAsync(dnph.Id);
            if (model == null)
            {
                throw new NotFoundException("Payment history does not exist");
            }
            model.BankId = dnph.BankId;
            model.ReferenceNumber = dnph.ReferenceNumber;
            model.Amount = dnph.Amount;
            model.Charges = dnph.Charges;
            model.PaymentMode = dnph.PaymentMode;
            model.Lastmodifiedby = dnph.Lastmodifiedby;
            model.LastModifiedDate = DateTime.Now;

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = "Payment history has been updated successfully"
            };
        }

        public async Task<Response> UpdateStatusAsync(DemandNoticePaymentHistoryModel dnph)
        {
            var model = await db.Set<DemandNoticePaymentHistory>().FindAsync(dnph.Id);
            if (model == null)
            {
                throw new NotFoundException("Payment history does not exist");
            }
            model.PaymentStatus = dnph.PaymentStatus;

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = "Payment history has been updated successfully"
            };
        }

        public async Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumber(long billingnumber)
        {
            var model = await db.Set<DemandNoticePaymentHistory>().Include(x => x.Bank)
                .Join(db.Set<DemandNoticeTaxpayer>(), dnph => dnph.BillingNumber,
                dnt => dnt.BillingNumber, (dnph, dnt) => new DemandNoticePaymentHistoryModel()
                {
                    Amount = dnph.Amount,
                    BankId = dnph.BankId,
                    BillingNumber = dnph.BillingNumber,
                    BankName = dnph.Bank.BankName,
                    Charges = dnph.Charges,
                    CreatedBy = dnph.CreatedBy,
                    DateCreated = dnph.DateCreated,
                    Id = dnph.Id,
                    IsWaiver = dnph.IsWaiver,
                    Lastmodifiedby = dnph.Lastmodifiedby,
                    LastModifiedDate = dnph.LastModifiedDate,
                    OwnerId = dnph.OwnerId,
                    PaymentMode = dnph.PaymentMode,
                    PaymentStatus = dnph.PaymentStatus,
                    ReferenceNumber = dnph.ReferenceNumber,
                    SyncStatus = dnph.SyncStatus,
                    TotalBillAmount = dnph.Amount + dnph.Charges,
                    BillingYear = dnt.BillingYr,
                    TaxPayerName = string.IsNullOrEmpty(dnph.OtherNames) ? dnt.TaxpayersName : dnph.OtherNames
                }).Where(x => x.BillingNumber == billingnumber).ToListAsync();

            return model;// await db.Set<DemandNoticePaymentHistoryModelExt>().FromSql(query).ToListAsync();
        }

        public async Task<List<DemandNoticePaymentHistoryModel>> ByBillingNumbers(long[] billingnumber)
        {
            var model = await db.Set<DemandNoticePaymentHistory>().Include(x => x.Bank)
               .Join(db.Set<DemandNoticeTaxpayer>(), dnph => dnph.BillingNumber,
               dnt => dnt.BillingNumber, (dnph, dnt) => new DemandNoticePaymentHistoryModel()
               {
                   Amount = dnph.Amount,
                   BankId = dnph.BankId,
                   BillingNumber = dnph.BillingNumber,
                   BankName = dnph.Bank.BankName,
                   Charges = dnph.Charges,
                   CreatedBy = dnph.CreatedBy,
                   DateCreated = dnph.DateCreated,
                   Id = dnph.Id,
                   IsWaiver = dnph.IsWaiver,
                   Lastmodifiedby = dnph.Lastmodifiedby,
                   LastModifiedDate = dnph.LastModifiedDate,
                   OwnerId = dnph.OwnerId,
                   PaymentMode = dnph.PaymentMode,
                   PaymentStatus = dnph.PaymentStatus,
                   ReferenceNumber = dnph.ReferenceNumber,
                   SyncStatus = dnph.SyncStatus,
                   TotalBillAmount = dnph.Amount + dnph.Charges,
                   BillingYear = dnt.BillingYr,
                   TaxPayerName = string.IsNullOrEmpty(dnph.OtherNames) ? dnt.TaxpayersName : dnph.OtherNames
               }).Where(x => billingnumber.Any(r => r == x.BillingNumber) && x.PaymentStatus == "APPROVED").ToListAsync();

            return model;
        }

        public async Task<DemandNoticePaymentHistoryModel> ById(Guid id)
        {
            var model = await db.Set<DemandNoticePaymentHistory>().Include(x => x.Bank)
                .Join(db.Set<DemandNoticeTaxpayer>(), dnph => dnph.BillingNumber,
                dnt => dnt.BillingNumber, (dnph, dnt) => new DemandNoticePaymentHistoryModel()
                {
                    Amount = dnph.Amount,
                    BankId = dnph.BankId,
                    BillingNumber = dnph.BillingNumber,
                    BankName = dnph.Bank.BankName,
                    Charges = dnph.Charges,
                    CreatedBy = dnph.CreatedBy,
                    DateCreated = dnph.DateCreated,
                    Id = dnph.Id,
                    IsWaiver = dnph.IsWaiver,
                    Lastmodifiedby = dnph.Lastmodifiedby,
                    LastModifiedDate = dnph.LastModifiedDate,
                    OwnerId = dnph.OwnerId,
                    PaymentMode = dnph.PaymentMode,
                    PaymentStatus = dnph.PaymentStatus,
                    ReferenceNumber = dnph.ReferenceNumber,
                    SyncStatus = dnph.SyncStatus,
                    TotalBillAmount = dnph.Amount + dnph.Charges,
                    BillingYear = dnt.BillingYr,
                    TaxPayerName = string.IsNullOrEmpty(dnph.OtherNames) ? dnt.TaxpayersName : dnph.OtherNames
                }).Where(x => x.Id == id).FirstOrDefaultAsync();

            return model;
        }

        public async Task<DemandNoticePaymentHistoryModel> ByIdExtended(Guid id)
        {
            var x = await db.Set<DemandNoticePaymentHistory>().Include(d => d.Bank).FirstOrDefaultAsync(t => t.Id == id);

            if (x == null)
            {
                return null;
            }
            return new DemandNoticePaymentHistoryModel()
            {
                Amount = x.Amount,
                BankId = x.BankId,
                BillingNumber = x.BillingNumber,
                Charges = x.Charges,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                IsWaiver = x.IsWaiver,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OwnerId = x.OwnerId,
                PaymentMode = x.PaymentMode,
                PaymentStatus = x.PaymentStatus,
                ReferenceNumber = x.ReferenceNumber,
                SyncStatus = x.SyncStatus,
                BankName = x.Bank.BankName,
                OtherNames = x.OtherNames
            };


        }

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var query = db.Set<DemandNoticePaymentHistory>()
                .Join(db.Set<TaxPayer>().Include(x => x.Company),
                dnph => dnph.OwnerId, tp => tp.Id, (dnph, tp) => new { dnph, tp })
                .Where(x => x.tp.Company.LcdaId == lcdaId)
                .Select(x => new DemandNoticePaymentHistoryModel
                {
                    Amount = x.dnph.Amount,
                    BankId = x.dnph.BankId,
                    BillingNumber = x.dnph.BillingNumber,
                    Charges = x.dnph.Charges,
                    CreatedBy = x.dnph.CreatedBy,
                    DateCreated = x.dnph.DateCreated,
                    Id = x.dnph.Id,
                    IsWaiver = x.dnph.IsWaiver,
                    Lastmodifiedby = x.dnph.Lastmodifiedby,
                    LastModifiedDate = x.dnph.LastModifiedDate,
                    OwnerId = x.dnph.OwnerId,
                    PaymentMode = x.dnph.PaymentMode,
                    PaymentStatus = x.dnph.PaymentStatus,
                    ReferenceNumber = x.dnph.ReferenceNumber,
                    SyncStatus = x.dnph.SyncStatus,
                    TaxPayerName = string.IsNullOrEmpty(x.dnph.OtherNames) ? $"{x.tp.Surname} {x.tp.Firstname} {x.tp.Lastname}" : x.dnph.OtherNames
                });

            var result = await query.OrderByDescending(d => d.DateCreated).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToListAsync();
            int totalCount = await query.CountAsync();

            return new
            {
                data = result,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<List<DemandNoticePaymentHistoryModel>> ApprovedPaymentHistory(Guid ownerId, int currentYr)
        {
            //string query = $"select tbl_demandNoticePaymentHistory.*,bank.bankName from tbl_demandNoticePaymentHistory " +
            //    $"inner join tbl_bank bank on bank.id = tbl_demandNoticePaymentHistory.bankId " +
            //    $"inner join tbl_demandNoticeTaxpayers on tbl_demandNoticeTaxpayers.taxpayerId = tbl_demandNoticePaymentHistory.ownerId " +
            //    $"where tbl_demandNoticePaymentHistory.ownerId = '{ownerId}' and tbl_demandNoticeTaxpayers.billingYr = {currentYr} " +
            //    $"and paymentStatus = 'APPROVED'";
            var result = await db.Set<DemandNoticePaymentHistory>().Include(x => x.Bank)
                .Join(db.Set<DemandNoticeTaxpayer>(), dnph => dnph.BillingNumber, dnt => dnt.BillingNumber, (dnph, dnt) => new
                { dnph, dnt }).Where(p => p.dnph.OwnerId == ownerId && p.dnt.BillingYr == currentYr && p.dnph.PaymentStatus == "APPROVED")
                .Select(x => new DemandNoticePaymentHistoryModel()
                {
                    Amount = x.dnph.Amount,
                    BankId = x.dnph.BankId,
                    BillingNumber = x.dnph.BillingNumber,
                    Charges = x.dnph.Charges,
                    CreatedBy = x.dnph.CreatedBy,
                    DateCreated = x.dnph.DateCreated,
                    Id = x.dnph.Id,
                    IsWaiver = x.dnph.IsWaiver,
                    Lastmodifiedby = x.dnph.Lastmodifiedby,
                    LastModifiedDate = x.dnph.LastModifiedDate,
                    OwnerId = x.dnph.OwnerId,
                    PaymentMode = x.dnph.PaymentMode,
                    PaymentStatus = x.dnph.PaymentStatus,
                    ReferenceNumber = x.dnph.ReferenceNumber,
                    SyncStatus = x.dnph.SyncStatus,
                    BankName = x.dnph.Bank.BankName,
                    TaxPayerName = string.IsNullOrEmpty(x.dnph.OtherNames) ? x.dnt.TaxpayersName : x.dnph.OtherNames
                }).ToListAsync();

            return result;
        }
    }
}

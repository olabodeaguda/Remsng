using Microsoft.EntityFrameworkCore;
using Remsng.Data.Entities;
using RemsNG.Common.Exceptions;
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
    public class DemandNoticePaymentHistoryRepository : IDemandNoticePaymentHistoryRepository
    {
        private readonly DbContext db;
        public DemandNoticePaymentHistoryRepository(DbContext _db)
        {
            db = _db;
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

        public async Task<bool> UpdateStatus(Guid id, DemandNoticeStatus status)
        {
            var entity = await db.Set<DemandNoticePaymentHistory>().FindAsync(id);
            if (entity == null)
                return false;

            //if (entity.PaymentStatus == status.ToString())
            //    return true;

            entity.PaymentStatus = status.ToString();
            await db.SaveChangesAsync();
            return true;
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
            var model = await db.Set<DemandNoticePaymentHistory>()
                .Include(x => x.Bank)
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

        public async Task<PrepaymentModel> GetPrepayment(Guid taxpayerId)
        {
            var r = await db.Set<Prepayment>()
                .FirstOrDefaultAsync(x => x.taxpayerId == taxpayerId && x.prepaymentStatus == "ACTIVE");
            if (r == null)
            {
                return null;
            }

            return new PrepaymentModel()
            {
                amount = r.amount,
                datecreated = r.datecreated,
                id = r.id,
                prepaymentStatus = r.prepaymentStatus,
                taxpayerId = r.taxpayerId
            };
        }


        public async Task<PrepaymentModel[]> GetPrepayment(Guid taxpayerId, long billingNo)
        {
            var rr = await db.Set<Prepayment>()
                .Where(x => (x.taxpayerId == taxpayerId && x.prepaymentStatus == "ACTIVE")
                || (x.BillingNo == billingNo && x.prepaymentStatus == "CLOSED")).ToArrayAsync();
            if (rr.Length <= 0)
            {
                return Array.Empty<PrepaymentModel>();
            }

            return rr.Select(r => new PrepaymentModel()
            {
                amount = r.amount,
                datecreated = r.datecreated,
                id = r.id,
                prepaymentStatus = r.prepaymentStatus,
                taxpayerId = r.taxpayerId
            }).ToArray();
        }


        public async Task<PrepaymentModel[]> GetPrepaymentList(Guid taxpayerId)
        {
            var rr = await db.Set<Prepayment>()
                .Where(x => x.taxpayerId == taxpayerId && x.prepaymentStatus == "ACTIVE").ToListAsync();
            if (rr.Count <= 0)
            {
                return Array.Empty<PrepaymentModel>();
            }

            return rr.Select(r => new PrepaymentModel()
            {
                amount = r.amount,
                datecreated = r.datecreated,
                id = r.id,
                prepaymentStatus = r.prepaymentStatus,
                taxpayerId = r.taxpayerId
            }).ToArray();
        }

        public async Task<PrepaymentModel> AddPrepaymentForAlreadyRegisterdAmount(PrepaymentModel prepayment)
        {
            //var prep = await db.Set<Prepayment>()
            //    .FirstOrDefaultAsync(x => x.taxpayerId == prepayment.taxpayerId
            //    && prepayment.amount == x.amount && x.prepaymentStatus != "CLOSED");

            //if (prep == null)
            //{
            Prepayment pm = new Prepayment()
            {
                taxpayerId = prepayment.taxpayerId,
                amount = prepayment.amount,
                datecreated = prepayment.datecreated,
                id = prepayment.id,
                prepaymentStatus = prepayment.prepaymentStatus
            };
            pm.datecreated = DateTime.Now;
            db.Set<Prepayment>().Add(pm);
            await db.SaveChangesAsync();
            prepayment.id = pm.id;
            return prepayment;
            //}
            //return prepayment;
        }

        public async Task<bool> UpdatePrepaymentStatus(long id, string prepaymentStatus)
        {
            var entity = await db.Set<Prepayment>().FindAsync(id);
            if (entity == null)
                return false;

            if (entity.prepaymentStatus == prepaymentStatus)
                return true;

            entity.prepaymentStatus = prepaymentStatus;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePrepaymentStatus(long[] id, string prepaymentStatus)
        {
            var entity = await db.Set<Prepayment>().Where(s => id.Any(w => w == s.id)).ToListAsync();
            if (entity.Count <= 0)
                return true;

            foreach (var tm in entity)
            {
                tm.prepaymentStatus = prepaymentStatus;
            }

            await db.SaveChangesAsync();
            return true;
        }


        public string PaymentQuery(List<AmountDueModel> paymentDueList,
            DemandNoticePaymentHistoryModel dnph, string status, string createdby)
        {
            string query = "";

            if (paymentDueList.Count > 0)
            {
                foreach (var tm in paymentDueList)
                {
                    switch (tm.Category)
                    {
                        case Category.Arrears:
                            query = query + $"update tbl_demandNoticeArrears set arrearsStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.Id}';";
                            break;
                        case Category.Penalty:
                            query = query + $"update tbl_demandNoticePenalty set itemPenaltyStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.Id}';";
                            break;
                        case Category.Item:
                            query = query + $"update tbl_demandNoticeItem set itemStatus = '{status}', " +
                                $" lastModifiedDate = getdate(),lastmodifiedby='{createdby}' where id='{tm.Id}';";
                            break;
                        default:
                            break;
                    }
                }

                query = query + $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where billingNumber = '{dnph.BillingNumber}';";
                query = query + $"update tbl_demandNoticePaymentHistory set paymentStatus = 'APPROVED',lastModifiedDate=getdate(),lastmodifiedby='{createdby}' where id = '{dnph.Id}';";
            }

            return query;
        }

        public async Task<decimal> TotalAmountPaid(long billNumber)
        {
            var entities = await db.Set<DemandNoticePaymentHistory>()
                .Where(x => x.BillingNumber == billNumber && x.PaymentStatus == "APPROVED")
                .ToListAsync();
            if (entities.Count <= 0)
                return 0;

            return entities.Sum(x => x.Amount);
        }
    }
}

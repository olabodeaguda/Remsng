using Microsoft.EntityFrameworkCore;
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
    public class DemandNoticeTaxpayersRepository : IDemandNoticeTaxpayersRepository
    {
        private readonly DbContext db;
        private readonly IErrorRepository errorDao;
        public DemandNoticeTaxpayersRepository(DbContext _db, IErrorRepository errorRepository)
        {
            db = _db;
            errorDao = errorRepository;
        }

        public async Task<List<DemandNoticeTaxpayersModel>> getTaxpayerByIds(string[] ids, int billingYr)
        {
            //StringBuilder stringBuilder = new StringBuilder();
            //string tIds = stringBuilder.AppendJoin(',', ids).ToString();
            //string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize from tbl_demandNoticeTaxpayers" +
            //    $" where taxpayerId in ({tIds}) and billingYr = {billingYr} and demandNoticeStatus not in ('CLOSED','CANCEL')";

            Guid[] guids = ids.Select(x => Guid.Parse(x)).ToArray();
            string[] status = { "CLOSED", "CANCEL" };

            var result = await db.Set<DemandNoticeTaxpayer>().Where(x => guids.Any(p => p == x.TaxpayerId) && status.Any(q => q != x.DemandNoticeStatus))
                .Select(x => new DemandNoticeTaxpayersModel()
                {
                    AddressName = x.AddressName,
                    BillingNumber = x.BillingNumber,
                    BillingYr = x.BillingYr,
                    CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                    CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DemandNoticeStatus = x.DemandNoticeStatus,
                    DnId = x.DnId,
                    DomainName = x.DomainName,
                    Id = x.Id,
                    IsUnbilled = x.IsUnbilled,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    LcdaAddress = x.LcdaAddress,
                    LcdaLogoFileName = x.LcdaLogoFileName,
                    LcdaName = x.LcdaName,
                    LcdaState = x.LcdaState,
                    RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                    TaxpayerId = x.TaxpayerId,
                    TaxpayersName = x.TaxpayersName,
                    WardName = x.WardName

                }).ToListAsync();
            return result;
        }


        public async Task<List<DemandNoticeTaxpayersModel>> getPendingDemandNoticeByTaxpayerByIds(Guid id)
        {
            string[] status = { "CLOSED", "CANCEL", "PAID","DELETED" };

            var result = await db.Set<DemandNoticeTaxpayer>().Where(x => x.TaxpayerId == id && !status.Any(q => q == x.DemandNoticeStatus))
                .Select(x => new DemandNoticeTaxpayersModel()
                {
                    AddressName = x.AddressName,
                    BillingNumber = x.BillingNumber,
                    BillingYr = x.BillingYr,
                    CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                    CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DemandNoticeStatus = x.DemandNoticeStatus,
                    DnId = x.DnId,
                    DomainName = x.DomainName,
                    Id = x.Id,
                    IsUnbilled = x.IsUnbilled,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    LcdaAddress = x.LcdaAddress,
                    LcdaLogoFileName = x.LcdaLogoFileName,
                    LcdaName = x.LcdaName,
                    LcdaState = x.LcdaState,
                    RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                    TaxpayerId = x.TaxpayerId,
                    TaxpayersName = x.TaxpayersName,
                    WardName = x.WardName

                }).ToListAsync();
            return result;
        }


        public async Task<bool> UpdateSatus(Guid id, string status)
        {
            var entity = await db.Set<DemandNoticeTaxpayer>().FindAsync(id);
            if (entity == null)
            {
                throw new NotFoundException("Request does not exist");
            }
            entity.DemandNoticeStatus = status;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSatus(Guid[] ids, string status)
        {
            foreach (var tm in ids)
            {
                var entity = await db.Set<DemandNoticeTaxpayer>().FindAsync(tm);
                if (entity != null)
                {
                    entity.DemandNoticeStatus = status;
                }
            }
            await db.SaveChangesAsync();
            return true;
        }
        public async Task<long> NewBillingNumber()
        {
            DbResponse dbResponse = new DbResponse();
            try
            {
                long result = await db.Set<DemandNoticeTaxpayer>().MaxAsync(x => x.BillingNumber);//.OrderByDescending(x => x.DateCreated).Select(x => x.BillingNumber).FirstOrDefaultAsync();
                //if (string.IsNullOrEmpty(result))
                //{
                //    return 0;
                //}
                return result; //long.Parse(result);
                // dbResponse = await db.Set<DbResponse>().FromSql("sp_currentMaxBilling").FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<Response> Add(DemandNoticeTaxpayersModel[] demandnotice)
        {
            DemandNoticeTaxpayer[] d = demandnotice.Select(dnt => new DemandNoticeTaxpayer
            {
                DnId = dnt.DnId,
                BillingYr = dnt.BillingYr,
                CreatedBy = dnt.CreatedBy,
                TaxpayerId = dnt.TaxpayerId,
                DomainName = dnt.DomainName,
                LcdaAddress = dnt.LcdaAddress,
                LcdaState = dnt.LcdaState,
                LcdaLogoFileName = dnt.LcdaLogoFileName,
                CouncilTreasurerSigFilen = dnt.CouncilTreasurerSigFilen,
                RevCoodinatorSigFilen = dnt.RevCoodinatorSigFilen,
                CouncilTreasurerMobile = dnt.CouncilTreasurerMobile,
                LcdaName = dnt.LcdaName,
                BillingNumber = dnt.BillingNumber,
                AddressName = dnt.AddressName,
                DateCreated = dnt.DateCreated,
                DemandNoticeStatus = dnt.DemandNoticeStatus,
                Id = dnt.Id,
                IsUnbilled = dnt.IsUnbilled,
                Lastmodifiedby = dnt.Lastmodifiedby,
                LastModifiedDate = dnt.LastModifiedDate,
                TaxpayersName = dnt.TaxpayersName,
                WardName = dnt.WardName,
                Period = dnt.Period,
                IsRunArrears = dnt.IsRunArrears,
                IsRunPenalty = dnt.IsRunPenalty
            }).ToArray();
            db.Set<DemandNoticeTaxpayer>().AddRange(d);
            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Taxpayer has been added successfully"
            };
        }

        public async Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel)
        {
            var query = db.Set<DemandNoticeTaxpayer>().Include(x => x.DemandNotice)
                .Where(x => x.DemandNotice.BatchNo == batchId);

            var results = await query.Select(x => new DemandNoticeTaxpayersModel
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName
            }).OrderByDescending(x => x.TaxpayersName)
                .Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToArrayAsync();

            int totalCount = await query.CountAsync();

            return new PageModel<DemandNoticeTaxpayersModel[]>
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + (int)Math.Ceiling(((double)totalCount / pageModel.PageSize))
            };
        }

        public async Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchIdAsync(string batchId)
        {
            //List<DemandNoticeTaxpayers> results = await db.Set<DemandNoticeTaxpayers>()
            //    .FromSql("sp_getDnTByBatchNumber @p0",
            //    new object[] { batchId }).ToListAsync();

            var result = await db.Set<DemandNoticeTaxpayer>().Include(x => x.DemandNotice)
               .Where(x => x.DemandNotice.BatchNo == batchId).Select(x => new DemandNoticeTaxpayersModel()
               {
                   AddressName = x.AddressName,
                   BillingNumber = x.BillingNumber,
                   BillingYr = x.BillingYr,
                   CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                   CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                   CreatedBy = x.CreatedBy,
                   DateCreated = x.DateCreated,
                   DemandNoticeStatus = x.DemandNoticeStatus,
                   DnId = x.DnId,
                   DomainName = x.DomainName,
                   Id = x.Id,
                   IsUnbilled = x.IsUnbilled,
                   Lastmodifiedby = x.Lastmodifiedby,
                   LastModifiedDate = x.LastModifiedDate,
                   LcdaAddress = x.LcdaAddress,
                   LcdaLogoFileName = x.LcdaLogoFileName,
                   LcdaName = x.LcdaName,
                   LcdaState = x.LcdaState,
                   RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                   TaxpayerId = x.TaxpayerId,
                   TaxpayersName = x.TaxpayersName,
                   WardName = x.WardName
               }).ToListAsync();

            return result;
        }

        public async Task<DemandNoticeTaxpayersModel> ByBillingNo(long billingNo)
        {
            var result = await db.Set<DemandNoticeTaxpayer>().Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                Period = x.Period,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty,
                StreetName = x.AddressName
            }).FirstOrDefaultAsync(p => p.BillingNumber == billingNo);

            return result;
        }

        public async Task<DemandNoticeTaxpayersModel> GetSingleTaxpayerAsync(string taxpayerId, int billingYr)
        {
            //var x = await db.Set<DemandNoticeTaxpayers>().FromSql("sp_getDemandNoticeTaxpayerByYear @p0,@p1",
            //  new object[] { taxpayerId, billingYr }).FirstOrDefaultAsync();

            //if (x == null)
            //{
            //    return null;
            //}

            Guid tpayer = Guid.Parse(taxpayerId);

            var result = await db.Set<DemandNoticeTaxpayer>().Include(x => x.DemandNotice)
                .Select(x => new DemandNoticeTaxpayersModel()
                {
                    AddressName = x.AddressName,
                    BillingNumber = x.BillingNumber,
                    BillingYr = x.BillingYr,
                    CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                    CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DemandNoticeStatus = x.DemandNoticeStatus,
                    DnId = x.DnId,
                    DomainName = x.DomainName,
                    Id = x.Id,
                    IsUnbilled = x.IsUnbilled,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    LcdaAddress = x.LcdaAddress,
                    LcdaLogoFileName = x.LcdaLogoFileName,
                    LcdaName = x.LcdaName,
                    LcdaState = x.LcdaState,
                    RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                    TaxpayerId = x.TaxpayerId,
                    TaxpayersName = x.TaxpayersName,
                    WardName = x.WardName

                }).FirstOrDefaultAsync(t => t.BillingYr == billingYr && t.TaxpayerId == tpayer);
            return result;
        }

        public async Task<List<DemandNoticeTaxpayersModel>> GetDNTaxpayerByBatchNoAsync(string batchno)
        {
            var tu = await db.Set<DemandNoticeTaxpayer>().Include(d => d.DemandNotice)
                .Where(p => p.DemandNotice.BatchNo == batchno).Select(x => new DemandNoticeTaxpayersModel()
                {
                    AddressName = x.AddressName,
                    BillingNumber = x.BillingNumber,
                    BillingYr = x.BillingYr,
                    CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                    CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                    CreatedBy = x.CreatedBy,
                    DateCreated = x.DateCreated,
                    DemandNoticeStatus = x.DemandNoticeStatus,
                    DnId = x.DnId,
                    DomainName = x.DomainName,
                    Id = x.Id,
                    IsUnbilled = x.IsUnbilled,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    LcdaAddress = x.LcdaAddress,
                    LcdaLogoFileName = x.LcdaLogoFileName,
                    LcdaName = x.LcdaName,
                    LcdaState = x.LcdaState,
                    RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                    TaxpayerId = x.TaxpayerId,
                    TaxpayersName = x.TaxpayersName,
                    WardName = x.WardName

                }).ToListAsync();
            return tu;
        }

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo(long billingNo, string createdBy)
        {
            string query = $"update tbl_demandNoticeArrears set arrearsStatus='CANCEL', lastmodifiedby='{createdBy}', lastModifiedDate=GETDATE() where billingNo = '{billingNo}'; ";
            query = query + $"update tbl_demandNoticeItem SET itemStatus='CANCEL', lastmodifiedby='{createdBy}', lastModifiedDate=GETDATE()  where billingNo ='{billingNo}';";
            query = query + $"update tbl_demandNoticePenalty SET itemPenaltyStatus='CANCEL', lastmodifiedby='{createdBy}', lastModifiedDate=GETDATE() where billingNo = '{billingNo}';";
            query = query + $"update tbl_demandNoticeTaxpayers SET demandNoticeStatus = 'CANCEL', lastmodifiedby='{createdBy}', lastModifiedDate=GETDATE() where billingNumber = '{billingNo}';";

            int count = await db.Database.ExecuteSqlCommandAsync(query);

            if (count > 0)
            {
                //log
                ErrorModel error = new ErrorModel()
                {
                    DateCreated = DateTime.Now,
                    ErrorType = "Delete Demand Notice",
                    Errorvalue = $"{billingNo}, created by {createdBy}",
                    Id = Guid.NewGuid(),
                    OwnerId = Guid.NewGuid()
                };

                await errorDao.Add(error);

                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{billingNo} has been deleted"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while deleting {billingNo}"
                };
            }
        }

        public async Task<List<DemandNoticeTaxpayersModel>> SearchAllAsync(string qs)
        {
            string[] q = qs.Split(new char[] { ' ' });
            string query = $"select tbl_demandNoticeTaxpayers.* from tbl_demandNoticeTaxpayers where ";
            string subQuery = string.Empty;
            for (int i = 0; i < q.Length; i++)
            {
                if (string.IsNullOrEmpty(q[i].Trim()))
                {
                    continue;
                }

                string t = $" (taxpayersName like '%{q[i]}%' or billingNumber like '%{q[i]}%' " +
                 $"or addressName like '%{q[i]}%'or wardName like '%{q[i]}%')";
                if (!string.IsNullOrEmpty(subQuery))
                {
                    subQuery = subQuery + " and ";
                }
                subQuery = subQuery + t;
            }
            query = query + $" {subQuery}";

            var results = await db.Set<DemandNoticeTaxpayer>().FromSql(query).ToListAsync();

            var result = results.Distinct().ToList();
            return result.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName
            }).ToList();
        }

        public async Task<bool> BlinkClosesDemandNoticeByCompany(Guid companyId)
        {
            string query = $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = 'CLOSED' " +
                $" WHERE id IN (select id from tbl_taxPayer where companyId = '{companyId}')";

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<DemandNoticeTaxpayersModel[]> GetAllReceivables()
        {
            string query = $"select dnt.* from tbl_demandNoticeTaxpayers as dnt " +
                $"where dnt.taxpayerId not in (select taxpayerId from tbl_demandNoticePenalty where itemPenaltyStatus != 'PAID') " +
                $"and dnt.demandNoticeStatus in ('PENDING','PART_PAYMENT')";

            var result = await db.Set<DemandNoticeTaxpayer>().FromSql(query).ToArrayAsync();
            return result.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName

            }).ToArray();
        }

        public async Task<DemandNoticeTaxpayersModel[]> GetAllReceivables(Guid[] taxpayerIds)
        {
            string ids = taxpayerIds.Select(x => x.ToString()).ToArray().FormatString();

            string query = $"select dnt.* from tbl_demandNoticeTaxpayers as dnt " +
                $"where dnt.taxpayerId not in " +
                $"(select taxpayerId from tbl_demandNoticePenalty where itemPenaltyStatus != 'PAID' and taxpayerId in ({ids}) ) " +
                $"and dnt.demandNoticeStatus in ('PENDING','PART_PAYMENT') and taxpayerId in ({ids}) and dnt.billingYr = {DateTime.Now.Year - 1}";

            var result = await db.Set<DemandNoticeTaxpayer>().FromSql(query).ToArrayAsync();
            return result.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName

            }).ToArray();
        }

        public async Task<List<DemandNoticeTaxpayersModel>> GetTaxpayerPayables(Guid taxpayerId)
        {
            //string query = $"select tbl_demandNoticeTaxpayers.* from tbl_demandNoticeTaxpayers " +
            //    $"where taxpayerId = '{taxpayerId}' and demandNoticeStatus in ('PART_PAYMENT','PENDING','PAID','CLOSED') order by  dateCreated";

            string[] status = { "PART_PAYMENT", "PENDING", "PAID", "CLOSED" };

            var result = await db.Set<DemandNoticeTaxpayer>()
                .Where(x => x.TaxpayerId == taxpayerId && status.Any(p => p == x.DemandNoticeStatus))
                .OrderByDescending(d => d.DateCreated).ToListAsync();

            return result.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName

            }).ToList();
        }

        public async Task<bool> MoveToBills(long billno)
        {
            string query = $"Update tbl_demandNoticeTaxpayers set isUnbilled = 0 where billingNumber ='{billno}'";
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            return count > 0;
        }

        public async Task<bool> MoveToUnBills(long billno)
        {
            string query = $"Update tbl_demandNoticeTaxpayers set isUnbilled = 1 where billingNumber ='{billno}'";
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            return count > 0;
        }

        public async Task<PageModel<DemandNoticeTaxpayersModel[]>> Search(DemandNoticeRequestModel rhModel,
            PageModel pageModel)
        {
            var query = db.Set<DemandNoticeTaxpayer>()
                .Include(p => p.DemandNotice)
                .ThenInclude(d => d.Street)
                .Where(x => x.BillingYr == rhModel.dateYear);

            if (rhModel.streetId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.StreetId == rhModel.streetId);
            }
            else if (rhModel.wardId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.WardId == rhModel.wardId);
            }

            if (!string.IsNullOrEmpty(rhModel.searchByName))
            {
                string[] prams = rhModel.searchByName.Split(new char[] { ' ' });

                switch (prams.Length)
                {
                    case 1:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{rhModel.searchByName}%"));
                        break;
                    case 2:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{prams[0]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[1]}%"));
                        break;
                    case 3:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{prams[0]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[1]}%") &&
                        EF.Functions.Like(x.TaxpayersName, $"%{prams[2]}%"));
                        break;
                    case 4:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{prams[0]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[1]}%")
                        && EF.Functions.Like(x.TaxpayersName, $"%{prams[2]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[3]}%"));
                        break;
                    default:
                        break;
                }
            }

            var result = await query.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                StreetName = x.DemandNotice.Street.StreetName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty,
                Period = x.Period
            }).Where(x => x.DemandNoticeStatus != "DELETED").OrderByDescending(x => x.DateCreated).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToArrayAsync();

            int totalCount = await query.CountAsync();
            return new PageModel<DemandNoticeTaxpayersModel[]>
            {
                PageNum = pageModel.PageNum,
                PageSize = pageModel.PageSize,
                data = result,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + (int)(((double)totalCount / pageModel.PageSize))
            };
        }

        public async Task<DemandNoticeTaxpayersModel[]> Search(DemandNoticeRequestModel rhModel)
        {
            var query = db.Set<DemandNoticeTaxpayer>()
                .Include(p => p.DemandNotice)
                .ThenInclude(d => d.Street)
                .Where(x => x.BillingYr == rhModel.dateYear);

            if (rhModel.streetId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.StreetId == rhModel.streetId);
            }
            else if (rhModel.wardId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.WardId == rhModel.wardId);
            }

            if (!string.IsNullOrEmpty(rhModel.searchByName))
            {
                string[] prams = rhModel.searchByName.Split(new char[] { ' ' });

                switch (prams.Length)
                {
                    case 1:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{rhModel.searchByName}%"));
                        break;
                    case 2:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{prams[0]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[1]}%"));
                        break;
                    case 3:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{prams[0]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[1]}%") &&
                        EF.Functions.Like(x.TaxpayersName, $"%{prams[2]}%"));
                        break;
                    case 4:
                        query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{prams[0]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[1]}%")
                        && EF.Functions.Like(x.TaxpayersName, $"%{prams[2]}%") && EF.Functions.Like(x.TaxpayersName, $"%{prams[3]}%"));
                        break;
                    default:
                        break;
                }
            }

            var result = await query.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                StreetName = x.DemandNotice.Street.StreetName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty,
                Period = x.Period
            }).Where(x => x.DemandNoticeStatus != "DELETED").OrderByDescending(x => x.DateCreated).ToArrayAsync();

            return result;
        }

        public async Task<PageModel<DemandNoticeTaxpayersModel[]>> SearchByLcdaId(DemandNoticeRequestModel rhModel,
            PageModel pageModel, Guid lcdaId)
        {
            var query = db.Set<DemandNoticeTaxpayer>()
                .Include(p => p.DemandNotice)
                .ThenInclude(d => d.Street)
                .Where(x => x.BillingYr == rhModel.dateYear && x.DemandNotice.LcdaId == lcdaId);

            if (rhModel.streetId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.StreetId == rhModel.streetId);
            }
            else if (rhModel.wardId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.WardId == rhModel.wardId);
            }

            if (!string.IsNullOrEmpty(rhModel.searchByName))
            {
                query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{rhModel.searchByName}%"));
            }

            var result = await query.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                StreetName = x.DemandNotice.Street.StreetName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty,
                Period = x.Period
            }).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).OrderByDescending(x => x.DateCreated).ToArrayAsync();

            int totalCount = await query.CountAsync();
            return new PageModel<DemandNoticeTaxpayersModel[]>
            {
                PageNum = pageModel.PageNum,
                PageSize = pageModel.PageSize,
                data = result,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + (int)(((double)totalCount / pageModel.PageSize))
            };
        }

        public async Task<DemandNoticeTaxpayersModel[]> SearchTaxpayers(DemandNoticeRequestModel rhModel)
        {
            string[] statuss = { "CANCEL", "CLOSED", "DELETED" };
            var query = db.Set<DemandNoticeTaxpayer>()
                .Include(p => p.DemandNotice)
                .ThenInclude(d => d.Street)
                .Where(x => x.BillingYr == rhModel.dateYear && !statuss.Any(p => p == x.DemandNoticeStatus));

            if (rhModel.streetId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.StreetId == rhModel.streetId);
            }
            else if (rhModel.wardId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.WardId == rhModel.wardId);
            }

            if (!string.IsNullOrEmpty(rhModel.searchByName))
            {
                query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{rhModel.searchByName}%"));
            }

            if (rhModel.TaxpayerIds.Length > 0)
            {
                query = query.Where(x => rhModel.TaxpayerIds.Any(p => p == x.TaxpayerId));
            }

            var result = await query.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                StreetName = x.DemandNotice.Street.StreetName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty
            }).OrderByDescending(x => x.DateCreated).ToArrayAsync();

            return result;
        }


        public async Task<DemandNoticeTaxpayersModel[]> SearchTaxpayers2(DemandNoticeRequestModel rhModel)
        {
            string[] statuss = { "CANCEL", "CLOSED", "DELETED" };
            var query = db.Set<DemandNoticeTaxpayer>()
                .Include(p => p.DemandNotice)
                .ThenInclude(d => d.Street)
                .Where(x => x.BillingYr == rhModel.dateYear && !statuss.Any(p => p == x.DemandNoticeStatus) && x.Period == rhModel.Period);

            if (rhModel.streetId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.StreetId == rhModel.streetId);
            }
            else if (rhModel.wardId != default(Guid))
            {
                query = query.Where(x => x.DemandNotice.WardId == rhModel.wardId);
            }

            if (!string.IsNullOrEmpty(rhModel.searchByName))
            {
                query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{rhModel.searchByName}%"));
            }

            if (rhModel.TaxpayerIds.Length > 0)
            {
                query = query.Where(x => rhModel.TaxpayerIds.Any(p => p == x.TaxpayerId));
            }

            var result = await query.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                StreetName = x.DemandNotice.Street.StreetName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty
            }).OrderByDescending(x => x.DateCreated).ToArrayAsync();

            return result;
        }



        public async Task<DemandNoticeTaxpayersModel[]> ConstructByTaxpayerIds(DemandNoticeRequestModel model,
            Dictionary<string, ImagesModel> images)
        {
            string LcdaLogoFileName = images.ContainsKey(ImgTypesEnum.LOGO.ToString()) ? images[ImgTypesEnum.LOGO.ToString()].ImgFilename : string.Empty;
            string RevCoodinatorSigFilen = images.ContainsKey(ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString()) ? images[ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString()].ImgFilename : string.Empty;
            string CouncilTreasurerSigFilen = images.ContainsKey(ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString()) ? images[ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString()].ImgFilename : string.Empty;

            long billNumber = model.InitialBillingNumber;
            var query = await db.Set<TaxPayer>()
                .Include(x => x.Street)
                .ThenInclude(x => x.Ward)
                .ThenInclude(x => x.Lcda)
                .ThenInclude(r => r.Domain)
                .Include(x => x.Address)
                .Where(x => x.TaxpayerStatus == "ACTIVE" && model.TaxpayerIds.Any(p => p == x.Id))
                .Select(x => new DemandNoticeTaxpayersModel()
                {
                    AddressName = $"{x.Address.Addressnumber}, {x.Street.StreetName}",
                    BillingYr = model.dateYear,
                    CouncilTreasurerSigFilen = CouncilTreasurerSigFilen,
                    CreatedBy = model.createdBy,
                    DateCreated = DateTime.Now,
                    DemandNoticeStatus = "PENDING",
                    DomainName = x.Street.Ward.Lcda.Domain.DomainName,
                    Id = Guid.NewGuid(),
                    IsUnbilled = model.isUnbilled,
                    Lastmodifiedby = x.Lastmodifiedby,
                    LastModifiedDate = x.LastModifiedDate,
                    LcdaAddress = model.LcdaAddress,
                    LcdaState = model.LcdaState,
                    CouncilTreasurerMobile = model.TreasurerMobile,
                    LcdaLogoFileName = LcdaLogoFileName,
                    LcdaName = x.Street.Ward.Lcda.LcdaName,
                    RevCoodinatorSigFilen = RevCoodinatorSigFilen,
                    TaxpayerId = x.Id,
                    TaxpayersName = $"{x.Surname} {x.Firstname} {x.Lastname}",
                    WardName = x.Street.Ward.WardName,
                    StreetName = x.Street.StreetName,
                    DnId = model.DemandNoticeId,
                    BillingNumber = billNumber,
                    Period = model.Period
                }).ToArrayAsync();
            DemandNoticeTaxpayersModel[] lst = new DemandNoticeTaxpayersModel[query.Length];

            if (!model.useSingleBill)
            {
                for (int i = 0; i < query.Length; i++)
                {
                    billNumber = billNumber + 1;
                    query[i].BillingNumber = billNumber;
                }
            }

            return query;
        }

        public async Task<DemandNoticeTaxpayersModel[]> ById(Guid[] ids)
        {
            var result = await db.Set<DemandNoticeTaxpayer>().Include(x => x.DemandNoticeItem)
                .Where(r => ids.Any(p => p == r.Id))
            .Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty,
                DemandNoticeItem = x.DemandNoticeItem.Select(d => new DemandNoticeItemModel
                {
                    AmountPaid = d.AmountPaid,
                    BillingNo = d.BillingNo,
                    CreatedBy = d.CreatedBy,
                    DateCreated = d.DateCreated,
                    DemandNoticeId = d.DemandNoticeId,
                    DnTaxpayersDetailsId = d.dn_taxpayersDetailsId,
                    Id = d.Id,
                    ItemAmount = d.ItemAmount,
                    ItemId = d.ItemId,
                    ItemName = d.ItemName,
                    ItemStatus = d.ItemStatus,
                    Lastmodifiedby = d.Lastmodifiedby,
                    LastModifiedDate = d.LastModifiedDate,
                    TaxpayerId = d.TaxpayerId
                }).ToList()
            }).ToArrayAsync();

            return result;
        }

        public async Task<DemandNoticeTaxpayersModel> ById(Guid id)
        {
            var result = await db.Set<DemandNoticeTaxpayer>().Include(x => x.DemandNoticeItem)
                .Where(r => r.Id == id)
            .Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty,
                DemandNoticeItem = x.DemandNoticeItem.Select(d => new DemandNoticeItemModel
                {
                    AmountPaid = d.AmountPaid,
                    BillingNo = d.BillingNo,
                    CreatedBy = d.CreatedBy,
                    DateCreated = d.DateCreated,
                    DemandNoticeId = d.DemandNoticeId,
                    DnTaxpayersDetailsId = d.dn_taxpayersDetailsId,
                    Id = d.Id,
                    ItemAmount = d.ItemAmount,
                    ItemId = d.ItemId,
                    ItemName = d.ItemName,
                    ItemStatus = d.ItemStatus,
                    Lastmodifiedby = d.Lastmodifiedby,
                    LastModifiedDate = d.LastModifiedDate,
                    TaxpayerId = d.TaxpayerId
                }).ToList()
            }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<bool> UpdateArrearsStatus(DemandNoticeTaxpayersModel[] dntaxpayers, bool isRunArrears)
        {
            foreach (var dnt in dntaxpayers)
            {
                DemandNoticeTaxpayer s = await db.Set<DemandNoticeTaxpayer>().FindAsync(dnt.Id);
                if (s != null)
                {
                    s.IsRunArrears = isRunArrears;
                }
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePenaltyStatus(DemandNoticeTaxpayersModel[] dntaxpayers, bool isRunPenalty)
        {
            foreach (var dnt in dntaxpayers)
            {
                DemandNoticeTaxpayer s = await db.Set<DemandNoticeTaxpayer>().FindAsync(dnt.Id);
                if (s != null)
                {
                    s.IsRunPenalty = isRunPenalty;
                }
            }

            await db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> UpdatePenaltyStatus(Guid[] dntaxpayers, bool isRunPenalty)
        {
            foreach (var dnt in dntaxpayers)
            {
                DemandNoticeTaxpayer s = await db.Set<DemandNoticeTaxpayer>().FindAsync(dnt);
                if (s != null)
                {
                    s.IsRunPenalty = isRunPenalty;
                }
            }

            await db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> UpdateAddress(Guid taxpayerId, string address)
        {
            string[] status = { "PART_PAYMENT", "PENDING", "PAID" };
            var entities = await db.Set<DemandNoticeTaxpayer>().Where(x => x.TaxpayerId == taxpayerId && status.Any(s => s == x.DemandNoticeStatus)).ToListAsync();
            if (entities.Count > 0)
            {
                foreach (var tm in entities)
                {
                    tm.AddressName = address;
                }
            }
            else
            {
                return true;
            }

            int count = await db.SaveChangesAsync();

            return count > 0;
        }

        public async Task<bool> UpdateTaxpayerName(Guid taxpayerId, string name)
        {
            string[] status = { "PART_PAYMENT", "PENDING", "PAID" };
            var entities = await db.Set<DemandNoticeTaxpayer>().Where(x => x.TaxpayerId == taxpayerId && status.Any(s => s == x.DemandNoticeStatus)).ToListAsync();
            if (entities.Count > 0)
            {
                foreach (var tm in entities)
                {
                    tm.TaxpayersName = name;
                }
            }
            else
            {
                return true;
            }

            int count = await db.SaveChangesAsync();

            return count > 0;
        }

        public async Task<bool> UpdateWard(Guid taxpayerId, string wardName)
        {
            string[] status = new string[] { "PENDING", "PART_PAYMENT", "PAID" };
            var entities = await db.Set<DemandNoticeTaxpayer>()
                .Where(x => x.TaxpayerId == taxpayerId && status.Any(p => p == x.DemandNoticeStatus))
                .ToArrayAsync();

            if (entities.Length <= 0)
                return true;

            foreach (var tm in entities)
            {
                tm.WardName = wardName;
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateDemandNoticeStatus(Guid[] demandNoticeIds, DemandNoticeStatus status)
        {
            var entities = await db.Set<DemandNoticeTaxpayer>()
               .Where(x => demandNoticeIds.Any(p => p == x.Id))
               .ToArrayAsync();

            if (entities.Length < 0)
                return false;
            foreach (var tm in entities)
            {
                tm.DemandNoticeStatus = status.ToString();
                tm.LastModifiedDate = DateTime.Now;
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateWardStreet(Guid[] taxpayerIds, string wardName, string street,
            List<TaxPayerModel> lst)
        {
            string[] status = new string[] { "PENDING", "PART_PAYMENT", "PAID" };
            var entities = await db.Set<DemandNoticeTaxpayer>()
                .Where(x => taxpayerIds.Any(s => s == x.TaxpayerId) && status.Any(p => p == x.DemandNoticeStatus))
                .ToArrayAsync();

            if (entities.Length <= 0)
                return true;

            foreach (var tm in entities)
            {
                var f = lst.FirstOrDefault(s => s.Id == tm.TaxpayerId);

                tm.WardName = wardName;
                if (f != null)
                {
                    tm.AddressName = $"{f.StreetNumber} {f.StreetName}";
                }
                else
                {
                    tm.AddressName = $"{street}";
                }
            }

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<DemandNoticeTaxpayersModel[]> GetById(Guid[] ids)
        {
            string[] statuss = { "CANCEL", "CLOSED", "DELETED" };
            var query = db.Set<DemandNoticeTaxpayer>()
                .Include(p => p.DemandNotice)
                .ThenInclude(d => d.Street)
                .Where(x => ids.Any(s => s == x.TaxpayerId) && !statuss.Any(p => p == x.DemandNoticeStatus));

            var result = await query.Select(x => new DemandNoticeTaxpayersModel()
            {
                AddressName = x.AddressName,
                BillingNumber = x.BillingNumber,
                BillingYr = x.BillingYr,
                CouncilTreasurerMobile = x.CouncilTreasurerMobile,
                CouncilTreasurerSigFilen = x.CouncilTreasurerSigFilen,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DemandNoticeStatus = x.DemandNoticeStatus,
                DnId = x.DnId,
                DomainName = x.DomainName,
                Id = x.Id,
                IsUnbilled = x.IsUnbilled,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaAddress = x.LcdaAddress,
                LcdaLogoFileName = x.LcdaLogoFileName,
                LcdaName = x.LcdaName,
                LcdaState = x.LcdaState,
                RevCoodinatorSigFilen = x.RevCoodinatorSigFilen,
                TaxpayerId = x.TaxpayerId,
                TaxpayersName = x.TaxpayersName,
                WardName = x.WardName,
                StreetName = x.DemandNotice.Street.StreetName,
                IsRunArrears = x.IsRunArrears,
                IsRunPenalty = x.IsRunPenalty
            }).OrderByDescending(x => x.DateCreated).ToArrayAsync();

            return result;
        }
    }
}

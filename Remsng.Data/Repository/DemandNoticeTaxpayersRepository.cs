using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class DemandNoticeTaxpayersRepository : AbstractRepository
    {
        private readonly ErrorRepository errorDao;
        public DemandNoticeTaxpayersRepository(DbContext _db) : base(_db)
        {
            errorDao = new ErrorRepository(_db);
        }

        public async Task<List<DemandNoticeTaxpayersModel>> getTaxpayerByIds(string[] ids, int billingYr)
        {
            //StringBuilder stringBuilder = new StringBuilder();
            //string tIds = stringBuilder.AppendJoin(',', ids).ToString();
            //string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize from tbl_demandNoticeTaxpayers" +
            //    $" where taxpayerId in ({tIds}) and billingYr = {billingYr} and demandNoticeStatus not in ('CLOSED','CANCEL')";

            Guid[] guids = ids.Select(x => Guid.Parse(x)).ToArray();
            string[] status = { "CLOSED", "CANCEL" };

            var result = await db.Set<DemandNoticeTaxpayers>().Where(x => guids.Any(p => p == x.TaxpayerId) && status.Any(q => q != x.DemandNoticeStatus))
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

        public async Task<bool> UpdateTaxPayer(Guid id, string status)
        {
            //string query = $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where id='{id}'";

            //int count = await db.Database.ExecuteSqlCommandAsync(query);
            //if (count > 0)
            //{
            //    return true;
            //}

            var entity = await db.Set<DemandNoticeTaxpayers>().FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            entity.DemandNoticeStatus = status;
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTaxpayers(string[] taxpayerIds, int billingYr, string status)
        {

            StringBuilder stringBuilder = new StringBuilder();
            string tIds = stringBuilder.AppendJoin(',', taxpayerIds).ToString();

            string query_dnt_Ids = $"select id from tbl_demandNoticeTaxpayers" +
                $" where taxpayerId in ({tIds}) and billingYr = {billingYr} and demandNoticeStatus not in ('CLOSED','CANCEL')";

            string query = $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where id in ({query_dnt_Ids})";

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateTaxPayers(Guid[] demandNoticeTaxpayerIds, string status)
        {
            string ds = string.Empty;

            for (int i = 0; i < demandNoticeTaxpayerIds.Length; i++)
            {
                ds = ds + "'" + demandNoticeTaxpayerIds[i] + "'";
                if (i < demandNoticeTaxpayerIds.Length - 1)
                {
                    ds = ds + ",";
                }
            }

            string query = $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where id in ({ds})";

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<long> NewBillingNumber()
        {
            DbResponse dbResponse = new DbResponse();
            try
            {
                dbResponse = await db.Set<DbResponse>().FromSql("sp_currentMaxBilling").FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return dbResponse == null ? 0 : long.Parse(dbResponse.msg);
        }

        public async Task<Response> Add(DemandNoticeTaxpayersModel dnt)
        {
            long billnumber = await NewBillingNumber();
            dnt.BillingNumber = (billnumber + 1).ToString();
            DemandNoticeTaxpayers d = new DemandNoticeTaxpayers
            {
                DnId = dnt.DnId,
                BillingYr = dnt.BillingYr,
                CreatedBy = dnt.CreatedBy,
                TaxpayerId = dnt.TaxpayerId,
                DomainName = dnt.DomainName,
                LcdaAddress = dnt.LcdaAddress == null ? string.Empty : dnt.LcdaAddress,
                LcdaState = dnt.LcdaState == null ? string.Empty : dnt.LcdaState,
                LcdaLogoFileName = dnt.LcdaLogoFileName == null ? string.Empty : dnt.LcdaLogoFileName,
                CouncilTreasurerSigFilen = dnt.CouncilTreasurerSigFilen == null ? string.Empty : dnt.CouncilTreasurerSigFilen,
                RevCoodinatorSigFilen = dnt.RevCoodinatorSigFilen == null ? string.Empty : dnt.RevCoodinatorSigFilen,
                CouncilTreasurerMobile = dnt.CouncilTreasurerMobile == null ? string.Empty : dnt.CouncilTreasurerMobile,
                LcdaName = dnt.LcdaName,
                BillingNumber = (billnumber + 1).ToString(),
                AddressName = dnt.AddressName,
                DateCreated = dnt.DateCreated,
                DemandNoticeStatus = dnt.DemandNoticeStatus,
                Id = dnt.Id,
                IsUnbilled = dnt.IsUnbilled,
                Lastmodifiedby = dnt.Lastmodifiedby,
                LastModifiedDate = dnt.LastModifiedDate,
                TaxpayersName = dnt.TaxpayersName,
                WardName = dnt.WardName
            };
            db.Set<DemandNoticeTaxpayers>().Add(d);//sp_addDemandNoticeTaxpayer

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = dnt.Id,
                description = $"Demand Notice has been creadted for {dnt.TaxpayersName} for the year {dnt.BillingYr}"
            };

        }

        public async Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel)
        {
            //List<DemandNoticeTaxpayers> results = await db.Set<DemandNoticeTaxpayers>().FromSql("sp_getDemandNoticeTaxpayerByBatchNumber " +
            //    "@p0,@p1, @p2",
            //    new object[] { batchId, pageModel.PageNum,
            //        pageModel.PageSize }).ToListAsync();
            //var totalCount = 0;
            //if (results.Count > 0)
            //{
            //    DemandNoticeTaxpayers companyItemExt = results[0];
            //    totalCount = 0;// companyItemExt.totalSize.Value;
            //}

            var query = db.Set<DemandNoticeTaxpayers>().Include(x => x.DemandNotice)
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
                .Skip(pageModel.PageNum * pageModel.PageSize).Take(pageModel.PageSize).ToArrayAsync();

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

            var result = await db.Set<DemandNoticeTaxpayers>().Include(x => x.DemandNotice)
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

        public async Task<DemandNoticeTaxpayersModel> ByBillingNo(string billingNo)
        {
            //string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize from tbl_demandNoticeTaxpayers where billingNumber = '{billingNo}'";
            //DemandNoticeTaxpayers x = await db.Set<DemandNoticeTaxpayers>()
            //    .FromSql(query,
            //    new object[] { billingNo }).FirstOrDefaultAsync();
            //if (x == null)
            //{
            //    return null;
            //}

            var result = await db.Set<DemandNoticeTaxpayers>().Select(x => new DemandNoticeTaxpayersModel()
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

            var result = await db.Set<DemandNoticeTaxpayers>().Include(x => x.DemandNotice)
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
            //var result = await db.Set<DemandNoticeTaxpayers>().FromSql("sp_getDNDemandNoticeByBatchNo @p0",
            //  new object[] { batchno }).ToListAsync();

            return await db.Set<DemandNoticeTaxpayers>().Select(x => new DemandNoticeTaxpayersModel()
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
        }

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo(string billingNo, string createdBy)
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

            var results = await db.Set<DemandNoticeTaxpayers>().FromSql(query).ToListAsync();

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

            var result = await db.Set<DemandNoticeTaxpayers>().FromSql(query).ToArrayAsync();
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

            var result = await db.Set<DemandNoticeTaxpayers>().FromSql(query).ToArrayAsync();
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

            var result = await db.Set<DemandNoticeTaxpayers>()
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

        public async Task<bool> MoveToBills(string billno)
        {
            string query = $"Update tbl_demandNoticeTaxpayers set isUnbilled = 0 where billingNumber ='{billno}'";
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            return count > 0;
        }

        public async Task<bool> MoveToUnBills(string billno)
        {
            string query = $"Update tbl_demandNoticeTaxpayers set isUnbilled = 1 where billingNumber ='{billno}'";
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            return count > 0;
        }

        public async Task<PageModel<DemandNoticeTaxpayersModel[]>> Search(DemandNoticeRequestModel rhModel, 
            PageModel pageModel)
        {
            var query = db.Set<DemandNoticeTaxpayers>()
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
                query = query.Where(x => EF.Functions.Like(x.TaxpayersName, $"%{rhModel.searchByName}%"));
                //pageModel.PageNum = 1;
                //pageModel.PageSize = 20;
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
                StreetName = x.DemandNotice.Street.StreetName
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

        public async Task<PageModel<DemandNoticeTaxpayersModel[]>> SearchByLcdaId(DemandNoticeRequestModel rhModel, 
            PageModel pageModel, Guid lcdaId)
        {
            var query = db.Set<DemandNoticeTaxpayers>()
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
                StreetName = x.DemandNotice.Street.StreetName
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

    }
}

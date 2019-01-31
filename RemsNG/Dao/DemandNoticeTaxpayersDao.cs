using Microsoft.EntityFrameworkCore;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DemandNoticeTaxpayersDao : AbstractDao
    {
        private readonly ErrorDao errorDao;
        public DemandNoticeTaxpayersDao(RemsDbContext _db) : base(_db)
        {
            errorDao = new ErrorDao(_db);
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> getTaxpayerByIds(string[] ids, int billingYr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string tIds = stringBuilder.AppendJoin(',', ids).ToString();

            string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize from tbl_demandNoticeTaxpayers" +
                $" where taxpayerId in ({tIds}) and billingYr = {billingYr} and demandNoticeStatus not in ('CLOSED','CANCEL')";
            return await db.Set<DemandNoticeTaxpayersDetail>().FromSql(query).ToListAsync();
        }

        public async Task<bool> UpdateTaxPayer(Guid id, string status)
        {
            string query = $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = '{status}' where id='{id}'";

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
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
                dbResponse = await db.DbResponses.FromSql("sp_currentMaxBilling").FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return dbResponse == null ? 0 : long.Parse(dbResponse.msg);
        }

        public async Task<Response> Add(DemandNoticeTaxpayersDetail dnt)
        {
            try
            {
                long billnumber = await NewBillingNumber();
                dnt.billingNumber = (billnumber + 1).ToString();
                DbResponse dbResponse = await db.DbResponses.FromSql("sp_addDemandNoticeTaxpayer @p0,@p1,@p2,@p3,@p4,@p5,@p6," +
                    "@p7,@p8,@p9,@p10,@p11,@p12", new object[]{
                dnt.dnId,
                dnt.billingYr,
                dnt.createdBy,
                dnt.taxpayerId,
                dnt.domainName,
                dnt.lcdaAddress == null?string.Empty:dnt.lcdaAddress,
                dnt.lcdaState == null?string.Empty:dnt.lcdaState,
                dnt.lcdaLogoFileName == null?string.Empty:dnt.lcdaLogoFileName,
                dnt.councilTreasurerSigFilen== null?string.Empty:dnt.councilTreasurerSigFilen,
                dnt.revCoodinatorSigFilen == null?string.Empty:dnt.revCoodinatorSigFilen,
                dnt.councilTreasurerMobile == null?string.Empty:dnt.councilTreasurerMobile,
                dnt.lcdaName,
                $"{(billnumber+1)}"
            }).FirstOrDefaultAsync();

                if (dbResponse.success)
                {
                    return new Response()
                    {
                        code = MsgCode_Enum.SUCCESS,
                        data = dbResponse.msg,
                        description = "Taxpayer has been successfully added"
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

        public async Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel)
        {
            List<DemandNoticeTaxpayersDetail> results = await db.DemandNoticeTaxpayersDetails.FromSql("sp_getDemandNoticeTaxpayerByBatchNumber " +
                "@p0,@p1, @p2",
                new object[] { batchId, pageModel.PageNum,
                    pageModel.PageSize }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                DemandNoticeTaxpayersDetail companyItemExt = results[0];
                totalCount = companyItemExt.totalSize.Value;
            }

            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> GetDNTaxpayerByBatchIdAsync(string batchId)
        {
            List<DemandNoticeTaxpayersDetail> results = await db.DemandNoticeTaxpayersDetails
                .FromSql("sp_getDnTByBatchNumber @p0",
                new object[] { batchId }).ToListAsync();
            return results;
        }

        public async Task<DemandNoticeTaxpayersDetail> ByBillingNo(string billingNo)
        {
            string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize from tbl_demandNoticeTaxpayers where billingNumber = '{billingNo}'";
            DemandNoticeTaxpayersDetail results = await db.DemandNoticeTaxpayersDetails
                .FromSql(query,
                new object[] { billingNo }).FirstOrDefaultAsync();
            return results;
        }

        public async Task<DemandNoticeTaxpayersDetail> GetSingleTaxpayerAsync(string taxpayerId, int billingYr)
        {
            return await db.DemandNoticeTaxpayersDetails.FromSql("sp_getDemandNoticeTaxpayerByYear @p0,@p1",
              new object[] { taxpayerId, billingYr }).FirstOrDefaultAsync();
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> GetDNTaxpayerByBatchNoAsync(string batchno)
        {
            return await db.DemandNoticeTaxpayersDetails.FromSql("sp_getDNDemandNoticeByBatchNo @p0",
              new object[] { batchno }).ToListAsync();
        }

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo1(string billingNo, string createdBy)
        {
            string query = $"delete from tbl_demandNoticeArrears where billingNo = '{billingNo}'; ";
            query = query + $"delete from tbl_demandNoticeItem where billingNo ='{billingNo}';";
            query = query + $"delete from tbl_demandNoticePenalty where billingNo = '{billingNo}';";
            query = query + $"delete from tbl_demandNoticeTaxpayers where billingNumber = '{billingNo}';";

            int count = await db.Database.ExecuteSqlCommandAsync(query);

            if (count > 0)
            {
                //log
                Error error = new Error()
                {
                    dateCreated = DateTime.Now,
                    errorType = "Delete Demand Notice",
                    errorvalue = $"{billingNo}, created by {createdBy}",
                    id = Guid.NewGuid(),
                    ownerId = Guid.NewGuid()
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
                Error error = new Error()
                {
                    dateCreated = DateTime.Now,
                    errorType = "Delete Demand Notice",
                    errorvalue = $"{billingNo}, created by {createdBy}",
                    id = Guid.NewGuid(),
                    ownerId = Guid.NewGuid()
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

        public async Task<List<DemandNoticeTaxpayersDetail>> SearchAllAsync(string qs)
        {
            string[] q = qs.Split(new char[] { ' ' });
            string query = $"select tbl_demandNoticeTaxpayers.*,-1 as totalSize from tbl_demandNoticeTaxpayers where ";
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

            var results = await db.DemandNoticeTaxpayersDetails.FromSql(query).ToListAsync();

            return results.Distinct().ToList();
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

        public async Task<DemandNoticeTaxpayersDetail[]> GetAllReceivables()
        {
            string query = $"select dnt.*,-1 as totalSize  from tbl_demandNoticeTaxpayers as dnt " +
                $"where dnt.taxpayerId not in (select taxpayerId from tbl_demandNoticePenalty where itemPenaltyStatus != 'PAID') " +
                $"and dnt.demandNoticeStatus in ('PENDING','PART_PAYMENT')";

            var results = await db.DemandNoticeTaxpayersDetails.FromSql(query).ToArrayAsync();
            return results;
        }

        public async Task<DemandNoticeTaxpayersDetail[]> GetAllReceivables(Guid[] taxpayerIds)
        {
            string ids = taxpayerIds.Select(x => x.ToString()).ToArray().FormatString();

            string query = $"select dnt.*,-1 as totalSize  from tbl_demandNoticeTaxpayers as dnt " +
                $"where dnt.taxpayerId not in " +
                $"(select taxpayerId from tbl_demandNoticePenalty where itemPenaltyStatus != 'PAID' and taxpayerId in ({ids}) ) " +
                $"and dnt.demandNoticeStatus in ('PENDING','PART_PAYMENT') and taxpayerId in ({ids}) and dnt.billingYr = {DateTime.Now.Year - 1}";

            var results = await db.DemandNoticeTaxpayersDetails.FromSql(query).ToArrayAsync();
            return results;
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> GetTaxpayerPayables(Guid taxpayerId)
        {
            string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize from tbl_demandNoticeTaxpayers " +
                $"where taxpayerId = '{taxpayerId}' and demandNoticeStatus in ('PART_PAYMENT','PENDING','PAID','CLOSED') order by  dateCreated";
            var results = await db.DemandNoticeTaxpayersDetails
                .FromSql(query,
                new object[] { taxpayerId.ToString() }).ToListAsync();
            return results;
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

        public async Task<List<DemandNoticeTaxpayersDetail>> GetAllUnbilledDemandNotice(string batchno)
        {
            string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize  " +
                $"from tbl_demandNoticeTaxpayers " +
                $"inner join tbl_taxPayer on tbl_taxPayer.id = tbl_demandNoticeTaxpayers.taxpayerId " +
                $"inner join tbl_street on  tbl_street.wardId = tbl_taxPayer.streetId " +
                $"inner join tbl_ward on tbl_ward.id = tbl_street.wardId " +
                $"where isUnbilled = 1";

            return await db.DemandNoticeTaxpayersDetails.FromSql(query).ToListAsync();
        }
    }
}

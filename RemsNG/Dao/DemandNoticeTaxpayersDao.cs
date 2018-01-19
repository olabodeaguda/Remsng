using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;
using RemsNG.Utilities;
using System.Text;

namespace RemsNG.Dao
{
    public class DemandNoticeTaxpayersDao : AbstractDao
    {
        public DemandNoticeTaxpayersDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<DemandNoticeTaxpayersDetail>> getTaxpayerByIds(string[] ids, int billingYr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string tIds = stringBuilder.AppendJoin(',', ids).ToString();

            string query = $"select tbl_demandNoticeTaxpayers.*, -1 as totalSize from tbl_demandNoticeTaxpayers where taxpayerId in ({tIds}) and billingYr = {billingYr}";
            return await db.Set<DemandNoticeTaxpayersDetail>().FromSql(query).ToListAsync();
        }

        public async Task<Response> Add(DemandNoticeTaxpayersDetail dnt)
        {
            try
            {
                DbResponse dbResponse = await db.DbResponses.FromSql("sp_addDemandNoticeTaxpayer @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11", new object[]{
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
                dnt.lcdaName
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
            catch (Exception X)
            {
                throw;
            }
        }

        public async Task<object> GetDNTaxpayerByBatchIdAsync(string batchId, PageModel pageModel)
        {
            List<DemandNoticeTaxpayersDetail> results = await db.DemandNoticeTaxpayersDetails.FromSql("sp_getDemandNoticeTaxpayerByBatchNumber @p0,@p1, @p2",
                new object[] { batchId, pageModel.PageNum, pageModel.PageSize }).ToListAsync();
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

        public async Task<Response> CancelTaxpayerDemandNoticeByBillingNo(string billingNo)
        {
            string query = $"delete from tbl_demandNoticeArrears where billingNo = '{billingNo}'; ";
            query = query + $"delete from tbl_demandNoticeItem where billingNo ='{billingNo}';";
            query = query + $"delete from tbl_demandNoticePenalty where billingNo = '{billingNo}';";
            query = query + $"delete from tbl_demandNoticeTaxpayers where billingNumber = '{billingNo}';";

            int count = await db.Database.ExecuteSqlCommandAsync(query);

            if (count > 0)
            {
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
    }
}

using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class TaxpayerDao : AbstractDao
    {
        public TaxpayerDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Create(Taxpayer taxpayer, bool confirmCompany)
        {
            var r = await Get(taxpayer.streetId, taxpayer.companyId);

            if (r != null && confirmCompany)
            {
                throw new DuplicateCompanyException("Company already exist on the street");
            }

            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addTaxpayer @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", new object[] {
                    taxpayer.id,
                    taxpayer.companyId,
                    taxpayer.streetId,
                    taxpayer.addressId == null?Guid.Empty:taxpayer.addressId,
                    taxpayer.createdBy,
                    taxpayer.surname,
                    taxpayer.firstname,
                    taxpayer.lastname
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Tax Payer has been added successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur when creating the taxpayer. Please try again or inform your admnistrator for assitance"
                };
            }
        }

        public async Task<object> Get(Guid streetId, Guid companyId)
        {
            string query = $"select tbl_taxPayer.*,'-1' as streetNumber from tbl_taxPayer where streetId = '{streetId}' and companyId='{companyId}'";
            return await db.Taxpayers.FromSql(query).FirstOrDefaultAsync();
        }

        public async Task<List<Taxpayer>> GetActiveTaxpayers(DemandNoticeRequest demandNoticeRequest)
        {
            if (demandNoticeRequest.streetId != Guid.Empty && demandNoticeRequest.streetId != null)
            {
                return await db.Taxpayers.FromSql($"select tbl_taxPayer.*,'-1' as streetNumber from tbl_taxPayer " +
                    $"where streetId = '{demandNoticeRequest.streetId}' and taxpayerStatus='ACTIVE'").ToListAsync();
            }
            else if (demandNoticeRequest.wardId != Guid.Empty && demandNoticeRequest.wardId != null)
            {
                string query = $"select tbl_taxPayer.*,'-1' as streetNumber from tbl_taxPayer " +
                    $"inner join tbl_street on tbl_taxPayer.streetId = tbl_street.id where tbl_street.wardId = '{demandNoticeRequest.wardId}' and taxpayerStatus='ACTIVE'";
                return await db.Taxpayers.FromSql(query).ToListAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task<TaxpayerExtension> ById(Guid id)
        {
            var result = await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerById @p0", new object[] { id }).FirstOrDefaultAsync();
            if (result == null || result.taxpayerStatus == TaxpayerEnum.DELETED.ToString())
            {
                return null;
            }

            return result;
        }

        public async Task<List<TaxpayerExtension>> ByStreetId(Guid streetId)
        {
            var result = await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByStreetId @p0", new object[] { streetId }).ToListAsync();
            result = result.Where(x => x.taxpayerStatus == TaxpayerEnum.ACTIVE.ToString()).ToList();
            return result;
        }

        public async Task<object> ByStreetId(Guid streetId, PageModel pageModel)
        {
            var results = await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByStreetIdPaginated @p0,@p1,@p2", new object[] { streetId, pageModel.PageSize, pageModel.PageNum }).ToListAsync();
            int totalCount = 0;
            if (results.Count > 0)
            {
                totalCount = results[0].totalSize;
            }

            return new
            {
                data = results.Where(x => x.taxpayerStatus == TaxpayerEnum.ACTIVE.ToString()).ToList(),
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<List<TaxpayerExtension>> ByCompanyId(Guid companyId)
        {
            var results = await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByCompanyId @p0", new object[] { companyId }).ToListAsync();
            results = results.Where(x => x.taxpayerStatus == TaxpayerEnum.ACTIVE.ToString()).ToList();
            return results;
        }

        public async Task<Response> Update(Taxpayer taxpayer)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_updateTaxpayer @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", new object[] {
                    taxpayer.id,
                    taxpayer.companyId,
                    taxpayer.streetId,
                    taxpayer.addressId,
                    taxpayer.lastmodifiedby,
                    taxpayer.surname,
                    taxpayer.firstname,
                    taxpayer.lastname
            }).FirstOrDefaultAsync();
            Response response = new Response();
            response.description = dbResponse.msg;

            if (dbResponse.success)
            {
                response.code = MsgCode_Enum.SUCCESS;
            }
            else
            {
                response.code = MsgCode_Enum.FAIL;
            }

            return response;
        }

        public async Task<List<TaxpayerExtension>> ByLcdaId(Guid lcdaId)
        {
            var results = await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByLcdaId @p0", new object[] { lcdaId }).ToListAsync();
            results = results.Where(x => x.taxpayerStatus == TaxpayerEnum.ACTIVE.ToString()).ToList();
            return results;
        }

        public async Task<List<TaxpayerExtension>> Search(Guid lcdaId, string qu)
        {
            string[] q = qu.Split(new char[] { ' ' });

            string query = $"select tbl_taxPayer.*,tbl_company.companyName as companyName, tbl_Address.addressnumber as streetNumber,-1 as totalSize  from tbl_taxPayer " +
               $"inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId " +
               $"inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId " +
               $"inner join tbl_ward on tbl_ward.id = tbl_street.wardId " +
               $"inner join tbl_address on tbl_address.ownerId = tbl_taxPayer.id " +
               $"where tbl_ward.lcdaId = '{lcdaId}' " +
               $"and (tbl_taxPayer.surname like '%{qu}%' or tbl_taxPayer.firstname like '%{qu}%' " +
               $"or tbl_taxPayer.lastname like '%{qu}%' " +
               $" or tbl_company.companyName like '%{qu}%')";

            string subQuery = string.Empty;

            for (int i = 0; i < q.Length; i++)
            {
                if (string.IsNullOrEmpty(q[i].Trim()))
                {
                    continue;
                }
                //if (query != string.Empty && i < q.Length)
                //{
                //    query = query + $" union ";
                //}
                string t = $" (tbl_taxPayer.surname like '%{q[i]}%' or tbl_taxPayer.firstname like '%{q[i]}%' " +
                $"or tbl_taxPayer.lastname like '%{q[i]}%' " +
                $" or tbl_company.companyName like '%{q[i]}%')";

                if (!string.IsNullOrEmpty(subQuery))
                {
                    subQuery = subQuery + " and ";
                }
                subQuery = subQuery + t;
            }

            query = query + (string.IsNullOrEmpty(subQuery) ? "" : " or ") + (string.IsNullOrEmpty(subQuery) ? "" : $" ( {subQuery} ) ");

            var results = await db.TaxpayerExtensions.FromSql(query).ToListAsync();
            return results.Distinct().Where(x => x.taxpayerStatus == TaxpayerEnum.ACTIVE.ToString()).OrderBy(x => x.firstname).ToList();
        }

        public async Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByLcdaIdpaginated @p0,@p1,@p2", new object[] { lcdaId, pageModel.PageSize, pageModel.PageNum }).ToListAsync();
            int totalCount = 0;
            if (results.Count > 0)
            {
                totalCount = results[0].totalSize;
            }

            return new
            {
                data = results.Where(x => x.taxpayerStatus == TaxpayerEnum.ACTIVE.ToString()).ToList(),
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<Lgda> getLcda(Guid taxpayerId)
        {
            string query = $"select distinct ld.* from tbl_lcda as ld ";
            query = query + $"inner join tbl_ward as wd on wd.lcdaId = ld.id ";
            query = query + $"inner join tbl_street as st on st.wardId = wd.id ";
            query = query + $"inner join tbl_taxPayer as tp on tp.streetId = st.id ";
            query = query + $"where tp.id= '{taxpayerId}'";

            return await db.lgdas.FromSql(query).FirstOrDefaultAsync();
        }

        public async Task<List<DemandNoticePaymentHistory>> PaymentHistory(Guid taxpayerId)
        {
            string query1 = $"select ndh.*,bank.bankName  from tbl_demandNoticePaymentHistory as ndh " +
                $"inner join tbl_bank bank on bank.id = ndh.bankId " +
                $"inner join tbl_demandNoticeTaxpayers as dnt on dnt.billingNumber = ndh.billingNumber " +
                $"where ndh.paymentStatus = 'APPROVED' and  dnt.taxpayerId = '{taxpayerId}' order by ndh.dateCreated desc";

            return await db.DemandNoticePaymentHistories.FromSql(query1).ToListAsync();
        }

        public async Task<int> UpdateStatus(Guid id, string status)
        {
            string query = $"update tbl_taxPayer set taxpayerStatus = '{status}' where id = '{id}'";
            return await db.Database.ExecuteSqlCommandAsync(query);
        }

        public async Task<List<TaxpayerExtension>> SearchInStreet(Guid streetid, string queryParams)
        {
            string[] str = queryParams.Split(new char[] { ' ' });
            string query = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                query = query + $"select tbl_taxPayer.*,tbl_company.companyName as companyName, " +
                  $" tbl_Address.addressnumber as streetNumber,0 as totalSize from tbl_taxPayer " +
                  $"inner join tbl_address on tbl_address.ownerId = tbl_taxPayer.id " +
                  $"inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId " +
                  $"inner join tbl_street on tbl_street.id = tbl_taxPayer.streetId " +
                  $"where (tbl_taxPayer.firstname like '%{str[i]}%' or tbl_taxPayer.lastname like '%{str[i]}%' " +
                  $"or tbl_taxPayer.surname like '%{str[i]}%') and tbl_street.id = '{streetid}' ";

                if (i < (str.Length - 1))
                {
                    query = query + " union ";
                }
            }
            var results = await db.TaxpayerExtensions.FromSql(query).ToListAsync();

            var re = results
                .GroupBy(x => x.id)
                .Select(p => p.FirstOrDefault())
                .ToList();

            return re;
        }
    }
}

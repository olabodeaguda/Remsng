using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addTaxpayer @p0,@p1,@p2,@p3,@p4", new object[] {
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
            return await db.Taxpayers.FromSql($"select tbl_taxPayer.*,'-1' as streetNumber from tbl_taxPayer where streetId = '{streetId}' and companyId='{companyId}'").FirstOrDefaultAsync();
        }

        public async Task<List<Taxpayer>> Get(DemandNoticeRequest demandNoticeRequest)
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
            return await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerById @p0", new object[] { id }).FirstOrDefaultAsync();
        }

        public async Task<List<TaxpayerExtension>> ByStreetId(Guid streetId)
        {
            return await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByStreetId @p0", new object[] { streetId }).ToListAsync();
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
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<List<TaxpayerExtension>> ByCompanyId(Guid companyId)
        {
            return await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByCompanyId @p0", new object[] { companyId }).ToListAsync();
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
            return await db.Set<TaxpayerExtension>().FromSql("sp_TaxpayerByLcdaId @p0", new object[] { lcdaId }).ToListAsync();
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
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

       
    }
}

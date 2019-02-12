using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class CompanyItemRepository : AbstractRepository
    {
        public CompanyItemRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(CompanyItemModel companyItem)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_createCompanyItem @p0, @p1,@p2,@p3,@p4,@p5,@p6", new object[] {
                companyItem.Id,
                companyItem.TaxpayerId,
                companyItem.ItemId,
                companyItem.Amount,
                companyItem.BillingYear,
                companyItem.CreatedBy,
                companyItem.CompanyStatus
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
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

        public async Task<Response> Update(CompanyItemModel companyItem)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateCompanyItem @p0, @p1,@p2,@p3,@p4,@p5", new object[] {
                companyItem.Id,
                companyItem.TaxpayerId,
                companyItem.ItemId,
                companyItem.Amount,
                companyItem.BillingYear,
                companyItem.CreatedBy
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
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

        public async Task<Response> UpdateStatus(Guid id, string companystatus)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateCompanyItemStatus @p0, @p1", new object[] {
                id,
                companystatus
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
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

        public async Task<List<CompanyItemModelExt>> ByTaxpayer(Guid taxpayerId)
        {
            return await db.companyItemExts.FromSql("sp_companyItemByTaxpayerId @p0", new object[] { taxpayerId }).ToListAsync();
        }

        public async Task<CompanyItemModelExt> ById(Guid id)
        {
            return await db.companyItemExts.FromSql("sp_companyItemById @p0", new object[] { id }).FirstOrDefaultAsync();
        }

        public async Task<object> ByTaxpayerpaginated(Guid id, PageModel pageModel)
        {
            List<CompanyItemModelExt> results = await db.companyItemExts.FromSql("sp_companyItemByTaxpayerIdPaginated @p0,@p1, @p2", new object[] { id, pageModel.PageNum, pageModel.PageSize }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                CompanyItemModelExt companyItemExt = results[0];
                totalCount = companyItemExt.totalSize;
            }

            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }
    }
}

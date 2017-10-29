using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;
using RemsNG.Utilities;

namespace RemsNG.Dao
{
    public class CompanyItemDao : AbstractDao
    {
        public CompanyItemDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(CompanyItem companyItem)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_createCompanyItem @p0, @p1,@p2,@p3,@p4,@p5,@p6", new object[] {
                companyItem.id,
                companyItem.taxpayerId,
                companyItem.itemId,
                companyItem.amount,
                companyItem.billingYear,
                companyItem.createdBy,
                companyItem.companyStatus
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

        public async Task<Response> Update(CompanyItem companyItem)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateCompanyItem @p0, @p1,@p2,@p3,@p4,@p5", new object[] {
                companyItem.id,
                companyItem.taxpayerId,
                companyItem.itemId,
                companyItem.amount,
                companyItem.billingYear,
                companyItem.createdBy
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

        public async Task<List<CompanyItemExt>> ByTaxpayer(Guid taxpayerId)
        {
            return await db.companyItemExts.FromSql("sp_companyItemByTaxpayerId @p0", new object[] { taxpayerId }).ToListAsync();
        }

        public async Task<CompanyItemExt> ById(Guid id)
        {
            return await db.companyItemExts.FromSql("sp_companyItemById @p0", new object[] { id }).FirstOrDefaultAsync();
        }

        public async Task<object> ByTaxpayerpaginated(Guid id, PageModel pageModel)
        {
            List<CompanyItemExt> results = await db.companyItemExts.FromSql("sp_companyItemByTaxpayerIdPaginated @p0,@p1, @p2", new object[] { id, pageModel.PageNum, pageModel.PageSize }).ToListAsync();
            var totalCount = 0;
            if (results.Count > 0)
            {
                CompanyItemExt companyItemExt = results[0];
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

using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
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
    public class TaxpayerCatgoryRepository : AbstractRepository
    {
        public TaxpayerCatgoryRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(TaxpayerCategory taxpayerCategory)
        {
            db.TaxPayersCategories.Add(taxpayerCategory);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Category have been added successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request failed. Please try again or contact an administrator"
                };
            }
        }

        public async Task<Response> Update(TaxpayerCategory taxpayerCategory)
        {
            var category = await db.TaxPayersCategories.FindAsync(new object[] { taxpayerCategory.Id });
            if (category == null)
            {
                throw new NotFoundException("Taxpayer category does not exist");
            }

            category.TaxpayerCategoryName = taxpayerCategory.TaxpayerCategoryName;
            category.Lastmodifiedby = taxpayerCategory.Lastmodifiedby;
            category.LastModifiedDate = DateTime.Now;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request failed. Please try again or contact an administrator"
                };
            }

        }

        public async Task<Response> Delete(Guid id)
        {
            var category = await db.TaxPayersCategories.FindAsync(new object[] { id });
            if (category == null)
            {
                throw new NotFoundException("Taxpayer category does not exist");
            }

            db.TaxPayersCategories.Remove(category);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Remove was successful"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request failed. Please try again or contact an administrator"
                };
            }

        }

        public async Task<TaxpayerCategory> GetById(Guid id)
        {
            return await db.TaxPayersCategories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<TaxpayerCategory>> GetListByLcdaIdAsync(Guid lcdaId)
        {
            return await db.TaxPayersCategories.Where(x => x.LcdaId == lcdaId).ToListAsync();
        }

        public async Task<object> GetListByLcdaIdAsync(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.TaxPayersCategories.Where(x => x.LcdaId == lcdaId).Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.TaxPayersCategories.Where(x => x.LcdaId == lcdaId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };

        }

        public async Task<object> GetByNameAndLcdaId(Guid lcdaid, string name)
        {
            return await db.TaxPayersCategories.FirstOrDefaultAsync(x => x.LcdaId == lcdaid && x.TaxpayerCategoryName.ToLower() == name);
        }

        public async Task<TaxpayerCategory> GetTaxpayerCategory(Guid taxpayerId)
        {
            string query = $"select tbl_taxpayerCategory.* from tbl_taxPayer " +
                $"inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId " +
                $"inner join tbl_taxpayerCategory on tbl_taxpayerCategory.id = tbl_company.categoryId " +
                $"where tbl_taxPayer.id='{taxpayerId}' ";

            var result = await db.TaxPayersCategories.FromSql(query).FirstOrDefaultAsync();
            return result;
        }
    }
}

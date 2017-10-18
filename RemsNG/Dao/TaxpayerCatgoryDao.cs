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
    public class TaxpayerCatgoryDao : AbstractDao
    {
        public TaxpayerCatgoryDao(RemsDbContext _db) : base(_db)
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
            var category = await db.TaxPayersCategories.FindAsync(new object[] { taxpayerCategory.id });
            if (category == null)
            {
                throw new NotFoundException("Taxpayer category does not exist");
            }

            category.taxpayerCategoryName = taxpayerCategory.taxpayerCategoryName;
            category.lastmodifiedby = taxpayerCategory.lastmodifiedby;
            category.lastModifiedDate = DateTime.Now;

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
            return await db.TaxPayersCategories.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<List<TaxpayerCategory>> GetListByLcdaIdAsync(Guid lcdaId)
        {
            return await db.TaxPayersCategories.Where(x => x.lcdaId == lcdaId).ToListAsync();
        }

        public async Task<object> GetListByLcdaIdAsync(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.TaxPayersCategories.Where(x => x.lcdaId == lcdaId).Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.TaxPayersCategories.Where(x => x.lcdaId == lcdaId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };

        }

        public async Task<object> GetByNameAndLcdaId(Guid lcdaid, string name) =>
            await db.TaxPayersCategories.FirstOrDefaultAsync(x => x.lcdaId == lcdaid && x.taxpayerCategoryName.ToLower() == name);
    }
}

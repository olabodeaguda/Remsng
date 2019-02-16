using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class TaxpayerCatgoryRepository : AbstractRepository
    {
        public TaxpayerCatgoryRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(TaxpayerCategoryModel taxpayerCategory)
        {
            db.TaxPayersCategories.Add(new TaxpayerCategory
            {
                CreatedBy = taxpayerCategory.CreatedBy,
                DateCreated = taxpayerCategory.DateCreated,
                Id = taxpayerCategory.Id,
                Lastmodifiedby = taxpayerCategory.Lastmodifiedby,
                LastModifiedDate = taxpayerCategory.LastModifiedDate,
                LcdaId = taxpayerCategory.LcdaId,
                TaxpayerCategoryName = taxpayerCategory.TaxpayerCategoryName
            });
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

        public async Task<Response> Update(TaxpayerCategoryModel taxpayerCategory)
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

        public async Task<TaxpayerCategoryModel> GetById(Guid id)
        {
            var t = await db.TaxPayersCategories.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null)
            {
                return null;
            }

            return new TaxpayerCategoryModel()
            {
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                Id = t.Id,
                Lastmodifiedby = t.Lastmodifiedby,
                LastModifiedDate = t.LastModifiedDate,
                LcdaId = t.LcdaId,
                TaxpayerCategoryName = t.TaxpayerCategoryName
            };
        }

        public async Task<List<TaxpayerCategoryModel>> GetListByLcdaIdAsync(Guid lcdaId)
        {
            var result = await db.TaxPayersCategories.Where(x => x.LcdaId == lcdaId).ToListAsync();
            return result.Select(t => new TaxpayerCategoryModel()
            {
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                Id = t.Id,
                Lastmodifiedby = t.Lastmodifiedby,
                LastModifiedDate = t.LastModifiedDate,
                LcdaId = t.LcdaId,
                TaxpayerCategoryName = t.TaxpayerCategoryName
            }).ToList();
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

        public async Task<TaxpayerCategoryModel> GetTaxpayerCategory(Guid taxpayerId)
        {
            string query = $"select tbl_taxpayerCategory.* from tbl_taxPayer " +
                $"inner join tbl_company on tbl_company.id = tbl_taxPayer.companyId " +
                $"inner join tbl_taxpayerCategory on tbl_taxpayerCategory.id = tbl_company.categoryId " +
                $"where tbl_taxPayer.id='{taxpayerId}' ";

            var t = await db.TaxPayersCategories.FromSql(query).FirstOrDefaultAsync();
            if (t == null)
            {
                return null;
            }

            return new TaxpayerCategoryModel()
            {
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                Id = t.Id,
                Lastmodifiedby = t.Lastmodifiedby,
                LastModifiedDate = t.LastModifiedDate,
                LcdaId = t.LcdaId,
                TaxpayerCategoryName = t.TaxpayerCategoryName
            };
        }
    }
}

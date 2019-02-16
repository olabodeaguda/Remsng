using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class ItemRepository : AbstractRepository
    {
        public ItemRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(ItemModel item)
        {
            var r = await db.Set<Item>().FirstOrDefaultAsync(x => x.ItemDescription.ToLower() == item.ItemDescription.ToLower()
            && item.LcdaId == x.LcdaId);
            if (r != null)
            {

                throw new DuplicateException($"{item.ItemDescription} already exist");
            }

            item.DateCreated = DateTime.Now;
            db.Set<Item>().Add(new Item()
            {
                CreatedBy = item.CreatedBy,
                DateCreated = item.DateCreated,
                Id = item.Id,
                ItemCode = item.ItemCode,
                ItemDescription = item.ItemDescription,
                ItemStatus = item.ItemStatus,
                Lastmodifiedby = item.Lastmodifiedby,
                LastModifiedDate = item.LastModifiedDate,
                LcdaId = item.LcdaId
            });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{item.ItemDescription} has been addedd successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{item.ItemDescription} request failed. Please try again or contact an administrator for help"
                };
            }
        }

        public async Task<Response> Update(ItemModel item)
        {
            var r = await db.Set<Item>().FindAsync(new object[] { item.Id });
            if (r == null)
            {
                throw new NotFoundException($"{item.ItemDescription} does not exist");
            }

            r.ItemDescription = item.ItemDescription;
            r.Lastmodifiedby = item.Lastmodifiedby;
            r.LastModifiedDate = DateTime.Now;
            r.ItemCode = item.ItemCode;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{item.ItemDescription} has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while updating {item.ItemDescription}. Please try again or contact administrator"
                };
            }
        }

        public async Task<Response> UpdateStatus(ItemModel item)
        {
            var r = await db.Set<Item>()
                .FindAsync(new object[] { item.Id });
            if (r == null)
            {
                throw new NotFoundException($"{item.ItemDescription} does not exist");
            }

            r.ItemStatus = item.ItemStatus;
            r.Lastmodifiedby = item.Lastmodifiedby;
            r.LastModifiedDate = DateTime.Now;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{item.ItemDescription} has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while updating {item.ItemDescription}. Please try again or contact administrator"
                };
            }
        }

        public async Task<object> ListByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.Set<Item>().Where(x => x.LcdaId == lcdaId).Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.Set<Item>()
                .Where(x => x.LcdaId == lcdaId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };

        }

        public async Task<object> ListByLcdaId(Guid lcdaId)
        {
            return await db.Set<Item>()
                .Where(x => x.LcdaId == lcdaId).ToListAsync();
        }

        public async Task<object> GetItemByIdAsync(Guid id)
        {
            return await db.Set<Item>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<object> GetByTaxPayersId(Guid taxpayersId)
        {
            return await db.Set<Item>()
                .FromSql("sp_itemByTaxpayersid @p0", new object[] { taxpayersId }).ToListAsync();
        }
    }
}

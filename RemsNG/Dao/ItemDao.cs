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
    public class ItemDao : AbstractRepository
    {
        public ItemDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(Item item)
        {
            var r = await db.Items.FirstOrDefaultAsync(x => x.itemDescription.ToLower() == item.itemDescription.ToLower()
            && item.lcdaId == x.lcdaId);
            if (r != null)
            {

                throw new DuplicateException($"{item.itemDescription} already exist");
            }

            item.dateCreated = DateTime.Now;
            db.Items.Add(item);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{item.itemDescription} has been addedd successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"{item.itemDescription} request failed. Please try again or contact an administrator for help"
                };
            }
        }

        public async Task<Response> Update(Item item)
        {
            var r = await db.Items.FindAsync(new object[] { item.id });
            if (r == null)
            {
                throw new NotFoundException($"{item.itemDescription} does not exist");
            }

            r.itemDescription = item.itemDescription;
            r.lastmodifiedby = item.lastmodifiedby;
            r.lastModifiedDate = DateTime.Now;
            r.itemCode = item.itemCode;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{item.itemDescription} has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while updating {item.itemDescription}. Please try again or contact administrator"
                };
            }
        }

        public async Task<Response> UpdateStatus(Item item)
        {
            var r = await db.Items.FindAsync(new object[] { item.id });
            if (r == null)
            {
                throw new NotFoundException($"{item.itemDescription} does not exist");
            }

            r.itemStatus = item.itemStatus;
            r.lastmodifiedby = item.lastmodifiedby;
            r.lastModifiedDate = DateTime.Now;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{item.itemDescription} has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while updating {item.itemDescription}. Please try again or contact administrator"
                };
            }
        }

        public async Task<object> ListByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.Items.Where(x => x.lcdaId == lcdaId).Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.Items.Where(x => x.lcdaId == lcdaId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };

        }

        public async Task<object> ListByLcdaId(Guid lcdaId)
        {
            return await db.Items.Where(x => x.lcdaId == lcdaId).ToListAsync();
        }

        public async Task<object> GetItemByIdAsync(Guid id)
        {
            return await db.Items.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<object> GetByTaxPayersId(Guid taxpayersId)
        {
            return await db.Items.FromSql("sp_itemByTaxpayersid @p0", new object[] { taxpayersId }).ToListAsync();
        }
    }
}

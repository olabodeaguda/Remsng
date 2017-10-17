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
    public class ItemPenaltyDao : AbstractDao
    {
        public ItemPenaltyDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(ItemPenalty item)
        {
            var r = db.ItemPenalties.FirstOrDefaultAsync(x => x.duration == item.duration);
            if (r != null)
            {

                throw new DuplicateException($"Penalty for duration {item.duration} already exist");
            }

            item.dateCreated = DateTime.Now;
            db.ItemPenalties.Add(item);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"Penalty has been addedd successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"Request failed. Please try again or contact an administrator for help"
                };
            }
        }

        public async Task<Response> Update(ItemPenalty item)
        {
            var r = await db.ItemPenalties.FindAsync(new object[] { item.id });
            if (r == null)
            {
                throw new NotFoundException($"Penalty does not exist");
            }

            r.duration = item.duration;
            r.isPercentage = item.isPercentage;
            r.amount = item.amount;
            r.lastmodifiedby = item.lastmodifiedby;
            r.lastModifiedDate = DateTime.Now;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"Penalty has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while updating penalty. Please try again or contact administrator"
                };
            }
        }

        public async Task<Response> UpdateStatus(ItemPenalty item)
        {
            var r = await db.Items.FindAsync(new object[] { item.id });
            if (r == null)
            {
                throw new NotFoundException($"Penalty does not exist");
            }

            r.itemStatus = item.penaltyStatus;
            r.lastmodifiedby = item.lastmodifiedby;
            r.lastModifiedDate = DateTime.Now;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"Penalty has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while updating penalty. Please try again or contact administrator"
                };
            }
        }

        public async Task<object> GetById(Guid id) => await db.ItemPenalties.FirstOrDefaultAsync(x => x.id == id);

        public async Task<object> ListByItemId(Guid itemId) => await db.ItemPenalties.Where(x => x.itemId == itemId).ToListAsync();

        public async Task<object> ListByItemId(Guid itemId, PageModel pageModel)
        {
            var results = await db.ItemPenalties.Where(x => x.itemId == itemId).Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.ItemPenalties.Where(x=>x.itemId==itemId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };



        }

    }
}

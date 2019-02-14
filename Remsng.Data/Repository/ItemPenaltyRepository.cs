﻿using Microsoft.EntityFrameworkCore;
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
    public class ItemPenaltyRepository : AbstractRepository
    {
        public ItemPenaltyRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(ItemPenaltyModel item)
        {
            var r = await db.ItemPenalties.FirstOrDefaultAsync(x => x.Duration == item.Duration && item.ItemId == x.ItemId);
            if (r != null)
            {
                throw new DuplicateException($"Penalty for duration {item.Duration} already exist");
            }

            item.DateCreated = DateTime.Now;
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

        public async Task<Response> Update(ItemPenaltyModel item)
        {
            var r = await db.ItemPenalties.FindAsync(new object[] { item.Id });
            if (r == null)
            {
                throw new NotFoundException($"Penalty does not exist");
            }

            r.Duration = item.Duration;
            r.IsPercentage = item.IsPercentage;
            r.Amount = item.Amount;
            r.Lastmodifiedby = item.Lastmodifiedby;
            r.LastModifiedDate = DateTime.Now;

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

        public async Task<Response> UpdateStatus(ItemPenaltyModel item)
        {
            var r = await db.ItemPenalties.FindAsync(new object[] { item.Id });
            if (r == null)
            {
                throw new NotFoundException($"Penalty does not exist");
            }

            r.PenaltyStatus = item.PenaltyStatus;
            r.Lastmodifiedby = item.Lastmodifiedby;
            r.LastModifiedDate = DateTime.Now;

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

        public async Task<object> GetById(Guid id)
        {
            return await db.ItemPenalties.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<object> ListByItemId(Guid itemId)
        {
            return await db.ItemPenalties.Where(x => x.ItemId == itemId).ToListAsync();
        }

        public async Task<object> ListByItemId(Guid itemId, PageModel pageModel)
        {
            var results = await db.ItemPenalties.Where(x => x.ItemId == itemId).Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.ItemPenalties.Where(x => x.ItemId == itemId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };



        }

    }
}

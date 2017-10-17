using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class ItemPenaltyService : IItemPenaltyService
    {
        private ItemPenaltyDao itemPenaltyDao;
        public ItemPenaltyService(RemsDbContext _db)
        {
            itemPenaltyDao = new ItemPenaltyDao(_db);
        }

        public async Task<Response> Add(ItemPenalty item)
        {
            return await itemPenaltyDao.Add(item);
        }

        public async Task<object> GetById(Guid id)
        {
            return await itemPenaltyDao.GetById(id);
        }

        public async Task<object> ListByItemId(Guid itemId)
        {
            return await itemPenaltyDao.ListByItemId(itemId);
        }

        public async Task<object> ListByItemId(Guid itemId, PageModel pageModel)
        {
            return await itemPenaltyDao.ListByItemId(itemId, pageModel);
        }

        public async Task<Response> Update(ItemPenalty item)
        {
            return await itemPenaltyDao.Update(item);
        }

        public async Task<Response> UpdateStatus(ItemPenalty item)
        {
            return await itemPenaltyDao.UpdateStatus(item);
        }
    }
}

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
    public class ItemService : IItemService
    {
        private ItemDao itemDao;

        public ItemService(RemsDbContext remsDb)
        {
            itemDao = new ItemDao(remsDb);
        }

        public async Task<Response> Add(Item item)
        {
            return await itemDao.Add(item);
        }

        public async Task<object> GetByTaxPayersId(Guid taxpayersId)
        {
            return await itemDao.GetByTaxPayersId(taxpayersId);
        }

        public async Task<object> GetItemByIdAsync(Guid id)
        {
            return await itemDao.GetItemByIdAsync(id);
        }

        public async Task<object> ListByLcdaId(Guid lcdaId, PageModel pageModel)
        {
            return await itemDao.ListByLcdaId(lcdaId, pageModel);
        }

        public async Task<object> ListByLcdaId(Guid lcdaId)
        {
            return await itemDao.ListByLcdaId(lcdaId);
        }

        public async Task<Response> Update(Item item)
        {
            return await itemDao.Update(item);
        }

        public async Task<Response> UpdateStatus(Item item)
        {
            return await itemDao.UpdateStatus(item);
        }
    }
}

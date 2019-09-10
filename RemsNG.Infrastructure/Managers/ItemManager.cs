using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ItemManager : IItemManager
    {
        private readonly IItemRepository itemDao;

        public ItemManager(IItemRepository itemRepository)
        {
            itemDao = itemRepository;
        }

        public async Task<Response> Add(ItemModel item)
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

        public async Task<Response> Update(ItemModel item)
        {
            return await itemDao.Update(item);
        }

        public async Task<Response> UpdateStatus(ItemModel item)
        {
            return await itemDao.UpdateStatus(item);
        }
    }
}

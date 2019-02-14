using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ListPropertyManagers : IListPropertyManagers
    {
        private readonly LcdaPropertyRepository lcdaPropertyDao;
        public ListPropertyManagers(RemsDbContext _db)
        {
            lcdaPropertyDao = new LcdaPropertyRepository(_db);
        }

        public async Task<List<LcdaPropertyModel>> ByLcda(Guid lcdaId)
        {
            return await lcdaPropertyDao.ByLcda(lcdaId);
        }
    }
}

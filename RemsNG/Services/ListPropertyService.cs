using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class ListPropertyService : IListPropertyService
    {
        private readonly LcdaPropertyRepository lcdaPropertyDao;
        public ListPropertyService(RemsDbContext _db)
        {
            lcdaPropertyDao = new LcdaPropertyRepository(_db);
        }

        public async Task<List<LcdaProperty>> ByLcda(Guid lcdaId)
        {
            return await lcdaPropertyDao.ByLcda(lcdaId);
        }
    }
}

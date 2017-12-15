using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class LcdaBankService : ILcdaBankService
    {
        private LcdaBankDao lcdaBankDao;
        public LcdaBankService(RemsDbContext _db)
        {
            lcdaBankDao = new LcdaBankDao(_db);
        }
        public async Task<List<LcdaBank>> Get(Guid lcdaId)
        {
            return await lcdaBankDao.Get(lcdaId);
        }
    }
}

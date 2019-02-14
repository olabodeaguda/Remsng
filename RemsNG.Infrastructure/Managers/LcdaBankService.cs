using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class LcdaBankManagers : ILcdaBankManagers
    {
        private readonly LcdaBankRepository lcdaBankDao;
        public LcdaBankManagers(RemsDbContext _db)
        {
            lcdaBankDao = new LcdaBankRepository(_db);
        }
        public async Task<List<BankLcdaModel>> Get(Guid lcdaId)
        {
            return await lcdaBankDao.Get(lcdaId);
        }
    }
}

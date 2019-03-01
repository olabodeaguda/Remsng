using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class LcdaBankManager : ILcdaBankManager
    {
        private readonly LcdaBankRepository lcdaBankDao;
        public LcdaBankManager(DbContext _db)
        {
            lcdaBankDao = new LcdaBankRepository(_db);
        }
        public async Task<List<BankLcdaModel>> Get(Guid lcdaId)
        {
            return await lcdaBankDao.Get(lcdaId);
        }
    }
}

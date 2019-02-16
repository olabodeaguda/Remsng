using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class BankManagers : IBankManagers
    {
        private readonly BankRepository bankDao;
        public BankManagers(DbContext _db)
        {
            bankDao = new BankRepository(_db);
        }

        public async Task<List<BankModel>> GetBankAsync()
        {
            return await bankDao.GetBankAsync();
        }
    }
}

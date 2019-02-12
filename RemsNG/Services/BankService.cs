using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class BankService : IBankService
    {
        private BankRepository bankDao;
        public BankService(RemsDbContext _db)
        {
            bankDao = new BankRepository(_db);
        }

        public async Task<List<Bank>> GetBankAsync()
        {
            return await bankDao.GetBankAsync();
        }
    }
}

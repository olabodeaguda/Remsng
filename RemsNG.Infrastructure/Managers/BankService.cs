using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Data.Repository;
using Remsng.Data;
using RemsNG.Common.Models;

namespace RemsNG.Services
{
    public class BankManagers : IBankManagers
    {
        private readonly BankRepository bankDao;
        public BankManagers(RemsDbContext _db)
        {
            bankDao = new BankRepository(_db);
        }

        public async Task<List<BankModel>> GetBankAsync()
        {
            return await bankDao.GetBankAsync();
        }
    }
}

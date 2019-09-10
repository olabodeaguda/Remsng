using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class BankManager : IBankManager
    {
        private readonly IBankRepository bankDao;
        public BankManager(IBankRepository bankRepository)
        {
            bankDao = bankRepository;
        }

        public async Task<List<BankModel>> GetBankAsync()
        {
            return await bankDao.GetBankAsync();
        }
    }
}

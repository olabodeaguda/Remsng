using RemsNG.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IBankService
    {
        Task<List<BankModel>> GetBankAsync();
    }
}

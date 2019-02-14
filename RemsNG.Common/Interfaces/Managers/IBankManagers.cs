using RemsNG.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IBankManagers
    {
        Task<List<BankModel>> GetBankAsync();
    }
}

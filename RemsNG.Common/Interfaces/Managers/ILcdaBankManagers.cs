using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ILcdaBankManagers
    {
        Task<List<BankLcdaModel>> Get(Guid lcdaId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface ILcdaBankService
    {
        Task<List<LcdaBank>> Get(Guid lcdaId);
    }
}

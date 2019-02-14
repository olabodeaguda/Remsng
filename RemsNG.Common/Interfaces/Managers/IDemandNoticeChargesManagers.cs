using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDemandNoticeChargesManagers
    {
        Task<decimal> getCharges(decimal amount, Guid lcdaId);
    }
}

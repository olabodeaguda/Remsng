using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IDemandNoticeCharges
    {
        Task<decimal> getCharges(decimal amount, Guid lcdaId);
    }
}

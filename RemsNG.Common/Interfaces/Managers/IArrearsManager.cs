using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IArrearsManager
    {
        Task<bool> RunTaxpayerArrears(Guid[] dnTaxpayerIds);
        Task<bool> RemoveTaxpayerArrears(Guid[] dnTaxpayerIds);
    }
}

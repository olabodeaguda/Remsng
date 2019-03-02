using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IArrearsManager
    {
        Task RunTaxpayerArrears(Guid taxpayerId);

        Task RunTaxpayerArrears(Guid[] taxpayerId);
    }
}

using RemsNG.Common.Interfaces.Managers;
using System;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ArrearsManager : IArrearsManager
    {
        public Task RunTaxpayerArrears(Guid taxpayerId)
        {
            // get amount status
            throw new NotImplementedException();
        }

        public Task RunTaxpayerArrears(Guid[] taxpayerId)
        {
            throw new NotImplementedException();
        }
    }
}

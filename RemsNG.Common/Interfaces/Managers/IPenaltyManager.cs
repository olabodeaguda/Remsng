using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IPenaltyManager
    {
        Task<bool> AddPenalty(Guid[] dnTaxpayerIds);
        Task<bool> AddPenalty(DemantNoticePenaltyModelExt2[] model);
        Task<bool> RemovePenalty(Guid[] dnTaxpayerIds);
    }
}

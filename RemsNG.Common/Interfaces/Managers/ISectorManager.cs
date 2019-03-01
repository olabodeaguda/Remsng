using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ISectorManager
    {
        Task<Response> Add(SectorModel sector);
        Task<List<SectorModel>> ByLcdaId(Guid lcdaId);
        Task<SectorModel> ById(Guid id);
        Task<Response> Update(SectorModel sector);
        Task<SectorModel> ByTaxpayerId(Guid taxpayerId);
    }
}

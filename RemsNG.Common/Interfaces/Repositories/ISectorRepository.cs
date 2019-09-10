using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface ISectorRepository
    {
        Task<Response> Add(SectorModel sector);
        Task<Response> Update(SectorModel sector);
        Task<List<SectorModel>> ByLcdaId(Guid lcdaId);
        Task<SectorModel> ById(Guid id);
        Task<SectorModel> ByTaxpayerId(Guid taxpayerId);

    }
}

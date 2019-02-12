using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface ISectorService
    {
        Task<Response> Add(Sector sector);
        Task<List<Sector>> ByLcdaId(Guid lcdaId);
        Task<Sector> ById(Guid id);
        Task<Response> Update(Sector sector);
        Task<Sector> ByTaxpayerId(Guid taxpayerId);
    }
}

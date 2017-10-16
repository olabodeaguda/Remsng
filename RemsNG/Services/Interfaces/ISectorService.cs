using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface ISectorService
    {
        Task<Response> Add(Sector sector);
        Task<List<Sector>> ByLcdaId(Guid lcdaId);
        Task<Sector> ById(Guid id);
        Task<Response> Update(Sector sector);
    }
}

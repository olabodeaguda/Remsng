using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IListPropertyService
    {
        Task<List<LcdaProperty>> ByLcda(Guid lcdaId);
    }
}

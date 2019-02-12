using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IListPropertyService
    {
        Task<List<LcdaPropertyModel>> ByLcda(Guid lcdaId);
    }
}

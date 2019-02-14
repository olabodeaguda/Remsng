using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IListPropertyManagers
    {
        Task<List<LcdaPropertyModel>> ByLcda(Guid lcdaId);
    }
}

using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ListPropertyManager : IListPropertyManager
    {
        private readonly ILcdaPropertyRepository lcdaPropertyDao;
        public ListPropertyManager(ILcdaPropertyRepository lcdaPropertyRepository)
        {
            lcdaPropertyDao = lcdaPropertyRepository;
        }

        public async Task<List<LcdaPropertyModel>> ByLcda(Guid lcdaId)
        {
            return await lcdaPropertyDao.ByLcda(lcdaId);
        }
    }
}

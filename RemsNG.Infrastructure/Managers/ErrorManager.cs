using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class ErrorManager : IErrorManager
    {
        private IErrorRepository errorDao;
        public ErrorManager(IErrorRepository errorRepository)
        {
            errorDao = errorRepository;
        }

        public async Task<bool> Add(ErrorModel error)
        {
            return await errorDao.Add(error);
        }

        public async Task<List<ErrorModel>> ByOwnerIdAsync(Guid ownerId)
        {
            return await errorDao.ByOwnerIdAsync(ownerId);
        }

    }
}

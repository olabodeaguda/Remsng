using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using Microsoft.Extensions.Logging;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class ErrorService : IErrorService
    {
        private ErrorDao errorDao;
        public ErrorService(RemsDbContext _db, ILoggerFactory loggerFactory)
        {
            errorDao = new ErrorDao(_db, loggerFactory);
        }

        public async Task<bool> Add(Error error)
        {
            return await errorDao.Add(error);
        }
    }
}

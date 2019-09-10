using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class ErrorRepository : IErrorRepository
    {
        private readonly DbContext db;
        private readonly ILogger logger;
        public ErrorRepository(DbContext _db, ILoggerFactory loggerFactory)
        {
            db = _db;
            logger = loggerFactory.CreateLogger("Error Dao");
        }


        public async Task<bool> Add(ErrorModel error)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>()
            //    .FromSql("sp_addError @p0,@p1,@p2", new object[]
            //{
            //    error.OwnerId,
            //    error.ErrorType,
            //    error.Errorvalue
            //}).FirstOrDefaultAsync();

            db.Set<Error>().Add(new Error
            {
                DateCreated = DateTime.Now,
                ErrorType = error.ErrorType,
                Errorvalue = error.Errorvalue,
                Id = Guid.NewGuid(),
                OwnerId = error.OwnerId
            });
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<ErrorModel>> ByOwnerIdAsync(Guid ownerId)
        {
            var result = await db.Set<Error>()
                .Where(x => x.OwnerId == ownerId)
                .Select(x => new ErrorModel()
                {
                    DateCreated = x.DateCreated,
                    ErrorType = x.ErrorType,
                    Errorvalue = x.Errorvalue,
                    Id = x.Id,
                    OwnerId = x.OwnerId
                }).ToListAsync();
            return result;
        }

    }
}

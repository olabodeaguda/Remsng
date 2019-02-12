using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class ErrorRepository : AbstractRepository
    {
        private readonly ILogger logger;
        public ErrorRepository(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("Error Dao");
        }

        public ErrorRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<bool> Add(Error error)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addError @p0,@p1,@p2", new object[]
            {
                error.OwnerId,
                error.ErrorType,
                error.Errorvalue
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return true;
            }

            if (logger != null)
            {
                logger.LogError(dbResponse.msg, error);
            }
            return false;
        }

        public async Task<List<Error>> ByOwnerIdAsync(Guid ownerId)
        {
            return await db.Errors.FromSql($"select * from tbl_error where ownerId = '{ownerId}'").ToListAsync();
        }

    }
}

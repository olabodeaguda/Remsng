using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
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

        public async Task<bool> Add(ErrorModel error)
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

        public async Task<List<ErrorModel>> ByOwnerIdAsync(Guid ownerId)
        {
            var result = await db.Errors.FromSql($"select * from tbl_error where ownerId = '{ownerId}'").ToListAsync();
            return result.Select(x => new ErrorModel()
            {
                DateCreated = x.DateCreated,
                ErrorType = x.ErrorType,
                Errorvalue = x.Errorvalue,
                Id = x.Id,
                OwnerId = x.OwnerId
            }).ToList();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RemsNG.Dao
{
    public class ErrorDao : AbstractDao
    {
        private readonly ILogger logger;
        public ErrorDao(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            this.logger = loggerFactory.CreateLogger("Error Dao");
        }

        public ErrorDao(RemsDbContext _db): base(_db)
        {
        }

        public async Task<bool> Add(Error error)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_addError @p0,@p1,@p2", new object[]
            {
                error.ownerId,
                error.errorType,
                error.errorvalue
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

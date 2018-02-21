using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Models;
using RemsNG.ORM;

namespace RemsNG.Dao
{
    public class SyncDao : AbstractDao
    {
        private readonly ILogger logger;
        public SyncDao(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            this.logger = loggerFactory.CreateLogger("Sync Dao");
        }

        public async Task<List<SyncDataModel>> Get() =>
             await db.SyncDataModels.FromSql("sp_paymentReportSync").ToListAsync();
    }
}

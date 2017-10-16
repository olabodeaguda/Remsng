using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class SectorDao : AbstractDao
    {
        private readonly ILogger logger;
        public SectorDao(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            this.logger = loggerFactory.CreateLogger("Sector Dao");
        }

        public async Task<Response> Add(Sector sector)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_createSector @p0, @p1, @p2", new object[] {
                sector.sectorName,
                sector.lcdaId,
                sector.createdBy
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }

            logger.LogError(dbResponse.msg, dbResponse); //new object[] { userLcda.userId, userLcda.lgdaId });
            return new Response()
            {
                code = MsgCode_Enum.FAIL,
                description = dbResponse.msg
            };
        }

        public async Task<Response> Update(Sector sector)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateSector @p0, @p1, @p2", new object[] {
                sector.id,
                sector.sectorName,
                sector.lastmodifiedby
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }

            logger.LogError(dbResponse.msg, dbResponse); //new object[] { userLcda.userId, userLcda.lgdaId });
            return new Response()
            {
                code = MsgCode_Enum.FAIL,
                description = dbResponse.msg
            };
        }

        public async Task<List<Sector>> ByLcdaId(Guid lcdaId) => await db.Sectors.Where(x => x.lcdaId == lcdaId).ToListAsync();

        public async Task<Sector> ById(Guid id) => await db.Sectors.FirstOrDefaultAsync(x=>x.id == id);
    }
}

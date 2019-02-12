using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class SectorRepository : AbstractRepository
    {
        private readonly ILogger logger;
        public SectorRepository(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("Sector Dao");
        }

        public async Task<Response> Add(Sector sector)
        {
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_createSector @p0, @p1, @p2, @p3", new object[] {
                sector.SectorName,
                sector.LcdaId,
                sector.CreatedBy,
                sector.Prefix
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
            DbResponse dbResponse = await db.DbResponses.FromSql("sp_updateSector @p0, @p1, @p2, @p3", new object[] {
                sector.Id,
                sector.SectorName,
                sector.Lastmodifiedby,
                sector.Prefix
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

        public async Task<List<Sector>> ByLcdaId(Guid lcdaId)
        {
            return await db.Sectors.Where(x => x.LcdaId == lcdaId).ToListAsync();
        }

        public async Task<Sector> ById(Guid id)
        {
            return await db.Sectors.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Sector> ByTaxpayerId(Guid taxpayerId)
        {
            string query = $"select top 1 sect.*, tp.id as tpId from tbl_taxPayer as tp " +
                $"inner join tbl_company as cpy on tp.companyId = cpy.id " +
                $"inner join tbl_sector as sect on sect.id = cpy.sectorId " +
                $"where tp.id = '{taxpayerId}'";
            return await db.Sectors.FromSql(query).FirstOrDefaultAsync();
        }
    }
}

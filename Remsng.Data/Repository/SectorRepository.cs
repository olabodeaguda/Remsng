using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
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

        public async Task<List<SectorModel>> ByLcdaId(Guid lcdaId)
        {
            var result = await db.Sectors.Where(x => x.LcdaId == lcdaId).ToListAsync();

            return result.Select(x => new SectorModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                Prefix = x.Prefix,
                SectorName = x.SectorName
            }).ToList();
        }

        public async Task<SectorModel> ById(Guid id)
        {
            var x = await db.Sectors.FirstOrDefaultAsync(p => p.Id == id);
            if (x == null)
            {
                return null;
            }
            return new SectorModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                Prefix = x.Prefix,
                SectorName = x.SectorName
            };
        }

        public async Task<SectorModel> ByTaxpayerId(Guid taxpayerId)
        {
            string query = $"select top 1 sect.*, tp.id as tpId from tbl_taxPayer as tp " +
                $"inner join tbl_company as cpy on tp.companyId = cpy.id " +
                $"inner join tbl_sector as sect on sect.id = cpy.sectorId " +
                $"where tp.id = '{taxpayerId}'";
            var x = await db.Sectors.FromSql(query).FirstOrDefaultAsync();

            if (x == null)
            {
                return null;
            }
            return new SectorModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                Prefix = x.Prefix,
                SectorName = x.SectorName
            };
        }
    }
}

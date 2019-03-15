using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
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
        public SectorRepository(DbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("Sector Dao");
        }

        public async Task<Response> Add(SectorModel sector)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>()
            //    .FromSql("sp_createSector @p0, @p1, @p2, @p3", new object[] {
            //    sector.SectorName,
            //    sector.LcdaId,
            //    sector.CreatedBy,
            //    sector.Prefix
            //}).FirstOrDefaultAsync();

            db.Set<Sector>().Add(new Sector
            {
                CreatedBy = sector.CreatedBy,
                DateCreated = DateTime.Now,
                Id = Guid.NewGuid(),
                LcdaId = sector.LcdaId,
                SectorName = sector.SectorName,
                Prefix = sector.Prefix
            });

            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Sector has been added successfully"
            };
        }

        public async Task<Response> Update(SectorModel sector)
        {
            var entity = await db.Set<Sector>().FindAsync(sector.Id);
            if (entity == null)
            {
                throw new NotFoundException($"{sector.SectorName} sector does not exist");
            }
            entity.SectorName = sector.SectorName;
            entity.Lastmodifiedby = sector.Lastmodifiedby;
            entity.Prefix = sector.Prefix;

            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Sector has been update successfully"
            };
        }

        public async Task<List<SectorModel>> ByLcdaId(Guid lcdaId)
        {
            var result = await db.Set<Sector>()
                .Where(x => x.LcdaId == lcdaId).OrderBy(x=>x.SectorName).ToListAsync();

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
            var x = await db.Set<SectorModel>()
                .FirstOrDefaultAsync(p => p.Id == id);
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
            var x = await db.Set<Sector>().FromSql(query).FirstOrDefaultAsync();

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

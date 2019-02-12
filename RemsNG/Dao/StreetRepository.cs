using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class StreetRepository : AbstractRepository
    {
        private readonly ILogger logger;
        public StreetRepository(RemsDbContext _db, ILoggerFactory loggerFactory) : base(_db)
        {
            logger = loggerFactory.CreateLogger("LCDA Dao");
        }

        public StreetRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(Street street)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addStreet @p0, @p1, @p2, @p3, @p4, @p5", new object[] {
                street.Id,
                street.WardId,
                street.StreetName,
                street.NumberOfHouse,
                street.CreatedBy,
                street.StreetDescription
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }

        public async Task<Response> Update(Street street)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_updateStreet @p0, @p1, @p2, @p3, @p4, @p5", new object[] {
                street.Id,
                street.WardId,
                street.StreetName,
                street.NumberOfHouse,
                street.Lastmodifiedby,
                street.StreetDescription
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }

        public async Task<Response> ChangeStatus(Guid id, string streetStatus)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_StreetchangeStatus @p0, @p1", new object[] {
                id,
                streetStatus
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }

        public async Task<Street> ById(Guid streetId)
        {
            return await db.Streets.FirstOrDefaultAsync(x => x.Id == streetId);
        }

        public async Task<List<Street>> ByWard(Guid wardId)
        {
            return await db.Streets.Where(x => x.WardId == wardId).OrderBy(x => x.StreetName).ToListAsync();
        }

        public async Task<object> ByWardpaginated(Guid wardId, PageModel pageModel)
        {
            var results = await db.Streets.Where(x => x.WardId == wardId)
                 .Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                 .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.Streets.Where(x => x.WardId == wardId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = totalCount//(totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<int> ByWardCount(Guid wardId)
        {
            return await db.Streets.Where(x => x.WardId == wardId).CountAsync();
        }

        public async Task<List<Street>> ByLcda(Guid lcdaId)
        {
            return await db.Streets.FromSql("sp_streetbyLcda @p0", new object[] { lcdaId }).ToListAsync();
        }

        public async Task<Domain> GetDomain(Guid streetId)
        {
            string query = $"select distinct tbl_domain.* from tbl_domain " +
                $"inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id " +
                $"inner join tbl_ward on tbl_ward.lcdaId = tbl_lcda.id " +
                $"inner join tbl_street on tbl_street.wardId = tbl_ward.id " +
                $" where tbl_street.id = '{streetId}'";

            return await db.Domains.FromSql(query).FirstOrDefaultAsync();
        }

        public async Task<List<Street>> SearchStreet(Guid wardId, string searchName)
        {
            return await db.Streets.Where(x => x.WardId == wardId && x.StreetName.Contains(searchName)).ToListAsync();
        }
    }
}

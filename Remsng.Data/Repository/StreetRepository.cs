using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class StreetRepository : IStreetRepository
    {
        private readonly DbContext db;
        private readonly ILogger logger;
        public StreetRepository(DbContext _db, ILoggerFactory loggerFactory)
        {
            db = _db;
            logger = loggerFactory.CreateLogger("LCDA Dao");
        }

        public async Task<Response> Add(StreetModel street)
        {
            db.Set<Street>().Add(new Street
            {
                Id = street.Id,
                WardId = street.WardId,
                StreetName = street.StreetName,
                NumberOfHouse = street.NumberOfHouse,
                CreatedBy = street.CreatedBy,
                StreetDescription = street.StreetDescription
            });

            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Street has been added sucessfully"
            };
        }

        public async Task<Response> Update(StreetModel street)
        {
            Street s = await db.Set<Street>().FindAsync(street.Id);
            if (s == null)
            {
                throw new NotFoundException($"{street.StreetName} street can not be found");
            }

            s.WardId = street.WardId;
            s.StreetName = street.StreetName;
            s.NumberOfHouse = street.NumberOfHouse;
            s.Lastmodifiedby = street.Lastmodifiedby;
            s.StreetDescription = street.StreetDescription;

            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Street has been updated successfully"
            };
        }

        public async Task<Response> ChangeStatus(Guid id, string streetStatus)
        {
            Street s = await db.Set<Street>().FindAsync(id);
            if (s == null)
            {
                throw new NotFoundException($"street can not be found");
            }

            s.StreetStatus = streetStatus;

            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Street has been updated successfully"
            };
        }

        public async Task<StreetModel> ById(Guid streetId)
        {
            var t = await db.Set<Street>()
                .Include(x => x.Ward)
                .FirstOrDefaultAsync(x => x.Id == streetId);
            if (t == null)
            {
                return null;
            }

            StreetModel s = new StreetModel()
            {
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                Id = t.Id,
                Lastmodifiedby = t.Lastmodifiedby,
                LastModifiedDate = t.LastModifiedDate,
                NumberOfHouse = t.NumberOfHouse,
                StreetDescription = t.StreetDescription,
                StreetName = t.StreetName,
                StreetStatus = t.StreetStatus,
                WardId = t.WardId
            };

            if (t.Ward != null)
            {
                s.Ward = new WardModel
                {
                    Id = t.Ward.Id,
                    WardName = t.Ward.WardName,
                    WardStatus = t.Ward.WardStatus
                };
            }

            return s;
        }

        public async Task<List<StreetModel>> ByWard(Guid wardId)
        {
            var result = await db.Set<Street>().Where(x => x.WardId == wardId).OrderBy(x => x.StreetName).ToListAsync();

            return result.Select(t => new StreetModel()
            {
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                Id = t.Id,
                Lastmodifiedby = t.Lastmodifiedby,
                LastModifiedDate = t.LastModifiedDate,
                NumberOfHouse = t.NumberOfHouse,
                StreetDescription = t.StreetDescription,
                StreetName = t.StreetName,
                StreetStatus = t.StreetStatus,
                WardId = t.WardId
            }).ToList();
        }

        public async Task<object> ByWardpaginated(Guid wardId, PageModel pageModel)
        {
            var results = await db.Set<Street>().Where(x => x.WardId == wardId)
                 .Skip((pageModel.PageNum - 1) * pageModel.PageSize)
                 .Take(pageModel.PageSize).ToListAsync();
            var totalCount = await db.Set<Street>().Where(x => x.WardId == wardId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<int> ByWardCount(Guid wardId)
        {
            return await db.Set<Street>().Where(x => x.WardId == wardId).CountAsync();
        }

        public async Task<List<StreetModel>> ByLcda(Guid lcdaId)
        {
            var result = await db.Set<Street>().Include(x => x.Ward)
                .Where(p => p.Ward.LcdaId == lcdaId).ToListAsync();

            return result.Select(t => new StreetModel()
            {
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                Id = t.Id,
                Lastmodifiedby = t.Lastmodifiedby,
                LastModifiedDate = t.LastModifiedDate,
                NumberOfHouse = t.NumberOfHouse,
                StreetDescription = t.StreetDescription,
                StreetName = t.StreetName,
                StreetStatus = t.StreetStatus,
                WardId = t.WardId
            }).ToList();
        }

        public async Task<DomainModel> GetDomain(Guid streetId)
        {
            string query = $"select distinct tbl_domain.* from tbl_domain " +
                $"inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id " +
                $"inner join tbl_ward on tbl_ward.lcdaId = tbl_lcda.id " +
                $"inner join tbl_street on tbl_street.wardId = tbl_ward.id " +
                $" where tbl_street.id = '{streetId}'";
            var result = await db.Set<Domain>()
                .FromSql(query).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }
            return new DomainModel()
            {
                AddressId = result.AddressId,
                Datecreated = result.Datecreated,
                DomainCode = result.DomainCode,
                DomainName = result.DomainName,
                DomainStatus = result.DomainStatus,
                DomainType = result.DomainType,
                Id = result.Id,
                StateId = result.StateId
            };
        }

        public async Task<List<StreetModel>> SearchStreet(Guid wardId, string searchName)
        {
            string[] q = searchName.Split(new char[] { ' ' });
            var result = new List<Street>();

            switch (q.Length)
            {
                case 1:
                    result = await db.Set<Street>().Where(x => x.WardId == wardId && EF.Functions.Like(x.StreetName, $"%{searchName}%")).ToListAsync();
                    break;
                case 2:
                    result = await db.Set<Street>().Where(x => x.WardId == wardId
                    && EF.Functions.Like(x.StreetName, $"%{q[0]}%") && EF.Functions.Like(x.StreetName, $"%{q[1]}%")).ToListAsync();
                    break;
                case 3:
                    result = await db.Set<Street>().Where(x => x.WardId == wardId
                    && EF.Functions.Like(x.StreetName, $"%{q[0]}%") && EF.Functions.Like(x.StreetName, $"%{q[1]}%")
                    && EF.Functions.Like(x.StreetName, $"%{q[2]}%")).ToListAsync();
                    break;
                case 4:
                    result = await db.Set<Street>().Where(x => x.WardId == wardId
                    && EF.Functions.Like(x.StreetName, $"%{q[0]}%") && EF.Functions.Like(x.StreetName, $"%{q[1]}%")
                    && EF.Functions.Like(x.StreetName, $"%{q[2]}%") && EF.Functions.Like(x.StreetName, $"%{q[3]}%")).ToListAsync();
                    break;
                default:
                    break;
            }

            return result.Select(t => new StreetModel()
            {
                CreatedBy = t.CreatedBy,
                DateCreated = t.DateCreated,
                Id = t.Id,
                Lastmodifiedby = t.Lastmodifiedby,
                LastModifiedDate = t.LastModifiedDate,
                NumberOfHouse = t.NumberOfHouse,
                StreetDescription = t.StreetDescription,
                StreetName = t.StreetName,
                StreetStatus = t.StreetStatus,
                WardId = t.WardId
            }).ToList();
        }
    }
}

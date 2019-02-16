using Microsoft.EntityFrameworkCore;
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
    public class WardRepository : AbstractRepository
    {
        public WardRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<List<WardModel>> all1()
        {
            var p = await db.Set<Ward>()
                .ToListAsync();
            return p.Select(x => new WardModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                WardName = x.WardName,
                WardStatus = x.WardStatus
            }).ToList();
        }

        public async Task<List<WardModel>> All()
        {
            var p = await db.Set<Ward>()
                .FromSql("sp_ward").ToListAsync();
            return p.Select(x => new WardModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                WardName = x.WardName,
                WardStatus = x.WardStatus
            }).ToList();
        }

        public async Task<List<WardModel>> ActiveWard()
        {
            var p = await db.Set<Ward>()
                .Where(x => x.WardStatus.ToUpper() == UserStatus.ACTIVE.ToString()).ToListAsync();
            return p.Select(x => new WardModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                WardName = x.WardName,
                WardStatus = x.WardStatus
            }).ToList();
        }

        public async Task<bool> Add(WardModel ward)
        {
            db.Set<Ward>().Add(new Ward()
            {
                CreatedBy = ward.CreatedBy,
                DateCreated = ward.DateCreated,
                Id = ward.Id,
                Lastmodifiedby = ward.Lastmodifiedby,
                LastModifiedDate = ward.LastModifiedDate,
                LcdaId = ward.LcdaId,
                WardName = ward.WardName,
                WardStatus = ward.WardStatus
            });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<object> Paginated(PageModel pageModel, Guid lgdaId)
        {
            var wardlst = await GetWardByLGDAId(lgdaId);
            var results = wardlst.OrderBy(x => x.WardName).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
            var totalCount = db.Set<Ward>().Count();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            var wardlst = await All();
            var results = wardlst.OrderBy(x => x.WardName).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
            var totalCount = db.Set<Ward>().Count();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<WardModel> GetWard(Guid id)
        {
            var x = await db.Set<Ward>()
                .FromSql("sp_wardById @p0", new object[] { id }).FirstOrDefaultAsync();
            return new WardModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                WardName = x.WardName,
                WardStatus = x.WardStatus
            };
        }

        public async Task<WardModel> GetWard(string wardName, Guid lgdaid)
        {
            var x = await db.Set<Ward>()
                .Where(p => p.WardName.ToLower() == wardName.ToLower() && p.LcdaId == lgdaid).FirstOrDefaultAsync();
            if (x == null)
            {
                return null;
            }

            return new WardModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                WardName = x.WardName,
                WardStatus = x.WardStatus
            };
        }

        public async Task<List<WardModel>> GetWardByLGDAId1(Guid lgdaId)
        {
            var result = await db.Set<Ward>()
                .Where(x => x.LcdaId == lgdaId).OrderBy(x => x.WardName).ToListAsync();
            if (result.Count <= 0)
            {
                return new List<WardModel>();
            }
            return result.Select(x => new WardModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                WardName = x.WardName,
                WardStatus = x.WardStatus
            }).ToList();
        }

        public async Task<List<WardModel>> GetWardByLGDAId(Guid lgdaId)
        {
            var result = await db.Set<Ward>()
                .FromSql("sp_wardByLcda @p0", new object[] { lgdaId }).ToListAsync();
            if (result == null)
            {
                return new List<WardModel>();
            }
            return result.Select(x => new WardModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                Id = x.Id,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                LcdaId = x.LcdaId,
                WardName = x.WardName,
                WardStatus = x.WardStatus
            }).ToList();
        }

        public async Task<bool> Update(WardModel ward)
        {
            var oldWard = db.Set<Ward>().FirstOrDefault(x => x.Id == ward.Id);
            if (oldWard == null)
            {
                throw new NotFoundException($"{ward.WardName} not found");
            }

            oldWard.WardName = ward.WardName;
            oldWard.Lastmodifiedby = ward.Lastmodifiedby;
            oldWard.LastModifiedDate = ward.LastModifiedDate;

            int affectedrow = await db.SaveChangesAsync();
            if (affectedrow > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<DomainModel> GetDomain(Guid wardId)
        {
            string query = $"select distinct tbl_domain.* from tbl_domain " +
                $"inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id " +
                $"inner join tbl_ward on tbl_ward.lcdaId = tbl_lcda.id " +
                $" where tbl_ward.id = '{wardId}'";
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
    }
}

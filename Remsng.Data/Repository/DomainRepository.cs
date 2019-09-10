using Microsoft.EntityFrameworkCore;
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
    public class DomainRepository : IDomainRepository
    {
        private readonly DbContext db;
        public DomainRepository(DbContext _db)
        {
            db = _db;
        }

        public async Task<List<DomainModel>> ActiveDomains()
        {
            var result = await db.Set<Domain>().Where(x => x.DomainStatus == UserStatus.ACTIVE.ToString()).OrderBy(x => x.DomainName).ToListAsync();
            return result.Select(x => new DomainModel()
            {
                AddressId = x.AddressId,
                Datecreated = x.Datecreated,
                DomainCode = x.DomainCode,
                DomainName = x.DomainName,
                DomainStatus = x.DomainStatus,
                DomainType = x.DomainType,
                Id = x.Id,
                StateId = x.StateId
            }).ToList();
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.Set<Domain>().Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.Set<Domain>().Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<bool> Add(DomainModel domain)
        {
            domain.DomainType = EncryptDecryptUtils.ToHexString("others");
            db.Set<Domain>().Add(new Domain()
            {
                AddressId = domain.AddressId,
                Datecreated = domain.Datecreated,
                DomainCode = domain.DomainCode,
                DomainName = domain.DomainName,
                DomainStatus = domain.DomainStatus,
                DomainType = domain.DomainType,
                Id = domain.Id,
                StateId = domain.StateId
            });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<DomainModel>> GetUserDomainByUsername(string username)
        {
            var result = await db.Set<Domain>()
                .FromSql("sp_getUserDomainByUsername @p0", new object[] { username }).ToListAsync();
            return result.Select(x => new DomainModel()
            {
                AddressId = x.AddressId,
                Datecreated = x.Datecreated,
                DomainCode = x.DomainCode,
                DomainName = x.DomainName,
                DomainStatus = x.DomainStatus,
                DomainType = x.DomainType,
                Id = x.Id,
                StateId = x.StateId
            }).ToList();
        }

        public async Task<List<DomainModel>> GetUserDomainByUsernameId(Guid id)
        {
            var result = await db.Set<Domain>()
                .FromSql("sp_getUserDomainByUserId @p0", new object[] { id }).ToListAsync();
            return result.Select(x => new DomainModel()
            {
                AddressId = x.AddressId,
                Datecreated = x.Datecreated,
                DomainCode = x.DomainCode,
                DomainName = x.DomainName,
                DomainStatus = x.DomainStatus,
                DomainType = x.DomainType,
                Id = x.Id,
                StateId = x.StateId
            }).ToList();
        }

        public async Task<DomainModel> byDomainId(Guid id)
        {
            var x = await db.Set<Domain>()
                .FirstOrDefaultAsync(f => f.Id == id);
            if (x == null)
            {
                return null;
            }
            return new DomainModel()
            {
                AddressId = x.AddressId,
                Datecreated = x.Datecreated,
                DomainCode = x.DomainCode,
                DomainName = x.DomainName,
                DomainStatus = x.DomainStatus,
                DomainType = x.DomainType,
                Id = x.Id,
                StateId = x.StateId
            };
        }

        public async Task<DomainModel> DomainbyLCDAId(Guid lcdaId)
        {
            var x = await db.Set<Domain>().FromSql("sp_domainbyLCDAId @p0", new object[] { lcdaId }).FirstOrDefaultAsync();
            if (x == null)
            {
                return null;
            }
            return new DomainModel()
            {
                AddressId = x.AddressId,
                Datecreated = x.Datecreated,
                DomainCode = x.DomainCode,
                DomainName = x.DomainName,
                DomainStatus = x.DomainStatus,
                DomainType = x.DomainType,
                Id = x.Id,
                StateId = x.StateId
            };
        }

        public async Task<DomainModel> byDomainCode(string domainCode)
        {
            var x = await db.Set<Domain>()
                .FirstOrDefaultAsync(p => p.DomainCode.ToLower() == domainCode.ToLower());
            if (x == null)
            {
                return null;
            }
            return new DomainModel()
            {
                AddressId = x.AddressId,
                Datecreated = x.Datecreated,
                DomainCode = x.DomainCode,
                DomainName = x.DomainName,
                DomainStatus = x.DomainStatus,
                DomainType = x.DomainType,
                Id = x.Id,
                StateId = x.StateId
            };
        }

        public async Task<bool> UpdateDomain(DomainModel domain)
        {
            var oldDomain = await db.Set<Domain>()
                .FirstOrDefaultAsync(x => x.Id == domain.Id);
            if (oldDomain == null)
            {
                throw new NotFoundException($"{domain.DomainName} not found");
            }
            oldDomain.DomainName = domain.DomainName;
            oldDomain.DomainCode = domain.DomainCode;
            oldDomain.StateId = domain.StateId;

            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> changeDomain(Guid domainId, string domainStatus)
        {
            var oldDomain = await db.Set<Domain>()
                .FirstOrDefaultAsync(x => x.Id == domainId);
            if (oldDomain == null)
            {
                throw new InvalidCredentialsException("Invalid credentials");
            }

            oldDomain.DomainStatus = domainStatus;
            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }
    }
}

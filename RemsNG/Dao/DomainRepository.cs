using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Exceptions;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DomainRepository : AbstractRepository
    {
        public DomainRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<Domain>> ActiveDomains()
        {
            return await db.Domains.Where(x => x.DomainStatus == UserStatus.ACTIVE.ToString()).OrderBy(x => x.DomainName).ToListAsync();
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.Domains.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.Domains.Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }
        
        public async Task<bool> Add(Domain domain)
        {
            domain.DomainType = EncryptDecryptUtils.ToHexString("others");
            db.Domains.Add(domain);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Domain>> GetUserDomainByUsername(string username)
        {
            return await Task.Run(() =>
            {
                return db.Domains.FromSql("sp_getUserDomainByUsername @p0", new object[] { username }).ToList();
            });
        }

        public async Task<List<Domain>> GetUserDomainByUsernameId(Guid id)
        {
            return await  db.Domains.FromSql("sp_getUserDomainByUserId @p0", new object[] { id }).ToListAsync();
        }

        public async Task<Domain> byDomainId(Guid id)
        {
            return await db.Domains.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Domain> DomainbyLCDAId(Guid lcdaId)
        {
            return await db.Set<Domain>().FromSql("sp_domainbyLCDAId @p0",new object[] { lcdaId}).FirstOrDefaultAsync();
        }

        public async Task<Domain> byDomainCode(string domainCode)
        {
            return await db.Domains.FirstOrDefaultAsync(x => x.DomainCode.ToLower() == domainCode.ToLower());
        }

        public async Task<bool> UpdateDomain(Domain domain)
        {
            var oldDomain = await db.Domains.FirstOrDefaultAsync(x => x.Id == domain.Id);
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
            var oldDomain = await db.Domains.FirstOrDefaultAsync(x => x.Id == domainId);
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

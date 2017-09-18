using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class DomainDao : AbstractDao
    {
        public DomainDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<object> Paginated(Models.PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.Domains.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.Domains.Count();
                return new
                {
                    data = results,
                    totalPageCount = totalCount % pageModel.PageSize + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<bool> Add(Domain domain)
        {
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

        public async Task<Domain> byDomainId(Guid id)
        {
            return await db.Domains.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<Domain> byDomainCode(string domainCode)
        {
            return await db.Domains.FirstOrDefaultAsync(x => x.domainCode.ToLower() == domainCode.ToLower());
        }

        public async Task<bool> UpdateDomain(Domain domain)
        {
            var oldDomain = await db.Domains.FirstOrDefaultAsync(x => x.id == domain.id);
            if (oldDomain == null)
            {
                throw new NotFoundException($"{domain.domainName} not found");
            }
            oldDomain.domainName = domain.domainName;
            oldDomain.domainCode = domain.domainCode;

            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> changeDomain(Guid domainId, string domainStatus)
        {
            var oldDomain = await db.Domains.FirstOrDefaultAsync(x => x.id == domainId);
            if (oldDomain == null)
            {
                throw new InvalidCredentialsException("Invalid credentials");
            }

            oldDomain.domainStatus = domainStatus;
            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }
    }
}

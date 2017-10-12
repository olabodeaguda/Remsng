using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class LcdaDao : AbstractDao
    {
        public LcdaDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Lgda> Get(Guid id)
        {
            return await db.lgdas.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<object> All()
        {
            return await db.lgdas.OrderBy(x => x.lcdaName).ToListAsync();
        }

        public async Task<object> All(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.lgdas.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.lgdas.Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<List<Lgda>> ActiveLCDAByDomainId(Guid domainId)
        {
            return await db.lgdas.Where(x => x.domainId == domainId && x.lcdaStatus == UserStatus.ACTIVE.ToString()).ToListAsync();

        }

        public async Task<UserLcda> UserLcdaByIds(Guid lgdaId, Guid userId)
        {
            return await db.UserLcdas.FirstOrDefaultAsync(x => x.userId == userId && x.lgdaId == lgdaId);
        }

        public async Task<bool> Add(Lgda lcda)
        {
            db.lgdas.Add(lcda);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Update(Lgda lcda)
        {
            var oldlcda = await db.lgdas.FirstOrDefaultAsync(x => x.id == lcda.id);
            if (oldlcda == null)
            {
                throw new NotFoundException($"{lcda.lcdaName} not found");
            }

            oldlcda.lcdaName = lcda.lcdaName;
            oldlcda.lcdaCode = lcda.lcdaCode;
            oldlcda.lastModifiedDate = DateTime.Now;
            oldlcda.lastmodifiedby = lcda.lastmodifiedby;
            oldlcda.domainId = lcda.domainId;

            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Changetatus(Guid id, string lcdastatus)
        {
            var oldlcda = await db.lgdas.FirstOrDefaultAsync(x => x.id == id);
            if (oldlcda == null)
            {
                throw new NotFoundException($"selected LCDA does not exist not found");
            }

            oldlcda.lcdaStatus = lcdastatus;
            int affectedCount = await db.SaveChangesAsync();
            if (affectedCount > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<Lgda> byLcdaCode(string lcdaCode)
        {
            return await db.lgdas.FirstOrDefaultAsync(x => x.lcdaCode.ToLower() == lcdaCode.ToLower());
        }

        public async Task<List<Lgda>> getLcdaByUsername(string username) =>
            await db.lgdas.FromSql("sp_getUserLCDAByUsername @p0", new object[] { username }).ToListAsync();

        public async Task<bool> AssignUserToLgda(UserLcda userLcda)
        {
            db.UserLcdas.Add(userLcda);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<List<Lgda>> UserDomainByUserId(Guid id)
        {
            return await db.lgdas.FromSql("sp_getUserLCDAByuserId @p0", new object[] { id }).ToListAsync();
        }

        public async Task<List<UserLcda>> UserRoleDomainbyUserId(Guid id)
        {
            return await db.UserLcdas.Include("role").Where(x => x.userId == id).ToListAsync();
        }
    }
}

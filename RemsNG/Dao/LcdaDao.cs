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

        public async Task<Lcda> Get(Guid id)
        {
            return await db.lcdas.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<object> All()
        {
            return await db.lcdas.ToListAsync();
        }

        public async Task<object> All(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.lcdas.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.lcdas.Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<List<Lcda>> ActiveLCDAByDomainId(Guid domainId)
        {
            return await Task.Run(() =>
            {
                return db.lcdas.Where(x => x.domainId == domainId && x.lcdaStatus == UserStatus.ACTIVE.ToString()).ToList();
            });
        }

        public async Task<bool> Add(Lcda lcda)
        {
            db.lcdas.Add(lcda);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Update(Lcda lcda)
        {
            var oldlcda = await db.lcdas.FirstOrDefaultAsync(x => x.id == lcda.id);
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
            var oldlcda = await db.lcdas.FirstOrDefaultAsync(x => x.id == id);
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

        public async Task<Lcda> byLcdaCode(string lcdaCode)
        {
            return await db.lcdas.FirstOrDefaultAsync(x => x.lcdaCode.ToLower() == lcdaCode.ToLower());
        }

        public async Task<List<Lcda>> getLcdaByUsername(string username) =>
            await db.lcdas.FromSql("sp_getUserLCDAByUsername @p0", new object[] { username }).ToListAsync();

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
    }
}

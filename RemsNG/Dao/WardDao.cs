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
    public class WardDao : AbstractDao
    {
        public WardDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<Ward>> all() => await db.Wards.ToListAsync();

        public async Task<List<Ward>> ActiveWard() => await db.Wards.Where(x => x.wardStatus.ToUpper() == UserStatus.ACTIVE.ToString()).ToListAsync();

        public async Task<bool> Add(Ward ward)
        {
            db.Wards.Add(ward);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<object> Paginated(Models.PageModel pageModel, Guid lgdaId)
        {
            return await Task.Run(() =>
            {
                var results = db.Wards.Where(x => x.lcdaId == lgdaId).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.Wards.Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<object> Paginated(Models.PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.Wards.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.Wards.Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<Ward> GetWard(Guid id)
        {
            return await db.Wards.FirstOrDefaultAsync(x => x.id == id);
        }
        public async Task<Ward> GetWard(string wardName, Guid lgdaid)
            => await db.Wards.Where(x => x.wardName.ToLower() == wardName.ToLower() && x.lcdaId == lgdaid).FirstOrDefaultAsync();

        public async Task<List<Ward>> GetWardByLGDAId(Guid lgdaId) => await db.Wards.Where(x => x.lcdaId == lgdaId).OrderBy(x=>x.wardName).ToListAsync();

        public async Task<bool> Update(Ward ward)
        {
            var oldWard = db.Wards.FirstOrDefault(x => x.id == ward.id);
            if (oldWard == null)
            {
                throw new NotFoundException($"{ward.wardName} not found");
            }

            oldWard.wardName = ward.wardName;
            oldWard.lastmodifiedBy = ward.lastmodifiedBy;
            oldWard.lastModifiedDate = ward.lastModifiedDate;

            int affectedrow = await db.SaveChangesAsync();
            if (affectedrow > 0)
            {
                return true;
            }

            return false;
        }
        
    }
}

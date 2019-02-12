﻿using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Exceptions;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class WardRepository : AbstractRepository
    {
        public WardRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<List<Ward>> all1()
        {
            return await db.Wards.ToListAsync();
        }

        public async Task<List<Ward>> All()
        {
            return await db.Wards.FromSql("sp_ward").ToListAsync();
        }

        public async Task<List<Ward>> ActiveWard()
        {
            return await db.Wards.Where(x => x.WardStatus.ToUpper() == UserStatus.ACTIVE.ToString()).ToListAsync();
        }

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

        public async Task<object> Paginated(PageModel pageModel, Guid lgdaId)
        {
            var wardlst = await GetWardByLGDAId(lgdaId);
            var results = wardlst.OrderBy(x => x.WardName).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
            var totalCount = db.Wards.Count();
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
            var totalCount = db.Wards.Count();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<Ward> GetWard(Guid id)
        {
            return await db.Wards.FromSql("sp_wardById @p0", new object[] { id }).FirstOrDefaultAsync();//.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<Ward> GetWard(string wardName, Guid lgdaid)
        {
            return await db.Wards.Where(x => x.WardName.ToLower() == wardName.ToLower() && x.LcdaId == lgdaid).FirstOrDefaultAsync();
        }

        public async Task<List<Ward>> GetWardByLGDAId1(Guid lgdaId)
        {
            return await db.Wards.Where(x => x.LcdaId == lgdaId).OrderBy(x => x.WardName).ToListAsync();
        }

        public async Task<List<Ward>> GetWardByLGDAId(Guid lgdaId)
        {
            return await db.Wards.FromSql("sp_wardByLcda @p0", new object[] { lgdaId }).ToListAsync();
        }

        public async Task<bool> Update(Ward ward)
        {
            var oldWard = db.Wards.FirstOrDefault(x => x.Id == ward.Id);
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

        public async Task<Domain> GetDomain(Guid wardId)
        {
            string query = $"select distinct tbl_domain.* from tbl_domain " +
                $"inner join tbl_lcda on tbl_lcda.domainId = tbl_domain.id " +
                $"inner join tbl_ward on tbl_ward.lcdaId = tbl_lcda.id " +
                $" where tbl_ward.id = '{wardId}'";

            return await db.Domains.FromSql(query).FirstOrDefaultAsync();
        }
    }
}
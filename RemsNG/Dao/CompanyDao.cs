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
    public class CompanyDao : AbstractDao
    {
        public CompanyDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(Company company)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addCompany @p0, @p1, @p2, @p3, @p4", new object[] {
                company.companyName,
                company.sectorId,
                company.categoryId,
                company.createdBy,
                company.lcdaId
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{company.companyName} has been added successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while processing your request. Please try again or contact your administrator for help"
                };
            }
        }

        public async Task<Response> Update(Company company)
        {
            var r = await db.Companies.FindAsync(new object[] { company.id });
            if (r == null)
            {
                throw new NotFoundException($"{company.companyName} can not be found");
            }

            r.addressId = company.addressId != null ? company.addressId : null;
            r.categoryId = company.categoryId;
            r.companyName = company.companyName;
            r.sectorId = company.sectorId;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{company.companyName} has been updated successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while processing your request. Please try again or contact your administrator for help"
                };
            }
        }

        public async Task<Response> UpdateStatus(Company company)
        {
            var r = await db.Companies.FindAsync(new object[] { company.id });
            if (r == null)
            {
                throw new NotFoundException($"{company.companyName} can not be found");
            }

            r.companyStatus = company.companyStatus;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{company.companyName} has been added successfully"
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"An error occur while processing your request. Please try again or contact your administrator for help"
                };
            }
        }

        public async Task<Company> ById(Guid id) => await db.Companies.FirstOrDefaultAsync(x => x.id == id);

        public async Task<List<CompanyExt>> ByLcda(Guid lcdaId)
            => await db.Set<CompanyExt>().FromSql("sp_CompanyBylcdaId  @p0", new object[] { lcdaId }).ToListAsync();

        public async Task<object> ByLcda(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.Set<CompanyExt>().FromSql("sp_CompanyBylcdaIdpaginate @p0, @p1, @p2", new object[] {
                    lcdaId,
                    pageModel.PageNum,
                    pageModel.PageSize
            }).ToListAsync();
            var totalCount = 0;// db.Companies.Where(x => x.lcdaId == lcdaId).Count();
            if (results.Count > 0)
            {
                totalCount = results[0].totalSize.Value;
            }
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<List<Company>> ByStretId(Guid streetId)
        {
            return await db.Set<Company>().FromSql("sp_companyBystreetId @p0", new object[] { streetId }).ToListAsync();
        }
    }
}

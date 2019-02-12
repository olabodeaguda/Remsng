using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Exceptions;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class CompanyRepository : AbstractRepository
    {
        public CompanyRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(Company company)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addCompany @p0, @p1, @p2, @p3, @p4", new object[] {
                company.CompanyName,
                company.SectorId,
                company.CategoryId,
                company.CreatedBy,
                company.LcdaId
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{company.CompanyName} has been added successfully"
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
            var r = await db.Companies.FindAsync(new object[] { company.Id });
            if (r == null)
            {
                throw new NotFoundException($"{company.CompanyName} can not be found");
            }

            r.AddressId = company.AddressId != null ? company.AddressId : null;
            r.CategoryId = company.CategoryId;
            r.CompanyName = company.CompanyName;
            r.SectorId = company.SectorId;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{company.CompanyName} has been updated successfully"
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
            var r = await db.Companies.FindAsync(new object[] { company.Id });
            if (r == null)
            {
                throw new NotFoundException($"{company.CompanyName} can not be found");
            }

            r.CompanyStatus = company.CompanyStatus;

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{company.CompanyName} has been added successfully"
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

        public async Task<Company> ById(Guid id)
        {
            return await db.Companies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CompanyExtModel>> ByLcda(Guid lcdaId)
        {
            return await db.Set<CompanyExtModel>().FromSql("sp_CompanyBylcdaId  @p0", new object[] { lcdaId }).ToListAsync();
        }

        public async Task<object> ByLcda(Guid lcdaId, PageModel pageModel)
        {
            var results = await db.Set<CompanyExtModel>().FromSql("sp_CompanyBylcdaIdpaginate @p0, @p1, @p2", new object[] {
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

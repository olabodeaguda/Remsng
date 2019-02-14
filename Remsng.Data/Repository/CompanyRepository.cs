using Microsoft.EntityFrameworkCore;
using Remsng.Data;
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
    public class CompanyRepository : AbstractRepository
    {
        public CompanyRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(CompanyModel company)
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

        public async Task<Response> Update(CompanyModel company)
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

        public async Task<Response> UpdateStatus(CompanyModel company)
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

        public async Task<CompanyModel> ById(Guid id)
        {
            var p = await db.Companies.FirstOrDefaultAsync(x => x.Id == id);
            if (p == null)
            {
                return null;
            }
            return new CompanyModel()
            {
                AddressId = p.AddressId,
                CategoryId = p.CategoryId,
                CompanyName = p.CompanyName,
                CompanyStatus = p.CompanyStatus,
                CreatedBy = p.CreatedBy,
                DateCreated = p.DateCreated,
                Id = p.Id,
                Lastmodifiedby = p.Lastmodifiedby,
                LastModifiedDate = p.LastModifiedDate,
                LcdaId = p.LcdaId,
                SectorId = p.SectorId,
                StreetId = p.StreetId
            };
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

        public async Task<List<CompanyModel>> ByStretId(Guid streetId)
        {
            var r = await db.Set<Company>().FromSql("sp_companyBystreetId @p0", new object[] { streetId }).ToListAsync();

            return r.Select(p => new CompanyModel()
            {
                AddressId = p.AddressId,
                CategoryId = p.CategoryId,
                CompanyName = p.CompanyName,
                CompanyStatus = p.CompanyStatus,
                CreatedBy = p.CreatedBy,
                DateCreated = p.DateCreated,
                Id = p.Id,
                Lastmodifiedby = p.Lastmodifiedby,
                LastModifiedDate = p.LastModifiedDate,
                LcdaId = p.LcdaId,
                SectorId = p.SectorId,
                StreetId = p.StreetId
            }).ToList();
        }
    }
}

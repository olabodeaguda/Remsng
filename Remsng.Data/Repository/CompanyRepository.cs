using Microsoft.EntityFrameworkCore;
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
        public CompanyRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(CompanyModel company)
        {
            db.Set<Company>().Add(new Company
            {
                Id = Guid.NewGuid(),
                CategoryId = company.CategoryId,
                CompanyName = company.CompanyName,
                SectorId = company.SectorId,
                CreatedBy = company.CreatedBy,
                LcdaId = company.LcdaId,
                CompanyStatus = "ACTIVE"
            });

            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = $"{company.CompanyName} has been added successfully"
            };

        }

        public async Task<Response> Update(CompanyModel company)
        {
            var r = await db.Set<Company>().FindAsync(new object[] { company.Id });
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
            var r = await db.Set<Company>().FindAsync(new object[] { company.Id });
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
            var p = await db.Set<Company>().FirstOrDefaultAsync(x => x.Id == id);
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
            return await db.Set<Company>()
                .Join(db.Set<Sector>(), cyp => cyp.SectorId, sec => sec.Id, (cyp, sec) => new { cyp, sec })
                .Join(db.Set<TaxpayerCategory>(), cyp => cyp.cyp.CategoryId, tc => tc.Id, (x, tc) => new CompanyExtModel
                {
                    addressId = x.cyp.AddressId,
                    categoryId = tc.Id,
                    categoryName = tc.TaxpayerCategoryName,
                    companyName = x.cyp.CompanyName,
                    companyStatus = x.cyp.CompanyStatus,
                    createdBy = x.cyp.CreatedBy,
                    dateCreated = x.cyp.DateCreated,
                    id = x.cyp.Id,
                    lastmodifiedby = x.cyp.Lastmodifiedby,
                    lastModifiedDate = x.cyp.LastModifiedDate,
                    lcdaId = x.cyp.LcdaId,
                    sectorId = x.sec.Id,
                    sectorName = x.sec.SectorName,
                    streetId = x.cyp.StreetId
                }).Where(x => x.lcdaId == lcdaId).ToListAsync();
            //return await db.Set<CompanyExtModel>().FromSql("sp_CompanyBylcdaId  @p0", new object[] { lcdaId }).ToListAsync();
        }

        public async Task<object> ByLcda(Guid lcdaId, PageModel pageModel)
        {
            var query = db.Set<Company>()
                .Join(db.Set<Sector>(), cyp => cyp.SectorId, sec => sec.Id, (cyp, sec) => new { cyp, sec })
                .Join(db.Set<TaxpayerCategory>(), cyp => cyp.cyp.CategoryId, tc => tc.Id, (x, tc) => new CompanyModel
                {
                    AddressId = x.cyp.AddressId,
                    CategoryId = tc.Id,
                    categoryName = tc.TaxpayerCategoryName,
                    CompanyName = x.cyp.CompanyName,
                    CompanyStatus = x.cyp.CompanyStatus,
                    CreatedBy = x.cyp.CreatedBy,
                    DateCreated = x.cyp.DateCreated,
                    Id = x.cyp.Id,
                    Lastmodifiedby = x.cyp.Lastmodifiedby,
                    LastModifiedDate = x.cyp.LastModifiedDate,
                    LcdaId = x.cyp.LcdaId,
                    SectorId = x.sec.Id,
                    sectorName = x.sec.SectorName,
                    StreetId = x.cyp.StreetId
                }).Where(x => x.LcdaId == lcdaId);
            if (pageModel.PageNum == 0)
            {
                pageModel.PageNum = 1;
            }

            var results = await query.OrderBy(x => x.CompanyName).Skip(pageModel.PageNum).Take(pageModel.PageSize).ToListAsync();
            //var results = await db.Set<CompanyExtModel>().FromSql("sp_CompanyBylcdaIdpaginate @p0, @p1, @p2", new object[] {
            //        lcdaId,
            //        pageModel.PageNum,
            //        pageModel.PageSize
            //}).ToListAsync();
            var totalCount = await query.CountAsync();

            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<List<CompanyModel>> ByStretId(Guid streetId)
        {
            var r = await db.Set<Company>()
                .Join(db.Set<Sector>(), cyp => cyp.SectorId, sec => sec.Id, (cyp, sec) => new { cyp, sec })
                .Join(db.Set<TaxpayerCategory>(), cyp => cyp.cyp.CategoryId, tc => tc.Id, (x, tc) => new CompanyModel
                {
                    AddressId = x.cyp.AddressId,
                    CategoryId = tc.Id,
                    categoryName = tc.TaxpayerCategoryName,
                    CompanyName = x.cyp.CompanyName,
                    CompanyStatus = x.cyp.CompanyStatus,
                    CreatedBy = x.cyp.CreatedBy,
                    DateCreated = x.cyp.DateCreated,
                    Id = x.cyp.Id,
                    Lastmodifiedby = x.cyp.Lastmodifiedby,
                    LastModifiedDate = x.cyp.LastModifiedDate,
                    LcdaId = x.cyp.LcdaId,
                    SectorId = x.sec.Id,
                    sectorName = x.sec.SectorName,
                    StreetId = x.cyp.StreetId
                }).OrderBy(x => x.CompanyName).ToListAsync();

            return r;
        }
    }
}

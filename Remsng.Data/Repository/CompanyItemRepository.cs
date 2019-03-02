using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class CompanyItemRepository : AbstractRepository
    {
        public CompanyItemRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(CompanyItemModel model)
        {
            db.Set<CompanyItem>().Add(new CompanyItem()
            {
                Amount = model.Amount,
                BillingYear = model.BillingYear,
                CompanyStatus = model.CompanyStatus,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                Id = model.Id,
                ItemId = model.ItemId,
                Lastmodifiedby = model.Lastmodifiedby,
                LastModifiedDate = model.LastModifiedDate,
                TaxpayerId = model.TaxpayerId
            });
            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Item has been addedd successfully"
            };
        }

        public async Task<Response> Update(CompanyItemModel model)
        {
            var result = await db.Set<CompanyItem>().FindAsync(model.Id);
            result.TaxpayerId = model.TaxpayerId;
            result.ItemId = model.ItemId;
            result.Amount = model.Amount;
            result.BillingYear = model.BillingYear;
            result.CreatedBy = model.CreatedBy;
            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = ""
            };
        }

        public async Task<Response> UpdateStatus(Guid id, string companystatus)
        {
            var result = await db.Set<CompanyItem>().FindAsync(id);
            result.CompanyStatus = companystatus;
            await db.SaveChangesAsync();

            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Status has been updated successfully"
            };
        }

        public async Task<List<CompanyItemModel>> ByTaxpayer(Guid taxpayerId)
        {
            var result = await db.Set<CompanyItem>()
                .Include(x => x.TaxPayer)
                .Include(x => x.Item)
                .Where(x => x.TaxpayerId == taxpayerId)
                .Select(p => new CompanyItemModel()
                {
                    Amount = p.Amount,
                    BillingYear = p.BillingYear,
                    CompanyStatus = p.CompanyStatus,
                    CreatedBy = p.CreatedBy,
                    DateCreated = p.DateCreated,
                    Id = p.Id,
                    ItemId = p.ItemId,
                    ItemName = p.Item.ItemDescription,
                    Lastmodifiedby = p.Lastmodifiedby,
                    LastModifiedDate = p.LastModifiedDate,
                    TaxpayerId = p.TaxpayerId,
                    Firstname = p.TaxPayer.Firstname,
                    Lastname = p.TaxPayer.Lastname,
                    Surname = p.TaxPayer.Surname
                }).ToListAsync();

            return result;
        }

        public async Task<List<CompanyItemModel>> ByTaxpayer(Guid[] taxpayerIds)
        {
            var result = await db.Set<CompanyItem>()
                .Include(x => x.TaxPayer)
                .Include(x => x.Item)
                .Where(x => taxpayerIds.Any(p => p == x.TaxpayerId))
                .Select(p => new CompanyItemModel()
                {
                    Amount = p.Amount,
                    BillingYear = p.BillingYear,
                    CompanyStatus = p.CompanyStatus,
                    CreatedBy = p.CreatedBy,
                    DateCreated = p.DateCreated,
                    Id = p.Id,
                    ItemId = p.ItemId,
                    ItemName = p.Item.ItemDescription,
                    Lastmodifiedby = p.Lastmodifiedby,
                    LastModifiedDate = p.LastModifiedDate,
                    TaxpayerId = p.TaxpayerId,
                    Firstname = p.TaxPayer.Firstname,
                    Lastname = p.TaxPayer.Lastname,
                    Surname = p.TaxPayer.Surname
                }).ToListAsync();

            return result;
        }

        public async Task<CompanyItemModel> ById(Guid id)
        {
            var result = await db.Set<CompanyItem>()
                .Include(x => x.TaxPayer)
                .Include(x => x.Item)
                .Where(x => x.Id == id)
                .Select(p => new CompanyItemModel()
                {
                    Amount = p.Amount,
                    BillingYear = p.BillingYear,
                    CompanyStatus = p.CompanyStatus,
                    CreatedBy = p.CreatedBy,
                    DateCreated = p.DateCreated,
                    Id = p.Id,
                    ItemId = p.ItemId,
                    ItemName = p.Item.ItemDescription,
                    Lastmodifiedby = p.Lastmodifiedby,
                    LastModifiedDate = p.LastModifiedDate,
                    TaxpayerId = p.TaxpayerId,
                    Firstname = p.TaxPayer.Firstname,
                    Lastname = p.TaxPayer.Lastname,
                    Surname = p.TaxPayer.Surname
                }).FirstOrDefaultAsync();
            return result;// await db.Set<CompanyItemModelExt>().FromSql("sp_companyItemById @p0", new object[] { id }).FirstOrDefaultAsync();
        }

        public async Task<object> ByTaxpayerpaginated(Guid id, PageModel pageModel)
        {

            var query = db.Set<CompanyItem>()
                .Include(x => x.TaxPayer)
                .Include(x => x.Item)
                .Where(x => x.TaxpayerId == id);


            List<CompanyItemModel> results = await query.Select(p => new CompanyItemModel()
            {
                Amount = p.Amount,
                BillingYear = p.BillingYear,
                CompanyStatus = p.CompanyStatus,
                CreatedBy = p.CreatedBy,
                DateCreated = p.DateCreated,
                Id = p.Id,
                ItemId = p.ItemId,
                ItemName = p.Item.ItemDescription,
                Lastmodifiedby = p.Lastmodifiedby,
                LastModifiedDate = p.LastModifiedDate,
                TaxpayerId = p.TaxpayerId,
                Firstname = p.TaxPayer.Firstname,
                Lastname = p.TaxPayer.Lastname,
                Surname = p.TaxPayer.Surname
            }).Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToListAsync();


            var totalCount = await query.CountAsync();

            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }
    }
}

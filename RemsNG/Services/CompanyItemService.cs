using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class CompanyItemService : ICompanyItemService
    {
        CompanyItemRepository companyItemDao;
        public CompanyItemService(RemsDbContext _db)
        {
            companyItemDao = new CompanyItemRepository(_db);
        }

        public async Task<Response> Add(CompanyItem companyItem)
        {
            return await companyItemDao.Add(companyItem);
        }

        public async Task<List<CompanyItemExt>> ByTaxpayer(Guid taxpayerId)
        {
            return await companyItemDao.ByTaxpayer(taxpayerId);
        }

        public async Task<CompanyItemExt> ById(Guid id)
        {
            return await companyItemDao.ById(id);
        }

        public async Task<Response> Update(CompanyItem companyItem)
        {
            return await companyItemDao.Update(companyItem);
        }

        public async Task<Response> UpdateStatus(Guid id, string companystatus)
        {
            return await companyItemDao.UpdateStatus(id, companystatus);
        }

        public async Task<object> ByTaxpayerpaginated(Guid id, PageModel pageModel)
        {
            return await companyItemDao.ByTaxpayerpaginated(id, pageModel);
        }
    }
}

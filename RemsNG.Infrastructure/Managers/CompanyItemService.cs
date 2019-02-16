using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class CompanyItemManagers : ICompanyItemManagers
    {
        private readonly CompanyItemRepository companyItemDao;
        public CompanyItemManagers(RemsDbContext _db)
        {
            companyItemDao = new CompanyItemRepository(_db);
        }

        public async Task<Response> Add(CompanyItemModel companyItem)
        {
            return await companyItemDao.Add(companyItem);
        }

        public async Task<List<CompanyItemModelExt>> ByTaxpayer(Guid taxpayerId)
        {
            return await companyItemDao.ByTaxpayer(taxpayerId);
        }

        public async Task<CompanyItemModelExt> ById(Guid id)
        {
            return await companyItemDao.ById(id);
        }

        public async Task<Response> Update(CompanyItemModel companyItem)
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

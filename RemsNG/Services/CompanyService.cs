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
    public class CompanyService : ICompany
    {
        private CompanyDao companyDao;
        public CompanyService(RemsDbContext db)
        {
            companyDao = new CompanyDao(db);
        }

        public async Task<Response> Add(Company company)
        {
            return await companyDao.Add(company);
        }

        public async Task<Company> ById(Guid id)
        {
            return await companyDao.ById(id);
        }

        public async Task<List<CompanyExt>> ByLcda(Guid lcdaId)
        {
            return await companyDao.ByLcda(lcdaId);
        }

        public async Task<object> ByLcda(Guid lcdaId, PageModel pageModel)
        {
            return await companyDao.ByLcda(lcdaId, pageModel);
        }

        public async Task<Response> Update(Company company)
        {
            return await companyDao.Update(company);
        }

        public async Task<Response> UpdateStatus(Company company)
        {
            return await companyDao.UpdateStatus(company);
        }
    }
}

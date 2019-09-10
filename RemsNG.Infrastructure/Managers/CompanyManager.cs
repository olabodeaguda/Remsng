using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class CompanyManager : ICompanyManager
    {
        private readonly ICompanyRepository companyDao;
        public CompanyManager(ICompanyRepository companyRepository)
        {
            companyDao = companyRepository;
        }

        public async Task<Response> Add(CompanyModel company)
        {
            return await companyDao.Add(company);
        }

        public async Task<CompanyModel> ById(Guid id)
        {
            return await companyDao.ById(id);
        }

        public async Task<List<CompanyExtModel>> ByLcda(Guid lcdaId)
        {
            return await companyDao.ByLcda(lcdaId);
        }

        public async Task<object> ByLcda(Guid lcdaId, PageModel pageModel)
        {
            return await companyDao.ByLcda(lcdaId, pageModel);
        }

        public async Task<List<CompanyModel>> ByStreetId(Guid streetId)
        {
            return await companyDao.ByStretId(streetId);
        }

        public async Task<Response> Update(CompanyModel company)
        {
            return await companyDao.Update(company);
        }

        public async Task<Response> UpdateStatus(CompanyModel company)
        {
            return await companyDao.UpdateStatus(company);
        }

    }
}

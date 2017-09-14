using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;

namespace RemsNG.Services
{
    public class DomainService : IDomainService
    {
        private readonly DomainDao domainDao;
        public DomainService(RemsDbContext _db)
        {
            domainDao = new DomainDao(_db);
        }
        public async Task<List<Domain>> GetDomainByUsername(string username)
        {
            return await domainDao.GetUserDomainByUsername(username);
        }
    }
}

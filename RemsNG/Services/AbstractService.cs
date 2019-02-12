using RemsNG.Dao;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class AbstractService : IAbstractService
    {
        AbstractRepository abstractDao;
        public AbstractService(RemsDbContext _db)
        {
            abstractDao = new AbstractRepository(_db);
        }

        public async  Task<bool> ExecuteQueryAsync(string query)
        {
            return await abstractDao.ExecuteQueryAsync(query);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Data.Repository;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class AbstractManagers : IAbstractManagers
    {
        AbstractRepository abstractDao;

        public AbstractManagers(DbContext _db)
        {
            abstractDao = new AbstractRepository(_db);
        }

        public async Task<bool> ExecuteQueryAsync(string query)
        {
            return await abstractDao.ExecuteQueryAsync(query);
        }
    }
}

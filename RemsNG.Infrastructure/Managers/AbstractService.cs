using Remsng.Data;
using RemsNG.Common.Interfaces.Services;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class AbstractService : IAbstractService
    {
        AbstractRepository abstractDao;

        public AbstractService(RemsDbContext _db) : base(_db)
        {
            abstractDao = new AbstractRepository(_db);
        }

        public async Task<bool> ExecuteQueryAsync(string query)
        {
            return await abstractDao.ExecuteQueryAsync(query);
        }
    }
}

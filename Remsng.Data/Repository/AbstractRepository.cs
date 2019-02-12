using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class AbstractRepository
    {
        protected readonly RemsDbContext db;
        public AbstractRepository(RemsDbContext _db)
        {
            db = _db;
        }

        public async Task<bool> ExecuteQueryAsync(string query)
        {
            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }

            return false;
        }
    }
}

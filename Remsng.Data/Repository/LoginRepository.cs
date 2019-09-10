using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Repositories;

namespace RemsNG.Data.Repository
{
    public class LoginRepository : ILoginRepository
    {
        private readonly DbContext db;
        public LoginRepository(DbContext _db)
        {
            db = _db;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Remsng.Data;

namespace RemsNG.Data.Repository
{
    public class LoginRepository : AbstractRepository
    {
        public LoginRepository(DbContext _db) : base(_db)
        {
        }

        //public async Task<User> GetUser(string username)
        //{
        //    return await db.Users.FirstOrDefaultAsync(x => x.username == username.Trim());
        //}

    }
}

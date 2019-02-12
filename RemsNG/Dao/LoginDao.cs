using Microsoft.EntityFrameworkCore;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class LoginDao:AbstractRepository
    {
        public LoginDao(RemsDbContext _db):base(_db)
        {
        }

        //public async Task<User> GetUser(string username)
        //{
        //    return await db.Users.FirstOrDefaultAsync(x => x.username == username.Trim());
        //}

    }
}

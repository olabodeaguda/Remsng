using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Data.Entities;
using RemsNG.Exceptions;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class UserRepository : AbstractRepository
    {
        public UserRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<User> Get(Guid id)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Update(User user)
        {
            var usr = await db.Users.FindAsync(new object[] { user.Id });
            if (usr != null)
            {
                usr.LastModifiedDate = user.LastModifiedDate;
                usr.Lastmodifiedby = user.Lastmodifiedby;
                usr.Surname = user.Surname;
                usr.Firstname = user.Firstname;
                usr.Lastname = user.Lastname;
                usr.Email = user.Email;
                usr.Gender = user.Gender;

                int count = await db.SaveChangesAsync();
                if (count > 0)
                {
                    return true;
                }
            }
            else
            {
                throw new NotFoundException($"{user.Username} was not found");
            }

            return false;
        }

        public async Task<bool> Create(User user)
        {
            db.Users.Add(user);

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddAndAssignLGDA(User user, UserLcda userLcda)
        {
            db.Users.Add(user);
            db.UserLcdas.Add(userLcda);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AssignLGDA(UserLcda userLcda)
        {
            db.UserLcdas.Add(userLcda);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<User> ByEmail(string email)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> ByUserName(string username)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await Task.Run(() =>
            {
                var results = db.Users.Skip((pageModel.PageNum - 1) * pageModel.PageSize).Take(pageModel.PageSize).ToList();
                var totalCount = db.Users.Count();
                return new
                {
                    data = results,
                    totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
                };
            });
        }

        public async Task<object> Paginated(PageModel pageModel, Guid lcdaId)
        {
            var results = await db.Users.FromSql("sp_domainUsers @p0, @p1, @p2", new object[] {
                     lcdaId,
                     pageModel.PageNum,
                     pageModel.PageSize
                }).ToListAsync();
            var totalCount = await db.UserLcdas.Where(x => x.LgdaId == lcdaId).CountAsync();
            return new
            {
                data = results,
                totalPageCount = (totalCount % pageModel.PageSize > 0 ? 1 : 0) + Math.Truncate((double)totalCount / pageModel.PageSize)
            };
        }

        public async Task<bool> ChangePwd(Guid id, string newPwd)
        {
            var user = await db.Users.FindAsync(new object[] { id });
            if (user == null)
            {
                throw new NotFoundException("Can't find selected user");
            }

            user.PasswordHash = EncryptDecryptUtils.ToHexString(newPwd);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ChangeStatus(string status,Guid id)
        {
            string query = $"update tbl_users " +
                $"set userStatus = '{status}' where id = '{id}' ";

            int count = await db.Database.ExecuteSqlCommandAsync(query);
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }
}

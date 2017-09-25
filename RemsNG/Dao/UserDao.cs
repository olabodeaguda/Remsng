﻿using Microsoft.EntityFrameworkCore;
using RemsNG.Exceptions;
using RemsNG.ORM;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Dao
{
    public class UserDao : AbstractDao
    {
        public UserDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<User> Get(Guid id)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.id == id);
        }

        public async Task<bool> Update(User user)
        {
            var usr = await db.Users.FindAsync(new object[] { user.id });
            if (usr != null)
            {
                usr.lastModifiedDate = user.lastModifiedDate;
                usr.lastmodifiedby = user.lastmodifiedby;
                usr.surname = user.surname;
                usr.firstname = user.firstname;
                usr.lastname = user.lastname;
                usr.email = user.email;
                usr.gender = user.gender;

                int count = await db.SaveChangesAsync();
                if (count > 0)
                {
                    return true;
                }
            }
            else
            {
                throw new NotFoundException($"{user.username} was not found");
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
            return await db.Users.FirstOrDefaultAsync(x => x.email == email);
        }

        public async Task<User> ByUserName(string username)
        {
            return await db.Users.FirstOrDefaultAsync(x => x.username == username);
        }

        public async Task<object> Paginated(Models.PageModel pageModel)
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

        public async Task<object> Paginated(Models.PageModel pageModel, Guid lcdaId)
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

        public async Task<bool> ChangePwd(Guid id, string newPwd)
        {
            var user = await db.Users.FindAsync(new object[] { id });
            if (user == null)
            {
                throw new NotFoundException("Can't find selected user");
            }

            user.passwordHash = EncryptDecryptUtils.ToHexString(newPwd);
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }
}

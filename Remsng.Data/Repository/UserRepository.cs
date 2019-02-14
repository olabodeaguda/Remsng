using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class UserRepository : AbstractRepository
    {
        public UserRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<UserModel> Get(Guid id)
        {
            var result = await db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null)
            {
                return null;
            }
            return new UserModel()
            {
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Email = result.Email,
                Firstname = result.Firstname,
                Gender = result.Gender,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                Lockedoutenabled = result.Lockedoutenabled,
                LockedOutEndDateUtc = result.LockedOutEndDateUtc,
                PasswordHash = result.PasswordHash,
                SecurityStamp = result.SecurityStamp,
                Surname = result.Surname,
                Username = result.Username,
                UserStatus = result.UserStatus
            };
        }

        public async Task<bool> Update(UserModel user)
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

        public async Task<bool> Create(UserModel result)
        {
            db.Users.Add(new User()
            {
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Email = result.Email,
                Firstname = result.Firstname,
                Gender = result.Gender,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                Lockedoutenabled = result.Lockedoutenabled,
                LockedOutEndDateUtc = result.LockedOutEndDateUtc,
                PasswordHash = result.PasswordHash,
                SecurityStamp = result.SecurityStamp,
                Surname = result.Surname,
                Username = result.Username,
                UserStatus = result.UserStatus
            });

            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddAndAssignLGDA(UserModel result, UserLcdaModel userLcda)
        {
            db.Users.Add(new User()
            {
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Email = result.Email,
                Firstname = result.Firstname,
                Gender = result.Gender,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                Lockedoutenabled = result.Lockedoutenabled,
                LockedOutEndDateUtc = result.LockedOutEndDateUtc,
                PasswordHash = result.PasswordHash,
                SecurityStamp = result.SecurityStamp,
                Surname = result.Surname,
                Username = result.Username,
                UserStatus = result.UserStatus
            });
            db.UserLcdas.Add(new UserLcda()
            {
                LgdaId = userLcda.LgdaId,
                UserId = userLcda.UserId
            });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AssignLGDA(UserLcdaModel userLcda)
        {
            db.UserLcdas.Add(new UserLcda()
            {
                LgdaId = userLcda.LgdaId,
                UserId = userLcda.UserId
            });
            int count = await db.SaveChangesAsync();
            if (count > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<UserModel> ByEmail(string email)
        {
            var result = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (result == null)
            {
                return null;
            }
            return new UserModel()
            {
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Email = result.Email,
                Firstname = result.Firstname,
                Gender = result.Gender,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                Lockedoutenabled = result.Lockedoutenabled,
                LockedOutEndDateUtc = result.LockedOutEndDateUtc,
                PasswordHash = result.PasswordHash,
                SecurityStamp = result.SecurityStamp,
                Surname = result.Surname,
                Username = result.Username,
                UserStatus = result.UserStatus
            };
        }

        public async Task<UserModel> ByUserName(string username)
        {
            var result = await db.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (result == null)
            {
                return null;
            }
            return new UserModel()
            {
                CreatedBy = result.CreatedBy,
                DateCreated = result.DateCreated,
                Email = result.Email,
                Firstname = result.Firstname,
                Gender = result.Gender,
                Id = result.Id,
                Lastmodifiedby = result.Lastmodifiedby,
                LastModifiedDate = result.LastModifiedDate,
                Lastname = result.Lastname,
                Lockedoutenabled = result.Lockedoutenabled,
                LockedOutEndDateUtc = result.LockedOutEndDateUtc,
                PasswordHash = result.PasswordHash,
                SecurityStamp = result.SecurityStamp,
                Surname = result.Surname,
                Username = result.Username,
                UserStatus = result.UserStatus
            };
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

        public async Task<bool> ChangeStatus(string status, Guid id)
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

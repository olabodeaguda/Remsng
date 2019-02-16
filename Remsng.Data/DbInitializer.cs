using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Linq;

namespace RemsNG.ORM
{
    public class DbInitializer
    {
        public static async void Initialize(DbContext db)
        {
            db.Database.EnsureCreated();
            var mosDomain = db.Set<Domain>().FirstOrDefault(x => x.DomainCode.ToLower() == "mos-admin");
            if (mosDomain == null)
            {
                Domain domain = new Domain()
                {
                    Id = Guid.NewGuid(),
                    DomainName = "MOS-ADMIN",
                    DomainCode = "MOS-ADMIN",
                    Datecreated = DateTime.Now,
                    DomainType = EncryptDecryptUtils.ToHexString("mos-admin")
                };
                db.Set<Domain>().Add(domain);
                mosDomain = domain;
            }

            var mosUser = db.Set<User>().FirstOrDefault(x => x.Username.ToLower() == "mos-admin");
            if (mosUser == null)
            {
                User user = new User()
                {
                    Id = Guid.NewGuid(),
                    CreatedBy = "APPLICATION",
                    DateCreated = DateTime.Now,
                    Email = "sleekeesoftNigeria@outlook.com",
                    Lockedoutenabled = false,
                    LockedOutEndDateUtc = null,
                    Username = "mos-admin",
                    UserStatus = "ACTIVE",
                    Firstname = "Olabode",
                    Lastname = "H",
                    Surname = "Aguda",
                    PasswordHash = EncryptDecryptUtils.ToHexString("@mistletle102016"),
                    //SecurityStamp = EncryptDecryptUtils.EncryptSecurityStampModel(new SecurityStampModel()
                    //{
                    //    question = "Whose intellectual property?",
                    //    answer = "Aguda Olabode Hassan"
                    //})
                };
                db.Set<User>().Add(user);
                mosUser = user;
            }

            var userdomain = db.Set<UserDomain>().FirstOrDefault(x => x.UserId == mosUser.Id && x.DomainId == mosDomain.Id);
            if (userdomain == null)
            {
                UserDomain ud = new UserDomain()
                {
                    DomainId = mosDomain.Id,
                    UserId = mosUser.Id
                };

                db.Set<UserDomain>().Add(ud);
            }

            await db.SaveChangesAsync();
        }
    }
}

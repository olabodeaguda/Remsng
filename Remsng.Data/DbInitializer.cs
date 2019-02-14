using Microsoft.EntityFrameworkCore;
using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DbInitializer
    {
        public static async void Initialize(RemsDbContext db)
        {
            db.Database.EnsureCreated();
            var mosDomain = db.Domains.FirstOrDefault(x => x.domainCode.ToLower() == "mos-admin");
            if (mosDomain == null)
            {
                Domain domain = new Domain()
                {
                    id = Guid.NewGuid(),
                    domainName = "MOS-ADMIN",
                    domainCode = "MOS-ADMIN",
                    datecreated = DateTime.Now,
                    domainType = EncryptDecryptUtils.ToHexString("mos-admin")
                };
                db.Domains.Add(domain);
                mosDomain = domain;
            }

            var mosUser = db.Users.FirstOrDefault(x => x.username.ToLower() == "mos-admin");
            if (mosUser == null)
            {
                User user = new User()
                {
                    id = Guid.NewGuid(),
                    createdBy = "APPLICATION",
                    dateCreated = DateTime.Now,
                    email = "sleekeesoftNigeria@outlook.com",
                    lockedoutenabled = false,
                    lockedOutEndDateUTC = null,
                    username = "mos-admin",
                    userStatus = "ACTIVE",
                    firstname = "Olabode",
                    lastname = "H",
                    surname = "Aguda",
                    passwordHash = EncryptDecryptUtils.ToHexString("@mistletle102016"),
                    securityStamp = EncryptDecryptUtils.EncryptSecurityStampModel(new SecurityStampModel()
                    {
                        question = "Whose intellectual property?",
                        answer = "Aguda Olabode Hassan"
                    })
                };
                db.Users.Add(user);
                mosUser = user;
            }

            var userdomain = db.UserDomains.FirstOrDefault(x => x.userId == mosUser.id && x.domainId == mosDomain.id);
            if (userdomain == null)
            {
                UserDomain ud = new UserDomain()
                {
                    domainId = mosDomain.id,
                    userId = mosUser.id
                };

                db.UserDomains.Add(ud);
            }

            await db.SaveChangesAsync();
        }
    }
}

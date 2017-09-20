﻿using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Dao;
using System.Security.Claims;
using System.Security.Principal;
using RemsNG.Models;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using RemsNG.Utilities;
using Newtonsoft.Json;

namespace RemsNG.Services
{
    public class UserService : IUserService
    {
        private readonly JwtIssuerOptions jwtOptions;
        private readonly LoginDao loginDao;
        private readonly DomainDao domainDao;
        private readonly RoleDao roleDao;
        private readonly LcdaDao lcdaDao;
        public UserService(RemsDbContext _db,
            IOptions<JwtIssuerOptions> _jwtOptions)
        {
            loginDao = new LoginDao(_db);
            domainDao = new DomainDao(_db);
            roleDao = new RoleDao(_db);
            lcdaDao = new LcdaDao(_db);
            jwtOptions = _jwtOptions.Value;
        }

        public async Task<object> GetToken(User user, Guid lcdaId, bool byDomain)
        {
            if (!byDomain)
            {
                List<UserLcda> uls = await lcdaDao.getLcdaByUsername(user.username);
                if (uls.Count > 0)
                {
                    var selectedDomain = uls.FirstOrDefault();
                    lcdaId = selectedDomain.lgdaId;
                }
            }
            else
            {
                Lcda lcda = await lcdaDao.Get(lcdaId);
                lcdaId = lcda.id;
            }

            Role role = await roleDao.GetUserRoleByUsernameByDomainId(user.username, lcdaId);

            int logTime = int.TryParse(jwtOptions.logOutTIme, out logTime) ? logTime : 30;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.username),
                            new Claim(ClaimTypes.Name, $"{user.surname} {user.firstname} {user.lastname}"),
                            new Claim(ClaimTypes.Role, role != null? role.roleName:string.Empty),
                            new Claim("Domain",JsonConvert.SerializeObject(lcdaId)),
                            new Claim("identity",EncryptDecryptUtils.ToHexString(user.id.ToString()))
                        }),
                Expires = DateTime.UtcNow.AddMinutes(logTime),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = jwtOptions.SigningCredentials,
                IssuedAt = DateTime.UtcNow,

            };

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(tokenHandler.CreateToken(jwt));
            return new
            {
                tk = encodedJwt,
                username = user.username,
                fullname = $"{user.surname} {user.firstname} {user.lastname}",
                userStatus = user.userStatus,
                domainName = domainName
            };
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await loginDao.GetUser(username);
        }
    }
}

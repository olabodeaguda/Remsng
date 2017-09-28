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
        private readonly UserDao userDao;
        public UserService(RemsDbContext _db,
            IOptions<JwtIssuerOptions> _jwtOptions)
        {
            loginDao = new LoginDao(_db);
            domainDao = new DomainDao(_db);
            roleDao = new RoleDao(_db);
            lcdaDao = new LcdaDao(_db);
            userDao = new UserDao(_db);
            jwtOptions = _jwtOptions.Value;
        }

        public async Task<object> GetToken(User user, Guid lcdaId)
        {
            String domainName = "";

            if (user.username.ToLower() != "mos-admin")
            {
                List<Lgda> uls = await lcdaDao.getLcdaByUsername(user.username);
                if (uls.Count > 0)
                {
                    var selectedDomain = uls.FirstOrDefault();
                    lcdaId = selectedDomain.id;
                    if (selectedDomain != null)
                    {
                        Lgda ld = await lcdaDao.Get(lcdaId);
                        domainName = ld.lcdaName;
                    }
                }
            }
            else
            {
                domainName = "mos-admin";
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
            return await userDao.ByUserName(username);
        }

        public async Task<User> ByEmail(string email)
        {
            return await userDao.ByEmail(email);
        }

        public async Task<bool> Create(User user)
        {
            return await userDao.Create(user);
        }

        public Task<bool> AddAndAssignLGDA(User user, UserLcda userLcda)
        {
            return userDao.AddAndAssignLGDA(user, userLcda);
        }

        public async Task<object> Paginated(PageModel pageModel, Guid lcdaId)
        {
            return await userDao.Paginated(pageModel, lcdaId);
        }

        public async Task<object> Paginated(PageModel pageModel)
        {
            return await userDao.Paginated(pageModel);
        }

        public async Task<bool> Update(User user)
        {
            return await userDao.Update(user);
        }

        public async Task<User> Get(Guid id)
        {
            return await userDao.Get(id);
        }

        public async Task<bool> ChangePwd(Guid id, string newPwd)
        {
            return await userDao.ChangePwd(id, newPwd);
        }

        public async Task<bool> AssignLGDA(UserLcda userLcda)
        {
            return await this.userDao.AssignLGDA(userLcda);
        }
    }
}

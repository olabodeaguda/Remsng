using RemsNG.Services.Interfaces;
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
using Microsoft.Extensions.Logging;
using RemsNG.Security;

namespace RemsNG.Services
{
    public class UserService : IUserService
    {
        private readonly JwtIssuerOptions jwtOptions;
        private readonly LoginDao loginDao;
        private readonly DomainRepository domainDao;
        private readonly RoleRepository roleDao;
        private readonly LcdaRepository lcdaDao;
        private readonly UserDao userDao;
        private readonly PermissionRepository permissionDao;
        public UserService(RemsDbContext _db,
            IOptions<JwtIssuerOptions> _jwtOptions, ILoggerFactory loggerFactory)
        {
            loginDao = new LoginDao(_db);
            domainDao = new DomainRepository(_db);
            roleDao = new RoleRepository(_db, loggerFactory);
            lcdaDao = new LcdaRepository(_db, loggerFactory);
            userDao = new UserDao(_db);
            permissionDao = new PermissionRepository(_db);
            jwtOptions = _jwtOptions.Value;
        }

        public async Task<object> GetToken(User user, Guid lcdaId)
        {
            String domainName = "";
            RoleExtension role = null;
            string permissions = string.Empty;
            List<Claim> claimLst = new List<Claim>();
            if (user.username.ToLower() != "mos-admin")
            {
                Lgda ld = await lcdaDao.Get(lcdaId);

                if (ld != null)
                {
                    domainName = ld.lcdaName;
                    role = await roleDao.GetUserDomainRoleByUsername(user.username, lcdaId);
                    if (role != null)
                    {
                        List<Permission> permissionlst = await permissionDao.byRoleId(role.id);
                        claimLst.AddRange(permissionlst.Select(x => new Claim(ClaimTypes.Role, x.permissionName)));
                    }
                }
            }
            else
            {
                domainName = "mos-admin";
            }

            int logTime = int.TryParse(jwtOptions.logOutTIme, out logTime) ? logTime : 30;

            var tokenHandler = new JwtSecurityTokenHandler();

            claimLst.AddRange(new List<Claim>()
                {
                     new Claim(ClaimTypes.NameIdentifier, user.username),
                            new Claim(ClaimTypes.Name, $"{user.surname} {user.firstname} {user.lastname}"),
                            new Claim("Domain",lcdaId.ToString()),
                            new Claim("identity",EncryptDecryptUtils.ToHexString(user.id.ToString()))
                });

            var jwt = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Subject = new ClaimsIdentity(claimLst),
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
                domainName = domainName,
                role = role?.roleName,
                id = user.id
            };
        }

        public string GetToken(Claim[] claim)
        {
            List<Claim> claimLst = new List<Claim>();

            int logTime = int.TryParse(jwtOptions.logOutTIme, out logTime) ? logTime : 30;

            var tokenHandler = new JwtSecurityTokenHandler();

            claimLst.AddRange(new List<Claim>()
                {
                     new Claim(ClaimTypes.NameIdentifier, ClaimExtension.GetUsername(claim)),
                            new Claim(ClaimTypes.Name, ClaimExtension.GetName(claim)),
                            new Claim("Domain",ClaimExtension.GetDomainId(claim).ToString()),
                            new Claim("identity",ClaimExtension.GetHexId(claim))
                });

            claimLst.AddRange(claim.Where(x => x.Type == ClaimTypes.Role));

            var jwt = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.UtcNow.AddMinutes(logTime),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = jwtOptions.SigningCredentials,
                IssuedAt = DateTime.UtcNow,

            };

            return new JwtSecurityTokenHandler().WriteToken(tokenHandler.CreateToken(jwt));
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

        public async Task<bool> ChangeStatus(string status, Guid id)
        {
            return await userDao.ChangeStatus(status, id);
        }
    }
}

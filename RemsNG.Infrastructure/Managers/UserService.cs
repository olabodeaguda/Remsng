using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class UserManagers : IUserManagers
    {
        private readonly JwtIssuerOptions jwtOptions;
        private readonly LoginRepository loginDao;
        private readonly DomainRepository domainDao;
        private readonly RoleRepository roleDao;
        private readonly LcdaRepository lcdaDao;
        private readonly UserRepository userDao;
        private readonly PermissionRepository permissionDao;
        public UserManagers(RemsDbContext _db,
            IOptions<JwtIssuerOptions> _jwtOptions, ILoggerFactory loggerFactory)
        {
            loginDao = new LoginRepository(_db);
            domainDao = new DomainRepository(_db);
            roleDao = new RoleRepository(_db, loggerFactory);
            lcdaDao = new LcdaRepository(_db, loggerFactory);
            userDao = new UserRepository(_db);
            permissionDao = new PermissionRepository(_db);
            jwtOptions = _jwtOptions.Value;
        }

        public async Task<object> GetToken(UserModel user, Guid lcdaId)
        {
            String domainName = "";
            RoleExtensionModel role = null;
            string permissions = string.Empty;
            List<Claim> claimLst = new List<Claim>();
            if (user.Username.ToLower() != "mos-admin")
            {
                LcdaModel ld = await lcdaDao.Get(lcdaId);

                if (ld != null)
                {
                    domainName = ld.LcdaName;
                    role = await roleDao.GetUserDomainRoleByUsername(user.Username, lcdaId);
                    if (role != null)
                    {
                        List<PermissionModel> permissionlst = await permissionDao.byRoleId(role.id);
                        claimLst.AddRange(permissionlst.Select(x => new Claim(ClaimTypes.Role, x.PermissionName)));
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
                     new Claim(ClaimTypes.NameIdentifier, user.Username),
                            new Claim(ClaimTypes.Name, $"{user.Surname} {user.Firstname} {user.Lastname}"),
                            new Claim("Domain",lcdaId.ToString()),
                            new Claim("identity", Common.Utilities.EncryptDecryptUtils.ToHexString(user.Id.ToString()))
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
                username = user.Username,
                fullname = $"{user.Surname} {user.Firstname} {user.Lastname}",
                userStatus = user.UserStatus,
                domainName = domainName,
                role = role?.roleName,
                id = user.Id
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

        public async Task<UserModel> GetUserByUsername(string username)
        {
            return await userDao.ByUserName(username);
        }

        public async Task<UserModel> ByEmail(string email)
        {
            return await userDao.ByEmail(email);
        }

        public async Task<bool> Create(UserModel user)
        {
            return await userDao.Create(user);
        }

        public Task<bool> AddAndAssignLGDA(UserModel user, UserLcdaModel userLcda)
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

        public async Task<bool> Update(UserModel user)
        {
            return await userDao.Update(user);
        }

        public async Task<UserModel> Get(Guid id)
        {
            return await userDao.Get(id);
        }

        public async Task<bool> ChangePwd(Guid id, string newPwd)
        {
            return await userDao.ChangePwd(id, newPwd);
        }

        public async Task<bool> AssignLGDA(UserLcdaModel userLcda)
        {
            return await userDao.AssignLGDA(userLcda);
        }

        public async Task<bool> ChangeStatus(string status, Guid id)
        {
            return await userDao.ChangeStatus(status, id);
        }
    }
}

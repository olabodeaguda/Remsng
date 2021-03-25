using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class UserManager : IUserManager
    {
        private readonly JwtIssuerOptions _jOptions;
        private readonly ILoginRepository loginDao;
        private readonly IDomainRepository domainDao;
        private readonly IRoleRepository roleDao;
        private readonly ILcdaRepository lcdaDao;
        private readonly IUserRepository userDao;
        private readonly IPermissionRepository permissionDao;
        public UserManager(ILoggerFactory loggerFactory, 
            IOptions<JwtIssuerOptions> jOptions, ILoginRepository loginRepository,
            IDomainRepository domainRepository,
            IRoleRepository roleRepository, ILcdaRepository lcdaRepository,
            IUserRepository userRepository, IPermissionRepository permissionRepository)
        {
            loginDao = loginRepository;
            domainDao = domainRepository;
            roleDao = roleRepository;
            lcdaDao = lcdaRepository;
            userDao = userRepository;
            permissionDao = permissionRepository;
            _jOptions = jOptions.Value;
        }

        public async Task<object> GetToken(UserModel user, Guid lcdaId)
        {
            String domainName = "";
            RoleModel role = null;
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
                        List<PermissionModel> permissionlst = await permissionDao.byRoleId(role.Id);
                        claimLst.AddRange(permissionlst.Select(x => new Claim(ClaimTypes.Role, x.PermissionName)));
                    }
                }
            }
            else
            {
                domainName = "mos-admin";
            }

            int logTime = int.TryParse(_jOptions.logOutTIme, out logTime) ? logTime : 30;

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
                Issuer = _jOptions.Issuer,
                Audience = _jOptions.Audience,
                Subject = new ClaimsIdentity(claimLst),
                Expires = DateTime.UtcNow.AddMinutes(300),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = _jOptions.SigningCredentials,
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
                role = role?.RoleName,
                id = user.Id
            };
        }

        public string GetToken(Claim[] claim)
        {
            List<Claim> claimLst = new List<Claim>();

            int logTime = 30;// int.TryParse(jwtOptions.logOutTIme, out logTime) ? logTime : 30;

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
                Issuer = _jOptions.Issuer,
                Audience = _jOptions.Audience,
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.UtcNow.AddMinutes(logTime),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = _jOptions.SigningCredentials,
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

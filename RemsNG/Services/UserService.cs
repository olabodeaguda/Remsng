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

namespace RemsNG.Services
{
    public class UserService : IUserService
    {
        private readonly JwtIssuerOptions jwtOptions;
        private readonly LoginDao loginDao;
        private readonly DomainDao domainDao;
        private readonly RoleDao roleDao;
        public UserService(RemsDbContext _db,
            IOptions<JwtIssuerOptions> _jwtOptions)
        {
            loginDao = new LoginDao(_db);
            domainDao = new DomainDao(_db);
            roleDao = new RoleDao(_db);
            jwtOptions = _jwtOptions.Value;
        }

        public async Task<string> GetToken(User user)
        {
            Role role = await roleDao.GetUserRoleByUsername(user.username);
            List<Domain> domain = await domainDao.GetUserDomainByUsername(user.username);
            int logTime = int.TryParse(jwtOptions.logOutTIme, out logTime) ? logTime : 30;


            //var jwt = new JwtSecurityToken(
            //    issuer: jwtOptions.Issuer,
            //    audience: jwtOptions.Audience,
            //    claims: new Claim[]
            //            {
            //                new Claim(ClaimTypes.Name, $"{user.surname} {user.firstname} {user.lastname}"),
            //                new Claim(ClaimTypes.Role, role != null? role.roleName:string.Empty),
            //                new Claim("Domain",(domain.Count > 0?domain[0].domainCode:""))
            //            },
            //    notBefore: DateTime.UtcNow,
            //    expires: DateTime.UtcNow.AddMinutes(logTime),
            //    signingCredentials: jwtOptions.SigningCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = new SecurityTokenDescriptor
            {
                Issuer = jwtOptions.Issuer,
                Audience = jwtOptions.Audience,
                Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, $"{user.surname} {user.firstname} {user.lastname}"),
//                            new Claim(ClaimTypes.Role, role != null? role.roleName:string.Empty),
new Claim(ClaimTypes.Role, "author"),
                            new Claim("Domain",(domain.Count > 0?domain[0].domainCode:"")),
                            new Claim("identity",EncryptDecryptUtils.ToHexString(user.id.ToString()))
                        }),
                Expires = DateTime.UtcNow.AddMinutes(logTime),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = jwtOptions.SigningCredentials,
                IssuedAt = DateTime.UtcNow,

            };

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(tokenHandler.CreateToken(jwt));
            return encodedJwt;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await loginDao.GetUser(username);
        }
    }
}

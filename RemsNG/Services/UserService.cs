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

namespace RemsNG.Services
{
    public class UserService : IUserService
    {
        private readonly JwtIssuerOptions jwtOptions;
        private readonly LoginDao loginDao;
        private readonly DomainDao domainDao;
        public UserService(RemsDbContext _db,
            IOptions<JwtIssuerOptions> _jwtOptions)
        {
            loginDao = new LoginDao(_db);
            domainDao = new DomainDao(_db);
            jwtOptions = _jwtOptions.Value;
        }

        public async Task<string> GetToken(User user)
        {
            List<Domain> domain = await domainDao.GetUserDomainByUsername(user.username);
            int logTime = int.TryParse(jwtOptions.logOutTIme, out logTime) ? logTime : 30;


            var jwt = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: new Claim[]
                        {
                            new Claim(ClaimTypes.Name, $"{user.surname} {user.firstname} {user.lastname}"),
                            new Claim(ClaimTypes.Role, "Author"),
                            new Claim("Domain",(domain.Count > 0?domain[0].domainCode:""))
                        },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(logTime),
                signingCredentials: jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await loginDao.GetUser(username);
        }
    }
}

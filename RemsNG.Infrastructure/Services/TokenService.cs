using RemsNG.Common.Interfaces.Services;
using RemsNG.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace RemsNG.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtIssuerOptions jOptions;
        public string GenerateToken(List<Claim> lst)
        {
            throw new System.NotImplementedException();
        }
    }
}

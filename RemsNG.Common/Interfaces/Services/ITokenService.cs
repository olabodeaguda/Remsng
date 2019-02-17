using System.Collections.Generic;
using System.Security.Claims;

namespace RemsNG.Common.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(List<Claim> lst);
    }
}

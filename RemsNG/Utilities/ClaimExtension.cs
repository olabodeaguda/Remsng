using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class ClaimExtension
    {
        public static bool IsMosAdmin(List<Claim> claims)
        {
            var hasClaim = claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");

            if (hasClaim)
            {
                return true;
            }
            return false;
        }

        public static Guid GetDomain(List<Claim> claims)
        {
            var domainId = claims.FirstOrDefault(x => x.Type == "Domain");
            if (domainId != null)
            {
                Guid dId = Guid.Empty;
                bool v = Guid.TryParse(domainId.Value, out dId);
                if (v)
                {
                    return dId;
                }
            }

            return Guid.Empty;
        }
    }
}

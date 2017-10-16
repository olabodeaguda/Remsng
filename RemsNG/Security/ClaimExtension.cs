using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Security
{
    public class ClaimExtension
    {
        public static bool IsMosAdmin(Claim[] claims)
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
                string dd = domainId.Value;
                Guid dId = Guid.Empty;
                bool v = Guid.TryParse(domainId.Value, out dId);
                if (v)
                {
                    return dId;
                }
            }

            return Guid.Empty;
        }

        public static string GetUsername(Claim[] claim)
        {
            var username = claim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (username != null)
            {
                return username.Value;
            }

            return string.Empty;
        }

        public static Guid GetDomainId(Claim[] claim)
        {
            var domainId = claim.FirstOrDefault(x => x.Type == "Domain");
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

        public static Guid UserId(Claim[] claims)
        {
            Claim userIdEncrypt = claims.FirstOrDefault(x => x.Type == "identity");
            if (userIdEncrypt != null)
            {
                string userId = EncryptDecryptUtils.FromHexString(userIdEncrypt.Value);
                Guid dId = Guid.Empty;
                bool v = Guid.TryParse(userId, out dId);
                if (v)
                {
                    return dId;
                }
            }
            return Guid.Empty;
        }


    }
}

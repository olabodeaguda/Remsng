using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class RemsAuthorizationFilter : TypeFilterAttribute
    {
        public RemsAuthorizationFilter(string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(ClaimTypes.Role, claimValue) };
        }
    }
}

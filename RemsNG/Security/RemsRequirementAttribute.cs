using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Security
{
    public class RemsRequirementAttribute : TypeFilterAttribute
    {
        public RemsRequirementAttribute(string claimValue) : base(typeof(RemsRequirementFilter))
        {
            Arguments = new object[] { new Claim(ClaimTypes.Role, claimValue) };
        }
    }
}

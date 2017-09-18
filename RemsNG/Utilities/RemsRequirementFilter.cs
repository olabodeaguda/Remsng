using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class RemsRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public RemsRequirementFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);

            if (!hasClaim)
            {
                hasClaim = context.HttpContext.User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");
            }
            if (!hasClaim)
            {
                context.Result = new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FORBIDDEN,
                    description = "You have no access to this request"
                }, 403);// new ForbidResult();
            }
        }
    }
}

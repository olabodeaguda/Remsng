using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Security
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
            if (context.HttpContext.User.Claims.Count() < 1)
            {
                context.Result = new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.INVALID_TOKEN,
                    description = "Please re-login. access have expired"
                }, 403);
            }
            else
            {
                Claim claim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == _claim.Type);
                if (claim != null)
                {

                }

                var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);


                if (!hasClaim)
                {
                    hasClaim = context.HttpContext.User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");
                }
                if (!hasClaim)
                {
                    throw new ForbidException("You have no access to this request");
                    //context.Result = new HttpMessageResult(new Response()
                    //{
                    //    code = MsgCode_Enum.FORBIDDEN,
                    //    description = "You have no access to this request"
                    //}, 403);// new ForbidResult();
                }
            }
        }
    }
}

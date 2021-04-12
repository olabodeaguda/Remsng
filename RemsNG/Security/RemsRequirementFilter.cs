using Microsoft.AspNetCore.Mvc.Filters;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Filters;
using System.Linq;
using System.Security.Claims;

namespace RemsNG.Security
{
    public class RemsRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;
        readonly IRoleManager _roleManager;
        public RemsRequirementFilter(Claim claim, IRoleManager roleManager)
        {
            _roleManager = roleManager;
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
                var userId = context.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "identity");

                if (userId == null)
                    context.Result = new HttpMessageResult(new Response()
                    {
                        code = MsgCode_Enum.FORBIDDEN,
                        description = "Please re-login. access have expired"
                    }, 403);

                bool hasClaim = _roleManager.IsPermited(userId.Value, _claim.Value).Result;

                //var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);

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
                    }, 403);
                }
            }
        }
    }
}

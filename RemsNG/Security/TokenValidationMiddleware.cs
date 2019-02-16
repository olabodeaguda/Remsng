using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Security
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate next;
        private static ILoggerFactory logger;
        public TokenValidationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory, IUserManagers userService)
        {
            logger = loggerFactory;
            try
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    string token = context.Request.Headers["Authorization"];
                    if (token != null)
                    {
                        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                        token = token.Replace("Bearer ", "");
                        try
                        {
                            JwtSecurityToken jst = handler.ReadJwtToken(token);
                            DateTime validTo = jst.ValidTo;
                            int compareDate = validTo.CompareTo(DateTime.UtcNow);
                            double times = DateTime.UtcNow.Subtract(validTo).TotalMinutes;
                            if (compareDate < 0 && times <= 10)
                            {
                                // change token
                                var claims = new List<Claim>(jst.Claims);
                                string newToken = userService.GetToken(claims.ToArray());
                                if (!string.IsNullOrEmpty(newToken))
                                {
                                    token = newToken;
                                    IHeaderDictionary headers = context.Response.Headers;
                                    headers["Access-Control-Expose-Headers"] = $"\'new-t\'";
                                    headers["new-t"] = newToken;
                                }
                            }

                            context.User = handler.ValidateToken(token,
                                ServicesCollection.tokenValidationParameters(),
                                out SecurityToken validatedToken);
                            await next(context);
                        }
                        catch (Exception ex)
                        {
                            await ErrorHandlingMiddleware.HandleExceptionAsync(context, ex);
                        }
                    }
                    else
                    {
                        await next(context);
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorHandlingMiddleware.HandleExceptionAsync(context, ex);
            }
        }
    }
}

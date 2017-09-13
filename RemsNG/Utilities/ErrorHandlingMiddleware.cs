using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RemsNG.Exceptions;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private static ILogger logger;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {

            try
            {

                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Response response = new Response();
            context.Response.ContentType = "application/json";

            if (exception.GetType() == typeof(SecurityTokenValidationException))
            {
                response.description = "Invalid token";
                response.code = MsgCode_Enum.INVALID_TOKEN;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception.GetType() == typeof(SecurityTokenInvalidIssuerException))
            {
                response.description = "Invalid issuer";
                response.code = MsgCode_Enum.INVALID_ISSUER;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                response.description = "Token expired";
                response.code = MsgCode_Enum.TOKEN_EXPIRED;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception.GetType() == typeof(NotFoundException))
            {
                response.description = exception.Message;
                response.code = MsgCode_Enum.NOTFOUND;
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                response.description = exception.Message;
                response.code = MsgCode_Enum.UNKNOWN;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var result = JsonConvert.SerializeObject(response);
           // logger.LogError(result, new object[] { exception, context.Response.StatusCode });
            return context.Response.WriteAsync(result);
        }
    }
}

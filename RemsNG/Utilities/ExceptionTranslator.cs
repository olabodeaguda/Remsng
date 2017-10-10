using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RemsNG.Exceptions;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class ExceptionTranslator
    {
        private ILogger logger;
        public ExceptionTranslator(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("ExceptionTranslator");
        }
        public void Translate(HttpContext context, Exception exception, Response response)
        {
             context.Response.ContentType = "application/json";
            
            if (exception.GetType() == typeof(SecurityTokenValidationException) || exception.GetType() == typeof(UserValidationException))
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
            else if (exception.GetType() == typeof(AlreadyExistException))
            {
                response.description = exception.Message;
                response.code = MsgCode_Enum.NOTFOUND;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            else if(exception.GetType() == typeof(InvalidOperationException))
            {
                response.description = "Database error";
                response.code = MsgCode_Enum.DATABASE_ERROR;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                response.description = exception.Message;
                response.code = MsgCode_Enum.UNKNOWN;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            logger.LogError(exception, $"{context.Response.StatusCode}");

        }
    }
}

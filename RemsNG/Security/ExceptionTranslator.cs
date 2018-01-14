using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RemsNG.Exceptions;
using RemsNG.Models;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemsNG.Security
{
    public class ExceptionTranslator
    {
        private ILogger logger;
        public ExceptionTranslator(ILoggerFactory loggerFactory)
        {
            if (loggerFactory != null)
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
            else if (exception.GetType() == typeof(InvalidOperationException))
            {
                response.description = "An Internal error occur, Try again or contact administrator ";
                response.code = MsgCode_Enum.INTERNAL_ERROR;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            else if (exception.GetType() == typeof(DuplicateException))
            {
                response.description = exception.Message;
                response.code = MsgCode_Enum.DUPLICATE;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }

            else if (exception.GetType() == typeof(DuplicateCompanyException))
            {
                response.description = exception.Message;
                response.code = MsgCode_Enum.DUPLICATE_COMPANY;
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else if (exception.GetType() == typeof(ForbidException))
            {
                response.description = exception.Message;
                response.code = MsgCode_Enum.FORBIDDEN;
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
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

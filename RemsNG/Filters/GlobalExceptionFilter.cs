using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemsNG.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var content = GetStatusCode<object>(context.Exception);
            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = (int)content.Item2;
            response.ContentType = "application/json";

            context.Result = new JsonResult(content.Item1);
        }

        public static (Response responseModel, HttpStatusCode statusCode) GetStatusCode<T>(Exception exception)
        {
            switch (exception)
            {
                case BaseException bex:
                    return (new Response
                    {
                        code = MsgCode_Enum.FAIL,
                        description = exception.Message,
                        status = false
                    }, bex.httpStatusCode);
                case SecurityTokenExpiredException bex:
                    return (new Response
                    {
                        code = MsgCode_Enum.INVALID_TOKEN,
                        description = exception.Message,
                        status = false
                    }, HttpStatusCode.Unauthorized);
                case SecurityTokenInvalidIssuerException bex:
                    return (new Response
                    {
                        code = MsgCode_Enum.INVALID_TOKEN,
                        description = exception.Message,
                        status = false
                    }, HttpStatusCode.Unauthorized);
                case SecurityTokenValidationException bex:
                    return (new Response
                    {
                        code = MsgCode_Enum.INVALID_TOKEN,
                        description = exception.Message,
                        status = false
                    }, HttpStatusCode.Unauthorized);
                default:
                    return (new Response
                    {
                        code = MsgCode_Enum.FAIL,
                        description = exception.Message,
                        status = false
                    }, HttpStatusCode.InternalServerError);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RemsNG.Common.Models;
using RemsNG.Exceptions;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemsNG.Security
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private static ILoggerFactory logger;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context , ILoggerFactory loggerFactory)
        {
            logger = loggerFactory;
            try
            {
                await next(context);    
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        public static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Response response = new Response();
            ExceptionTranslator ex = new ExceptionTranslator(logger);
            ex.Translate(context, exception, response);
            var result = JsonConvert.SerializeObject(response);
            return context.Response.WriteAsync(result);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task Invoke(HttpContext context, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory;
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await ErrorHandlingMiddleware.HandleExceptionAsync(context, ex);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using RemsNG.Models;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Security
{
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILogger _logger;

        public GlobalExceptionFilter(ILoggerFactory logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this._logger = logger.CreateLogger("Global Exception Filter");
        }

        public void Dispose()
        {
            
        }

        public void OnException(ExceptionContext context)
        {
            var response = new Response()
            {
                 description = context.Exception.Message,
                 code = MsgCode_Enum.FAIL
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = 500,
                 DeclaredType = typeof(Response)
            };

            this._logger.LogError("GlobalExceptionFilter", context.Exception);
        }
    }
}

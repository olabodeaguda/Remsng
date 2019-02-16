using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Models;
using RemsNG.Exceptions;
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
        public GlobalExceptionFilter()
        {
        }

        public void Dispose()
        {

        }

        public void OnException(ExceptionContext context)
        {
            var response = new Response();

            throw new GlobalExceptionHandler(context.Exception.Message);
        }
    }
}

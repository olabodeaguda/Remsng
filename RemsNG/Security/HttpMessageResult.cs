using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Security
{
    public class HttpMessageResult : IActionResult
    {
        private readonly object message;
        private readonly int httpCode;

        public HttpMessageResult(object _message,int _httpCode)
        {
            message = _message;
            httpCode = _httpCode;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = httpCode;

            var myByteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await context.HttpContext.Response.Body.WriteAsync(myByteArray, 0, myByteArray.Length);
            await context.HttpContext.Response.Body.FlushAsync();
        }
    }
}

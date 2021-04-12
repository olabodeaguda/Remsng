using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Filters
{
    public class ForbiddenObjectResult : ObjectResult
    {
        public ForbiddenObjectResult(object value)
                : base(value)
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}

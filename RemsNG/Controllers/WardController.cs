using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Utilities;
using RemsNG.Services.Interfaces;
using System.Security.Claims;
using RemsNG.Models;
using RemsNG.ORM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/ward")]
    public class WardController : Controller
    {
        private readonly IWardService wardService;
        public WardController(IWardService _wardservice)
        {
            wardService = _wardservice;
        }

        [Route("all")]
        [RemsRequirementAttribute("GET_LCDA")]
        [HttpGet]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            var hasClaim = User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");
            if (hasClaim)
            {
                pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
                pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
                return await wardService.Paginated(new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
            }
            else
            {
                var domainId = User.Claims.FirstOrDefault(x => x.Type == "Domain");
                if (domainId != null)
                {
                    Guid dId = Guid.Empty;
                    bool v = Guid.TryParse(domainId.Value, out dId);
                    if (v)
                    {
                        List<Ward> cd = await wardService.GetWardByLGDAId(Id);
                        return cd;
                    }
                }
            }
            return new object[] { };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

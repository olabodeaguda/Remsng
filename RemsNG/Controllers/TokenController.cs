using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using RemsNG.Models;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            byte[] obj = Convert.FromBase64String(value);
            string jsonValue = Encoding.UTF8.GetString(obj);
            LoginModel ln = JsonConvert.DeserializeObject<LoginModel>(jsonValue);
            if (string.IsNullOrEmpty(ln.username))
            {

            }
        }
    }
}

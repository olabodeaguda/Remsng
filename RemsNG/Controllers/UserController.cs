using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using RemsNG.Models;
using Newtonsoft.Json;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using RemsNG.ORM;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly ILogger logger;
        private readonly IUserService userService;
        public UserController(IUserService _userService, ILoggerFactory loggerFactory)
        {
            userService = _userService;
            logger = loggerFactory.CreateLogger<UserController>();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader]string value)
        {
            byte[] obj = Convert.FromBase64String(value);
            string jsonValue = Encoding.UTF8.GetString(obj);
            LoginModel ln = JsonConvert.DeserializeObject<LoginModel>(jsonValue);
            if (string.IsNullOrEmpty(ln.username))
            {
                return BadRequest(new Response() { code = MsgCode_Enum.INVALID_PARAMETER_PASSED, data = "Username is required!!" });
            }
            User user = await userService.GetUserByUsername(ln.username);
            if (user == null)
            {
                logger.LogError($"{ln.username} authentication failed, user does not exist", new object[] { ln.username });
                return NotFound(new Response() { code = MsgCode_Enum.NOTFOUND, data = $"{ln.username} does not exist" });
            }

            if (user.passwordHash != EncryptDecryptUtils.ToHexString(ln.pwd))
            {
                logger.LogError($"{ln.username} password is incorrect", new object[] { ln.username });
                return new HttpMessageResult("Password is incorrect", 401);
            }

            if (user.userStatus != UserStatus.ACTIVE.ToString())
            {
                return BadRequest(new Response() { code = MsgCode_Enum.INACTIVE_USER, data = "Username is not active" });
            }

            //generate jwt
            string token = await userService.GetToken(user, ln.domainId, ln.domainId == Guid.Empty ? false : true);

            Response response = new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = new
                {
                    tk = token,
                    firstname = user.firstname,
                    surname = user.lastname,
                    lastname = user.lastname,
                    userStatus = user.userStatus
                }
            };

            return Ok(response);
        }
    }
}

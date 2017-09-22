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

namespace RemsNG.Controllers
{
    [Route("api/v1/user")]
    public class UserController : Controller
    {
        private readonly ILogger logger;
        private readonly IUserService userService;
        private readonly IDomainService domainService;
        public UserController(IUserService _userService,
            ILoggerFactory loggerFactory, IDomainService _domainService)
        {
            userService = _userService;
            domainService = _domainService;
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
            object token = await userService.GetToken(user, ln.domainId);

            Response response = new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = token
            };

            return Ok(response);
        }
    }
}

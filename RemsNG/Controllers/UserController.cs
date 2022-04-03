using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemsNG.Common.Exceptions;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Route("api/v1/user")]
    public class UserController : Controller
    {
        private readonly ILogger logger;
        private readonly IUserManager userService;
        private readonly IDomainManager domainService;
        private readonly ILcdaManager lcdaService;
        private readonly IRoleManager roleService;
        public UserController(IUserManager _userService,
            ILoggerFactory loggerFactory, IDomainManager _domainService,
            ILcdaManager _lcdaService, IRoleManager _roleservice)
        {
            userService = _userService;
            domainService = _domainService;
            lcdaService = _lcdaService;
            roleService = _roleservice;
            logger = loggerFactory.CreateLogger<UserController>();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Bad request"
                });
            }

            UserModel user = await userService.Get(id);
            user.PasswordHash = null;

            if (user == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "User Profile could not be found"
                });
            }

            return Ok(new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = user
            });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromHeader]string value)
        {
            if (DateTime.Now.CompareTo(new DateTime(2023, 2, 27)) > 0)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = $"Licence expired. Please contact your administrator for renewal"
                });
            }
            byte[] obj = Convert.FromBase64String(value);
            string jsonValue = Encoding.UTF8.GetString(obj);
            LoginModel ln = JsonConvert.DeserializeObject<LoginModel>(jsonValue);
            if (string.IsNullOrEmpty(ln.username))
            {
                return BadRequest(new Response() { code = MsgCode_Enum.INVALID_PARAMETER_PASSED, data = "Username is required!!" });
            }
            UserModel user = await userService.GetUserByUsername(ln.username);
            if (user == null)
            {
                logger.LogError($"{ln.username} authentication failed, user does not exist", new object[] { ln.username });
                return NotFound(new Response() { code = MsgCode_Enum.NOTFOUND, data = $"{ln.username} does not exist" });
            }

            string s = Common.Utilities.EncryptDecryptUtils.ToHexString(ln.pwd);
            if (user.PasswordHash != Common.Utilities.EncryptDecryptUtils.ToHexString(ln.pwd))
            {
                logger.LogError($"{ln.username} password is incorrect", new object[] { ln.username });
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Password is incorrect"
                }, 401);
            }

            if (user.UserStatus != UserStatus.ACTIVE.ToString())
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

        [RemsRequirementAttribute("ASSIGN_DOMAIN")]
        [Route("assignlgda")]
        [HttpPost]
        public async Task<object> AssignToUser([FromBody]UserLcdaModel userLcda)
        {
            if (userLcda.LgdaId == default(Guid))
            {
                throw new InvalidCredentialsException("LGDA is required");
            }
            else if (userLcda.UserId == default(Guid))
            {
                throw new InvalidCredentialsException("User is required");
            }

            UserModel user = await userService.Get(userLcda.UserId);
            if (user == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "User not found"
                });
            }

            LcdaModel lcda = await lcdaService.Get(userLcda.LgdaId);
            if (lcda == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Select LGDA not found"
                });
            }
            else if (lcda.LcdaStatus != UserStatus.ACTIVE.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = $"{lcda.LcdaName} is not Active"
                });
            }

            UserLcdaModel lg = await lcdaService.UserLcdaByIds(lcda.Id, user.Id);
            if (lg != null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = $"{user.Username} already exist int LCDA {lcda.LcdaName}"
                });
            }

            bool result = await userService.AssignLGDA(userLcda);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{user.Username} have been assigned to {lcda.LcdaName}"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur. Please refresh the page or try again later"
                });
            }
        }

        [RemsRequirementAttribute("GET_PROFILE")]
        [Route("profiles")]
        [HttpGet]
        public async Task<object> Profiles([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                return await userService.Paginated(new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                });
            }
            else
            {
                Guid lcdaId = ClaimExtension.GetDomain(User.Claims.ToList());
                if (lcdaId == Guid.Empty)
                {
                    return new object[] { };
                }

                return await userService.Paginated(new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                }, lcdaId);
            }
        }

        [RemsRequirementAttribute("ADD_PROFILE")]
        [Route("add")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserModel user)
        {
            UserModel username = await userService.GetUserByUsername(user.Username);
            UserModel email = await userService.ByEmail(user.Email);
            List<LcdaModel> lst = await lcdaService.byUsername(user.Username);

            user.Id = Guid.NewGuid();
            user.DateCreated = DateTime.Now;
            user.CreatedBy = User.Identity.Name;
            bool isAdded = false;

            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                if (username != null)
                {
                    throw new AlreadyExistException("Username already Exist");
                }
                else if (email != null)
                {
                    throw new AlreadyExistException("Email already Exist");
                }
                user.UserStatus = UserStatus.ACTIVE.ToString();
                isAdded = await userService.Create(user);
            }
            else
            {
                Guid domainId = ClaimExtension.GetDomain(User.Claims.ToList());
                if (domainId == Guid.Empty)
                {
                    throw new InvalidCredentialsException("Logon user must be in a valid local government development authority");
                }

                if (username != null && lst.Any(x => x.Id == domainId))
                {
                    throw new AlreadyExistException("Username already Exist");
                }
                else if (email != null && lst.Any(x => x.Id == domainId))
                {
                    throw new AlreadyExistException("Email already Exist");
                }

                UserLcdaModel userLcda = new UserLcdaModel()
                {
                    UserId = user.Id,
                    LgdaId = domainId
                };
                user.UserStatus = UserStatus.NOT_ACTIVE.ToString();
                isAdded = await userService.AddAndAssignLGDA(user, userLcda);
            }

            if (isAdded)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{user.Username} have been added successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Failed, Please try again or contact administrator for help"
                }, 409);
            }
        }

        [RemsRequirementAttribute("UPDATE_PROFILE")]
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UserModel user)
        {
            user.LastModifiedDate = DateTime.Now;
            user.Lastmodifiedby = User.Identity.Name;
            if (user.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Wrong request"
                });
            }

            bool result = await userService.Update(user);

            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{user.Username} has been updated successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur, Please try again or notify an administrator"
                }, 409);
            }
        }

        [RemsRequirementAttribute("CHANGE_USER_PWD")]
        [Route("changepwdchange")]
        [HttpPost]
        public async Task<IActionResult> ChangeUserPassword([FromHeader] string value)
        {
            byte[] obj = Convert.FromBase64String(value);
            string jsonValue = Encoding.UTF8.GetString(obj);
            ChangePasswordModel ln = JsonConvert.DeserializeObject<ChangePasswordModel>(jsonValue);

            if (!ln.confirmPwd.Equals(ln.newPwd))
            {
                throw new InvalidCredentialsException("Please re-confirm your password");
            }

            bool result = await userService.ChangePwd(ln.id, ln.newPwd);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Password has been changed successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Password change failed. Please contact administrator or try again"
                }, 409);
            }
        }

        [RemsRequirementAttribute("UPDATE_PROFILE")]
        [Route("changestatus")]
        [HttpPost]
        public async Task<IActionResult> ChangeUserStatus([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.UserStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "User status is required"
                });
            }
            else if (user.Id == Guid.Empty)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Invalid request"
                });
            }

            bool result = await userService.ChangeStatus(user.UserStatus, user.Id);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "request has been treated successfully"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Please referesh the page and try again"
                });
            }
        }
    }
}

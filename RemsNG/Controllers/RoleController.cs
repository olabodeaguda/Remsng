using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/role")]
    public class RoleController : Controller
    {
        IRoleManager roleservice;
        IPermissionManager permissionService;
        public RoleController(IRoleManager _roleservice, IPermissionManager _permissionService)
        {
            roleservice = _roleservice;
            permissionService = _permissionService;
        }

        // [RemsRequirementAttribute("GET_ROLES")]
        [HttpGet]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            if (string.IsNullOrEmpty(pageSize))
            {
                pageSize = "50";
            }
            if (string.IsNullOrEmpty(pageNum))
            {
                pageNum = "1";
            }
            var hasClaim = User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");
            if (hasClaim)
            {
                //mos-admin
                return await roleservice.Paginated(new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                });
            }
            else
            {
                //normal user
                string username = ClaimExtension.GetUsername(User.Claims.ToArray());
                if (username == string.Empty)
                {
                    return new HttpMessageResult(new Response()
                    {
                        code = MsgCode_Enum.INVALID_TOKEN,
                        description = "Username is invalid"
                    }, 403);
                }
                Guid domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
                if (domainId == Guid.Empty)
                {
                    return new HttpMessageResult(new Response()
                    {
                        code = MsgCode_Enum.WRONG_CREDENTIALS,
                        description = "Username is invalid"
                    }, 403);
                }

                return await roleservice.Paginated(new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                }, domainId);
            }
        }

        [Route("alldomainroles")]
        [HttpGet]
        public async Task<object> AllUserRoles([FromHeader] string username)
        {
            var hasClaim = User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier && x.Value.ToLower() == "mos-admin");
            if (hasClaim)
            {
                return await roleservice.AllDomainRolesByUsername(username);
            }
            else
            {
                Guid domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
                if (domainId == Guid.Empty)
                {
                    return NotFound(new Response()
                    {
                        code = MsgCode_Enum.WRONG_CREDENTIALS,
                        description = "Username is invalid"
                    });
                }

                return roleservice.ByDomainId(domainId);
            }
        }

        [RemsRequirementAttribute("ADD_ROLES")]
        [HttpPost]
        public async Task<object> Post([FromBody] RoleModel role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role name is required"
                });
            }
            if (!ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                role.DomainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            }

            if (role.DomainId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "User Domain can not be found.. Please log in again"
                }, 403);
            }
            role.Id = Guid.NewGuid();
            role.RoleStatus = UserStatus.ACTIVE.ToString();

            bool result = await roleservice.Add(role);
            if (result)
            {
                return Created("/role", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{role.RoleName} have been added successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request Failed, Please try again or contact administrator"
                }, 403);
            }
        }

        [Route("update")]
        [RemsRequirementAttribute("UPDATE_ROLES")]
        [HttpPost]
        public async Task<object> Update([FromBody] RoleModel role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role name is required"
                });
            }

            if (role.DomainId == Guid.Empty)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "User Domain can not be found.. Please log in again"
                });
            }

            if (role.Id == Guid.Empty)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Selected role can not be found"
                });
            }

            bool result = await roleservice.Update(role);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"Update was successful"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request Failed, Please try again or contact administrator"
                }, 409);
            }
        }

        [Route("changestatus")]
        [RemsRequirementAttribute("CHANGE_STATUS")]
        [HttpPost]
        public async Task<object> ChangeStatus([FromBody] RoleModel role)
        {
            if (role.Id == Guid.Empty)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Selected role is invalid"
                });
            }
            else if (string.IsNullOrEmpty(role.RoleStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role status is required"
                });
            }

            var r = await roleservice.GetById(role.Id);
            if (r == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Role not found"
                });
            }
            else if (r.RoleStatus == role.RoleStatus)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Status is the same"
                });
            }
            bool result = await roleservice.UpdateStatus(role);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Update was successful"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request Failed, Please try again or contact administrator"
                }, 409);
            }

        }

        [Route("assignrole")]
        [RemsRequirementAttribute("ASSIGN_ROLES")]
        [HttpPost]
        public async Task<object> AssignRoleToUser([FromBody] UserRoleModel userRole)
        {
            var role = await roleservice.GetById(userRole.RoleId);
            if (role == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Role not found"
                });
            }
            else if (role.RoleStatus != UserStatus.ACTIVE.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Selected role is not active"
                });
            }
            RoleModel roleex = await roleservice.UserDomainRolesByDomainId(userRole.UserId, role.DomainId);

            if (roleex != null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "User already have an active role"
                });
            }

            UserRoleModel ur = await roleservice.GetUserRoleAsync(userRole.UserId, userRole.RoleId);
            if (ur != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = "Role already exist for the selected user"
                }, 409);
            }

            bool result = await roleservice.AssignRoleToUserAsync(userRole);
            if (result)
            {
                return Created("/role", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Request is successfull"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request Failed. Please try again or contact an administrator"
                }, 409);
            }
        }

        [Route("assignroletopermission")]
        [RemsRequirementAttribute("MANAGE_PERMISSION")]
        [HttpPost]
        public async Task<object> AssignPermissionToRole([FromBody] RolePermissionModel rolePermission)
        {
            if (rolePermission.PermissionId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Permission is required"
                }, 409);
            }
            else if (rolePermission.RoleId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role is required"
                }, 409);
            }
            RolePermissionModel pem = await permissionService.ByPermissionAndRoleId(rolePermission);
            if (pem != null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = "Permission already exist in role"
                }, 409);
            }

            bool result = await roleservice.Add(rolePermission);
            if (result)
            {
                return Created("/role", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Request is succesful"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Request Failed. Please try again or contact administrator"
                }, 409);
            }
        }

        [Route("removerolepermission")]
        [RemsRequirementAttribute("MANAGE_PERMISSION")]
        [HttpPost]
        public async Task<object> removeRolePermission([FromBody] RolePermissionModel rolePermission)
        {
            if (rolePermission.PermissionId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Permission is required"
                }, 409);
            }
            else if (rolePermission.RoleId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role is required"
                }, 409);
            }

            RolePermissionModel pem = await permissionService.ByPermissionAndRoleId(rolePermission);
            if (pem == null)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = "Permission does not exist"
                }, 409);
            }

            bool result = await permissionService.RemovePermission(pem);
            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Permission has been remove successfully"
                });
            }
            else
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.DUPLICATE,
                    description = "An error occur while trying to remove permission. Please contact administrator or retry"
                }, 409);
            }
        }

        [Route("{id}")]
        [Authorize]
        [HttpGet]
        public async Task<object> Get([FromRoute] Guid id)
        {
            RoleModel role = await roleservice.GetById(id);
            if (role == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Role not found"
                });
            }
            return role;
        }

        [Authorize]
        [HttpGet]
        [Route("currentrole/{id}")]
        public async Task<object> CurrentUserRole([FromRoute] Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Bad request"
                });
            }

            List<RoleModel> roleExtensions = await roleservice.AllRoleByUserId(id);
            Response response = new Response();
            response.code = MsgCode_Enum.SUCCESS;
            Guid domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                response.data = roleExtensions;
            }
            else
            {
                response.data = roleExtensions.Where(x => x.DomainId == domainId);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("currentuserdomainrole/{domainId}/{userId}")]
        public async Task<object> CurrentUserRole([FromRoute] Guid domainId, [FromRoute] Guid userId)
        {
            if (domainId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Domain is required"
                });
            }
            else if (userId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "User is required"
                });
            }

            RoleModel roleExtension = await roleservice.UserDomainRolesByDomainId(userId, domainId);

            return Ok(new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = roleExtension
            });
        }

        [RemsRequirementAttribute("REMOVE_ROLE")]
        [HttpPost]
        [Route("remove")]
        public async Task<object> RemoveRole([FromBody] UserRoleModel userRole)
        {
            UserRoleModel ur = await roleservice.GetUserRoleAsync(userRole.UserId, userRole.RoleId);
            if (ur == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Request not found"
                });
            }

            bool result = await roleservice.Remove(userRole);

            if (result)
            {
                return Ok(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Remove was successful"
                });
            }

            return new HttpMessageResult(new Response()
            {
                code = MsgCode_Enum.FAIL,
                description = ""
            }, 409);
        }

        [HttpGet]
        [Authorize]
        [Route("domainroles/{domainId}")]
        public async Task<object> DomainRoles(Guid domainId)
        {
            if (domainId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await roleservice.ByDomainId(domainId);
        }
    }
}

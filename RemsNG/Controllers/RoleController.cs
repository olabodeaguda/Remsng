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
using Microsoft.AspNetCore.Authorization;
using RemsNG.Security;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/role")]
    public class RoleController : Controller
    {
        IRoleService roleservice;
        IPermission permissionService;
        public RoleController(IRoleService _roleservice, IPermission _permissionService)
        {
            roleservice = _roleservice;
            permissionService = _permissionService;
        }

        [RemsRequirementAttribute("GET_ROLES")]
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
        [RemsRequirementAttribute("GET_ROLES")]
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
        public async Task<object> Post([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.roleName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role name is required"
                });
            }
            if (!ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                role.domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            }

            if (role.domainId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "User Domain can not be found.. Please log in again"
                }, 403);
            }
            role.id = Guid.NewGuid();
            role.roleStatus = UserStatus.ACTIVE.ToString();

            bool result = await roleservice.Add(role);
            if (result)
            {
                return Created("/role", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = $"{role.roleName} have been added successfully"
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
        public async Task<object> Update([FromBody] Role role)
        {
            if (string.IsNullOrEmpty(role.roleName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role name is required"
                });
            }

            if (role.domainId == Guid.Empty)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "User Domain can not be found.. Please log in again"
                });
            }

            if (role.id == Guid.Empty)
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
        public async Task<object> ChangeStatus([FromBody] Role role)
        {
            if (role.id == Guid.Empty)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Selected role is invalid"
                });
            }
            else if (string.IsNullOrEmpty(role.roleStatus))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role status is required"
                });
            }

            var r = await roleservice.GetById(role.id);
            if (r == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Role not found"
                });
            }
            else if (r.roleStatus == role.roleStatus)
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
        public async Task<object> AssignRoleToUser([FromBody] UserRole userRole)
        {
            var role = await roleservice.GetById(userRole.roleid);
            if (role == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Role not found"
                });
            }
            else if (role.roleStatus != UserStatus.ACTIVE.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Selected role is not active"
                });
            }
            RoleExtension roleex = await roleservice.UserDomainRolesByDomainId(userRole.userid, role.domainId);

            if (roleex != null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "User already have an active role"
                });
            }

            UserRole ur = await roleservice.GetUserRoleAsync(userRole.userid, userRole.roleid);
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
        public async Task<object> AssignPermissionToRole([FromBody] RolePermission rolePermission)
        {
            if (rolePermission.permissionId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Permission is required"
                }, 409);
            }
            else if (rolePermission.roleId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role is required"
                }, 409);
            }
            RolePermission pem = await permissionService.ByPermissionAndRoleId(rolePermission);
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
        public async Task<object> removeRolePermission([FromBody] RolePermission rolePermission)
        {
            if (rolePermission.permissionId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Permission is required"
                }, 409);
            }
            else if (rolePermission.roleId == Guid.Empty)
            {
                return new HttpMessageResult(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Role is required"
                }, 409);
            }

            RolePermission pem = await permissionService.ByPermissionAndRoleId(rolePermission);
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
            RoleExtension role = await roleservice.GetById(id);
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

            List<RoleExtension> roleExtensions = await roleservice.AllRoleByUserId(id);
            Response response = new Models.Response();
            response.code = MsgCode_Enum.SUCCESS;
            Guid domainId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            if (ClaimExtension.IsMosAdmin(User.Claims.ToArray()))
            {
                response.data = roleExtensions;
            }
            else
            {
                response.data = roleExtensions.Where(x => x.domainId == domainId);
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

            RoleExtension roleExtension = await roleservice.UserDomainRolesByDomainId(userId, domainId);

            return Ok(new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                data = roleExtension
            });
        }

        [RemsRequirementAttribute("REMOVE_ROLE")]
        [HttpPost]
        [Route("remove")]
        public async Task<object> RemoveRole([FromBody] UserRole userRole)
        {
            UserRole ur = await roleservice.GetUserRoleAsync(userRole.userid, userRole.roleid);
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

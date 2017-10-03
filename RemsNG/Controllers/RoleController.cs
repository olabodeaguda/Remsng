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
    [Route("api/v1/role")]
    public class RoleController : Controller
    {
        IRoleService roleservice;
        public RoleController(IRoleService _roleservice)
        {
            roleservice = _roleservice;
        }

        [RemsRequirementAttribute("GET_ROLES")]
        [HttpGet]
        public async Task<object> Get([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
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

                return roleservice.GetUserDomainRoleByUsername(username, domainId);
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
            List<RoleExtension> roleex = await roleservice.AllDomainRolesByDomainId(userRole.userid, role.domainId);

            if (roleex.Count > 0)
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

        //public async Task<object> RemoveRole()
        //{

        //    return;
        //}

        [Route("assignroletopermission")]
        [RemsRequirementAttribute("ASSIGN_ROLES")]
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

        [Route("{id}")]
        [RemsRequirementAttribute("GET_ROLES")]
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
    }
}

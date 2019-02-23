using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Threading.Tasks;

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/permission")]
    public class PermissionController : Controller
    {
        IPermissionManagers permissionService;
        IRoleManagers roleservice;
        public PermissionController(IPermissionManagers _permissionService, IRoleManagers _roleservice)
        {
            permissionService = _permissionService;
            roleservice = _roleservice;
        }

        [HttpGet]
        public async Task<object> Get()
        {
            return await permissionService.All();
        }

        [Route("byRoleid/{id}")]
        [HttpGet]
        public async Task<object> GetByRoleId([FromRoute]Guid id, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            var role = await roleservice.GetById(id);
            if (role == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Role not found"
                });
            }
            if (string.IsNullOrEmpty(pageSize))
            {
                pageSize = "20";
            }
            else if (string.IsNullOrEmpty(pageNum))
            {
                pageNum = "1";
            }

            int totalCount = await permissionService.PermissionCountByRoleId(id);
            var r = await permissionService.byRoleId(id, new PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            });

            return Ok(new
            {
                data = r,
                totalCount = totalCount
            });
        }

        [Route("permissionnotinrole/{id}")]
        [HttpGet]
        public async Task<object> GetPermissionNotInRole([FromRoute] Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.WRONG_CREDENTIALS,
                    description = "Bad request"
                });
            }

            RoleModel role = await roleservice.GetById(id);
            if (role == null)
            {
                return NotFound(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = "Role not found"
                });
            }

            return await permissionService.GetPermissionNotInRole(id);
        }


    }
}

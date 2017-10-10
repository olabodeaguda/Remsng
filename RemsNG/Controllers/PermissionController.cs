using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using RemsNG.ORM;
using RemsNG.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/permission")]
    public class PermissionController : Controller
    {
        IPermission permissionService;
        IRoleService roleservice;
        public PermissionController(IPermission _permissionService, IRoleService _roleservice)
        {
            permissionService = _permissionService;
            roleservice = _roleservice;
        }

        [RemsRequirementAttribute("GET_PERMISSIONS")]
        [HttpGet]
        public async Task<object> Get()
        {
            return await permissionService.All();
        }

        [Route("byRoleid/{id}")]
        [RemsRequirementAttribute("GET_PERMISSIONS")]
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
        [RemsRequirementAttribute("GET_PERMISSIONS")]
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

            RoleExtension role = await roleservice.GetById(id);
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

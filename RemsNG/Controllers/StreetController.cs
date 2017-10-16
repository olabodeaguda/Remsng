using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Services.Interfaces;
using RemsNG.ORM;
using RemsNG.Models;
using RemsNG.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/street")]
    public class StreetController : Controller
    {
        private readonly IStreetService streetService;

        public StreetController(IStreetService _streetService)
        {
            streetService = _streetService;
        }

        [Route("paginated/{wardId}")]
        [HttpGet]
        public async Task<object> GetStreetByWardIdPagenated(Guid wardId, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            if (wardId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Ward is required"
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

            return await streetService.ByWardpaginated(wardId, new PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            });
        }

        [Route("bywardid/{wardId}")]
        [HttpGet]
        public async Task<object> GetStreetByWardId(Guid wardId)
        {
            return await streetService.ByWard(wardId);
        }

        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            return await streetService.ById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]Street value)
        {
            if (value.wardId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Ward is required"
                });
            }
            else if (string.IsNullOrEmpty(value.streetName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street Name is required"
                });
            }

            value.createdBy = User.Identity.Name;
            Response response = await streetService.Add(value);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<object> Put(Guid id, [FromBody]Street value)
        {
            if (value.wardId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Ward is required"
                });
            }
            else if (string.IsNullOrEmpty(value.streetName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street Name is required"
                });
            }
            else if (value.id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur. Please refresh the page and try again"
                });
            }

            value.lastmodifiedby = User.Identity.Name;
            Response response = await streetService.Update(value);
            return Ok(response);
        }

        [HttpDelete("{id}/{status}")]
        public async Task<object> Post(Guid id, string status)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur. Please refresh the page and try again"
                });
            }
            else if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street Status is required"
                });
            }
            else if (!CommonList.StatusLst.Contains(status))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Wrong status"
                });
            }
            Response response = await streetService.ChangeStatus(id, status);
            return Ok(response);
        }
    }
}

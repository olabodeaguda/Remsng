using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/street")]
    public class StreetController : Controller
    {
        private readonly IStreetManagers streetService;

        public StreetController(IStreetManagers _streetService)
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

        [Route("bylcda/{lcdaId}")]
        [HttpGet]
        public async Task<object> GetStreetBylcdaId(Guid lcdaId)
        {
            return await streetService.ByLcda(lcdaId);
        }

        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            return await streetService.ById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]StreetModel value)
        {
            if (value.WardId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Ward is required"
                });
            }
            else if (string.IsNullOrEmpty(value.StreetName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street Name is required"
                });
            }

            value.CreatedBy = User.Identity.Name;
            Response response = await streetService.Add(value);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<object> Put(Guid id, [FromBody]StreetModel value)
        {
            if (value.WardId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Ward is required"
                });
            }
            else if (string.IsNullOrEmpty(value.StreetName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street Name is required"
                });
            }
            else if (value.Id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "An error occur. Please refresh the page and try again"
                });
            }

            value.Lastmodifiedby = User.Identity.Name;
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

        [Route("search/{wardid}")]
        [HttpGet]
        public async Task<IActionResult> GetStreet(Guid wardid, [FromQuery] string query)
        {
            if (wardid == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "bad request"
                });
            }
            if (string.IsNullOrEmpty(query))
            {
                return Ok(await streetService.ByWard(wardid));
            }

            return Ok(await streetService.Search(wardid, query));
        }
    }
}

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
    [Route("api/v1/sector")]
    public class SectorController : Controller
    {
        private readonly ISectorManagers sectorService;
        public SectorController(ISectorManagers _sectorService)
        {
            sectorService = _sectorService;
        }

        [HttpGet("bylcda/{id}")]
        public async Task<object> GetByLcdaid(Guid id)
        {
            if (id == default(Guid))
            {
                return await sectorService.ByLcdaId(id);
            }
            return await sectorService.ByLcdaId(id);
        }

        [HttpGet("{id}")]
        public async Task<object> GetbyId(Guid id)
        {
            if (id == default(Guid))
            {
                return await sectorService.ByLcdaId(id);
            }

            return await sectorService.ById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]SectorModel sector)
        {
            if (sector.LcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector is required"
                });
            }
            else if (string.IsNullOrEmpty(sector.SectorName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector Name is required"
                });
            }

            sector.CreatedBy = User.Identity.Name;

            Response response = await sectorService.Add(sector);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<object> Put(Guid id, [FromBody]SectorModel sector)
        {
            if (string.IsNullOrEmpty(sector.SectorName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector Name is required"
                });
            }
            else if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request.Please refresh and retry"
                });
            }

            sector.Lastmodifiedby = User.Identity.Name;

            Response response = await sectorService.Update(sector);
            return Ok(response);
        }
    }
}

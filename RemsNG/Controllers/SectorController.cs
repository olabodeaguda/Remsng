using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services;
using RemsNG.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using RemsNG.ORM;
using RemsNG.Models;
using RemsNG.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/sector")]
    public class SectorController : Controller
    {
        private readonly ISectorService sectorService;
        public SectorController(ISectorService _sectorService)
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
        public async Task<object> Post([FromBody]Sector sector)
        {
            if (sector.lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector is required"
                });
            }
            else if (string.IsNullOrEmpty(sector.sectorName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector Name is required"
                });
            }

            sector.createdBy = User.Identity.Name;

            Response response = await sectorService.Add(sector);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<object> Put(Guid id, [FromBody]Sector sector)
        {
            if (string.IsNullOrEmpty(sector.sectorName))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Sector Name is required"
                });
            }
            else if(id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request.Please refresh and retry"
                });
            }

            sector.lastmodifiedby = User.Identity.Name;

            Response response = await sectorService.Add(sector);
            return Ok(response);
        }
    }
}

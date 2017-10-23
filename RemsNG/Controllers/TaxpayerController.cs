using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Route("api/v1/taxpayer")]
    public class TaxpayerController : Controller
    {
        private ITaxpayerService taxpayerService;
        public TaxpayerController(ITaxpayerService _taxpayerService)
        {
            taxpayerService = _taxpayerService;
        }
        [Route("bylcda/{id}")]
        [HttpGet]
        public async Task<object> ByLcda(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByLcdaId(id);
        }

        [Route("bylcdapaginated/{id}")]
        [HttpGet]
        public async Task<object> ByLcdaPaginated(Guid id, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByLcdaId(id, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }


        [Route("bycompany/{id}")]
        [HttpGet]
        public async Task<object> ByCompany(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByCompanyId(id);
        }

        [Route("bystreet/{id}")]
        [HttpGet]
        public async Task<object> ByStreet(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByStreetId(id);
        }

        [Route("bystreetpaginated/{id}")]
        [HttpGet]
        public async Task<object> ByStreetPaginated(Guid id, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "20" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ByStreetId(id, new PageModel() { PageNum = int.Parse(pageNum), PageSize = int.Parse(pageSize) });
        }

        [HttpGet("{id}")]
        public async Task<object> Get(Guid id)
        {
            if (id == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            return await taxpayerService.ById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]Taxpayer value, [FromBody] Address address)
        {
            if (address.streetId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Street is required"
                });
            }
            else if (value.companyId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Company is required"
                });
            }
            else if (string.IsNullOrEmpty(address.addressnumber))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "House number is required"
                });
            }


            return await Task.Run(() =>
            {
                return new object();

            });
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

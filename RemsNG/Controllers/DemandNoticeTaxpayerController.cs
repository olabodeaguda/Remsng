﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.Exceptions;
using RemsNG.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/dnt")]
    public class DemandNoticeTaxpayerController : Controller
    {
        private IDnTaxpayer dnTaxpayer;
        public DemandNoticeTaxpayerController(IDnTaxpayer _dnTaxpayer)
        {
            dnTaxpayer = _dnTaxpayer;
        }

        [HttpGet("billingno/{billingno}")]
        public async Task<object> GetByBillingNumber([FromRoute]string billingno)
        {
            if (string.IsNullOrEmpty(billingno))
            {
                return BadRequest(new Response()
                {
                    code = Utilities.MsgCode_Enum.INVALID_PARAMETER_PASSED,
                    description = "Billing number is required"
                });
            }
            var tu = await dnTaxpayer.ByBillingNo(billingno);

            return Ok(new Response()
            {
                code = Utilities.MsgCode_Enum.SUCCESS,
                data = new object[] { tu }
            });
        }
        
        [HttpGet("batchno/{batchno}")]
        public async Task<object> GetByBatchNo([FromRoute]string batchno, [FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (string.IsNullOrEmpty(batchno))
            {
                throw new InvalidCredentialsException("Batch number is required");
            }
            return await dnTaxpayer.GetDNTaxpayerByBatchIdAsync(batchno, new Models.PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            });
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
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

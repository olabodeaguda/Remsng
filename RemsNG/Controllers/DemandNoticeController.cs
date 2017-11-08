﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemsNG.Services.Interfaces;
using RemsNG.Models;
using RemsNG.Utilities;
using RemsNG.ORM;
using RemsNG.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using RemsNG.Security;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RemsNG.Controllers
{
    [Authorize]
    [Route("api/v1/demandnotice")]
    public class DemandNoticeController : Controller
    {
        private IDemandNoticeService demandService;
        private IStreetService streetService;
        private IWardService wardService;
        public DemandNoticeController(IDemandNoticeService _demandService, IStreetService _streetService,
            IWardService _wardService)
        {
            demandService = _demandService;
            streetService = _streetService;
            wardService = _wardService;
        }

        [HttpGet("bylcda")]
        public async Task<object> ByLcda([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            if (lcdaId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            return await demandService.ByLcdaId(lcdaId, new PageModel()
            {
                PageNum = int.Parse(pageNum),
                PageSize = int.Parse(pageSize)
            });
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

            return await demandService.GetById(id);
        }

        [HttpPost]
        public async Task<object> Post([FromBody]DemandNoticeRequest demandNoticeRequest)
        {
           // DemandNoticeRequest demandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequest>(value);
            if (demandNoticeRequest.streetId != null && demandNoticeRequest.streetId != default(Guid))
            {
                Street street = await streetService.ById(demandNoticeRequest.streetId.Value);
                if (street == null)
                {
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.NOTFOUND,
                        description = "Street not found"
                    });
                }

            }
            else if (demandNoticeRequest.wardId != null && demandNoticeRequest.wardId != default(Guid))
            {
                Guid s = demandNoticeRequest.wardId.Value;
                Ward ward = await wardService.GetWard(demandNoticeRequest.wardId.Value);
                if (ward == null)
                {
                    return BadRequest(new Response()
                    {
                        code = MsgCode_Enum.NOTFOUND,
                        description = "Ward not found"
                    });
                }
            }
            else if (demandNoticeRequest.dateYear == 0)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Billing year is required"
                });
            }

            DemandNotice demandNotice = new DemandNotice();
            demandNotice.lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            string encr = JsonConvert.SerializeObject(demandNoticeRequest);
            string dx = EncryptDecryptUtils.ToHexString(encr); ;
            string ddx = EncryptDecryptUtils.FromHexString(dx);
            demandNotice.query = EncryptDecryptUtils.ToHexString(encr);
            demandNotice.billingYear = demandNoticeRequest.dateYear;
            demandNotice.createdBy = User.Identity.Name;
            demandNotice.id = Guid.NewGuid();
            demandNotice.batchNo = CommonList.GetBatchNo();
            demandNotice.demandNoticeStatus = DemandNoticeStatus.SUBMITTED.ToString();

            Response response = await demandService.Add(demandNotice);

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Created("/demandnotice", response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updatequery/{id}")]
        public async Task<object> UpdateQuery(Guid id, [FromBody]DemandNoticeRequest query)
        {
            DemandNotice demandNotice = await demandService.GetById(id);
            if (demandNotice.demandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be Edited"
                });
            }

            query.lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());

            Response response = await demandService.UpdateQuery(new DemandNotice()
            {
                id = id,
                query = JsonConvert.SerializeObject(query),
                lastmodifiedby = User.Identity.Name,
                lastModifiedDate = DateTime.Now
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updateyear/{id}")]
        public async Task<object> UpdateBillingYear(Guid id, [FromHeader]int billingYr)
        {
            DemandNotice demandNotice = await demandService.GetById(id);
            if (demandNotice.demandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be edited"
                });
            }

            Response response = await demandService.UpdateBillingYr(new DemandNotice()
            {
                id = id,
                billingYear = billingYr,
                lastmodifiedby = User.Identity.Name,
                lastModifiedDate = DateTime.Now
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPut("updatestatus/{id}")]
        public async Task<object> UpdateStatus(Guid id, [FromHeader]string dns)
        {
            DemandNotice demandNotice = await demandService.GetById(id);
            if (demandNotice.demandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be Edited"
                });
            }

            Response response = await demandService.UpdateBillingYr(new DemandNotice()
            {
                id = id,
                demandNoticeStatus = dns,
                lastmodifiedby = User.Identity.Name,
                lastModifiedDate = DateTime.Now
            });

            if (response.code == MsgCode_Enum.SUCCESS)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }


    }
}

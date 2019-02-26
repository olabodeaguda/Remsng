using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Infrastructure.Extensions;
using RemsNG.Security;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace RemsNG.Controllers
{
    //[Authorize]
    [Route("api/v1/demandnotice")]
    public class DemandNoticeController : Controller
    {
        private IDemandNoticeManagers demandService;
        private IStreetManagers streetService;
        private IWardManagers wardService;
        private IDemandNoticeTaxpayerManagers dnTaxpayerService;
        public DemandNoticeController(IDemandNoticeManagers _demandService, IStreetManagers _streetService,
            IWardManagers _wardService, IDemandNoticeTaxpayerManagers _dnTaxpayerService)
        {
            demandService = _demandService;
            streetService = _streetService;
            wardService = _wardService;
            dnTaxpayerService = _dnTaxpayerService;
        }

        [HttpGet("bylcda")]
        public async Task<object> ByLcda([FromHeader] string pageSize, [FromHeader] string pageNum)
        {
            Guid lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            bool ismosdmin = ClaimExtension.IsMosAdmin(User.Claims.ToArray());
            if (lcdaId == default(Guid) && !ismosdmin)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }

            pageSize = string.IsNullOrEmpty(pageSize) ? "1" : pageSize;
            pageNum = string.IsNullOrEmpty(pageNum) ? "1" : pageNum;

            if (!ismosdmin)
            {
                return await demandService.ByLcdaId(lcdaId, new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                });
            }
            else
            {
                return await demandService.All(new PageModel()
                {
                    PageNum = int.Parse(pageNum),
                    PageSize = int.Parse(pageSize)
                });
            }
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

            var result = await demandService.GetById(id);
            result.Query = Utilities.EncryptDecryptUtils.FromHexString(result.Query);
            Response response = new Response();
            response.data = result;
            if (result != null)
            {
                response.code = MsgCode_Enum.SUCCESS;
            }
            else
            {
                response.code = MsgCode_Enum.NOTFOUND;
                response.description = $"Record not found";
            }

            return response;
        }

        [HttpGet("batchno/{id}")]
        public async Task<object> GetByBatchNumber(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request"
                });
            }
            var result = await demandService.GetByBatchId(id);
            result.Query = Utilities.EncryptDecryptUtils.FromHexString(result.Query);
            Response response = new Response();
            response.data = result;
            if (result != null)
            {
                response.code = MsgCode_Enum.SUCCESS;
            }
            else
            {
                response.code = MsgCode_Enum.NOTFOUND;
                response.description = $"{id} not found";
            }

            return response;
        }

        [RemsRequirementAttribute("DEMANDNOTICE_REQUEST")]
        [HttpPost]
        public async Task<object> Post([FromBody]DemandNoticeRequestModel demandNoticeRequest)
        {
            if (demandNoticeRequest.streetId != null && demandNoticeRequest.streetId != default(Guid))
            {
                StreetModel street = await streetService.ById(demandNoticeRequest.streetId);
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
                Guid s = demandNoticeRequest.wardId;
                WardModel ward = await wardService.GetWard(demandNoticeRequest.wardId);
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


            DemandNoticeModel demandNotice = new DemandNoticeModel();
            demandNotice.LcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());
            string encr = JsonConvert.SerializeObject(demandNoticeRequest);
            string dx = Utilities.EncryptDecryptUtils.ToHexString(encr); ;
            string ddx = Utilities.EncryptDecryptUtils.FromHexString(dx);
            demandNotice.Query = Utilities.EncryptDecryptUtils.ToHexString(encr);

            demandNoticeRequest.createdBy = User.Identity.Name;
            demandNotice.BillingYear = demandNoticeRequest.dateYear;
            demandNotice.CreatedBy = User.Identity.Name;
            demandNotice.Id = Guid.NewGuid();
            demandNotice.BatchNo = CommonList.GetBatchNo();
            demandNotice.DemandNoticeStatus = DemandNoticeStatus.SUBMITTED.ToString();
            demandNotice.WardId = demandNoticeRequest.wardId;
            demandNotice.StreetId = demandNoticeRequest.streetId;
            demandNotice.IsUnbilled = demandNoticeRequest.isUnbilled;

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

        [RemsRequirementAttribute("DEMANDNOTICE_REQUEST")]
        [HttpPost("run/bytaxpayer")]
        public async Task<IActionResult> PostDemandNoticeRequest([FromBody] DemandNoticeRequestModel demandNoticeRequest)
        {
            return Created("/demandNotice", null);
        }


        [HttpPost("search/{pageNum}/{pageSize}")]
        public async Task<IActionResult> SearchDemandNotice([FromBody] SearchDNModel model, int pageNum = 1, int pageSize = 20)
        {
            var modl = new DemandNoticeRequestModel();
            modl.dateYear = model.DateYear;
            modl.lcdaId = string.IsNullOrEmpty(model.LcdaId) ? default(Guid) : Guid.Parse(model.LcdaId);
            modl.searchByName = model.SearchByName;
            modl.streetId = string.IsNullOrEmpty(model.StreetId) ? default(Guid) : Guid.Parse(model.StreetId);
            modl.wardId = string.IsNullOrEmpty(model.WardId) ? default(Guid) : Guid.Parse(model.WardId);

            return Ok(await demandService.SearchDemandNotice(modl, new PageModel()
            {
                PageNum = pageNum,
                PageSize = pageSize
            }));
        }

        [HttpPost("searchinfo")]
        public async Task<IActionResult> GetSearchInfo([FromBody] SearchDNModel model)
        {
            var modl = new DemandNoticeRequestModel();
            modl.dateYear = model.DateYear;
            modl.lcdaId = string.IsNullOrEmpty(model.LcdaId) ? default(Guid) : Guid.Parse(model.LcdaId);
            modl.searchByName = model.SearchByName;
            modl.streetId = string.IsNullOrEmpty(model.StreetId) ? default(Guid) : Guid.Parse(model.StreetId);
            modl.wardId = string.IsNullOrEmpty(model.WardId) ? default(Guid) : Guid.Parse(model.WardId);

            var result = await demandService.SearchInfo(modl);

            return Ok(result);
        }

        [HttpPut("updatequery/{id}")]
        public async Task<object> UpdateQuery(Guid id, [FromBody]DemandNoticeRequestModel query)
        {
            DemandNoticeModel demandNotice = await demandService.GetById(id);
            if (demandNotice.DemandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be Edited"
                });
            }

            query.lcdaId = ClaimExtension.GetDomainId(User.Claims.ToArray());

            Response response = await demandService.UpdateQuery(new DemandNoticeModel()
            {
                Id = id,
                Query = JsonConvert.SerializeObject(query),
                Lastmodifiedby = User.Identity.Name,
                LastModifiedDate = DateTime.Now
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
            DemandNoticeModel demandNotice = await demandService.GetById(id);
            if (demandNotice.DemandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be edited"
                });
            }

            Response response = await demandService.UpdateBillingYr(new DemandNoticeModel()
            {
                Id = id,
                BillingYear = billingYr,
                Lastmodifiedby = User.Identity.Name,
                LastModifiedDate = DateTime.Now
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
            DemandNoticeModel demandNotice = await demandService.GetById(id);
            if (demandNotice.DemandNoticeStatus != DemandNoticeStatus.SUBMITTED.ToString())
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Demand notice with status 'submitted' can't be Edited"
                });
            }

            Response response = await demandService.UpdateBillingYr(new DemandNoticeModel()
            {
                Id = id,
                DemandNoticeStatus = dns,
                Lastmodifiedby = User.Identity.Name,
                LastModifiedDate = DateTime.Now
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

        [HttpPost("addarrears")]
        public async Task<object> AddArrears([FromBody]DemandNoticeArrearsModel dna)
        {
            if (dna.ItemId == default(Guid))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Item is required"
                });
            }
            else if (dna.TotalAmount < 1)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Item is required"
                });
            }
            else if (dna.TaxpayerId == Guid.Empty || string.IsNullOrEmpty(dna.BillingNo))
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = "Bad request. Please refresh the page and try again"
                });
            }

            DemandNoticeTaxpayersModel dnTxPayer = await dnTaxpayerService.ByBillingNo(dna.BillingNo);

            if (dnTxPayer == null)
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.NOTFOUND,
                    description = $"{dna.BillingNo} not found"
                });
            }

            dna.ArrearsStatus = DemandNoticeStatus.PENDING.ToString();
            dna.BillingYear = dnTxPayer.BillingYr;
            dna.CreatedBy = User.Identity.Name;

            bool result = await demandService.AddArrears(dna);

            if (result)
            {
                return Created("/", new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Arrears has been created successfully"
                });
            }
            else
            {
                return BadRequest(new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = "Arrears has been created successfully"
                });
            }
        }


        [HttpPost("validtaxpayers")]
        public async Task<IActionResult> ValidTaxpayer([FromBody] SearchDNModel model)
        {
            var modl = new DemandNoticeRequestModel();
            modl.dateYear = model.DateYear;
            modl.lcdaId = string.IsNullOrEmpty(model.LcdaId) ? default(Guid) : Guid.Parse(model.LcdaId);
            modl.searchByName = model.SearchByName;
            modl.streetId = string.IsNullOrEmpty(model.StreetId) ? default(Guid) : Guid.Parse(model.StreetId);
            modl.wardId = string.IsNullOrEmpty(model.WardId) ? default(Guid) : Guid.Parse(model.WardId);

            TaxPayerModel[] taxpayers = await demandService.ValidTaxpayers(modl);
            return Ok(new Response
            {
                code = MsgCode_Enum.SUCCESS,
                data = taxpayers
            });
        }
    }
}

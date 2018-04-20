using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using RemsNG.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class RunDemandNoticeService : IRunDemandNoticeService
    {
        private readonly TaxpayerDao taxpayerDao;
        private readonly DemandNoticeDao demandNoticeDao;
        private readonly DemandNoticeTaxpayersDao demandNoticeTaxpayersDao;
        private readonly ErrorDao errorDao;
        private readonly ILogger logger;
        private readonly DemandNoticeItemDao demandNoticeItemDao;
        private readonly DemandNoticeArrearDao demandNoticeArrearDao;
        private readonly DemandNoticePenaltyDao demandNoticePenaltyDao;
        private readonly LcdaDao lcdaDao;
        private readonly WardDao wardDao;
        private readonly StreetDao streetDao;
        private readonly IAddress address;
        private readonly ILcdaService lcdaService;
        private readonly IStateService stateService;
        private readonly IImageService imageService;
        private readonly IDnDownloadService dnDownloadService;
        private readonly IBatchDwnRequestService batchDwnRequestService;
        private readonly IDemandNoticeTaxpayerService demandNoticeTaxpayerService;
        private ITaxpayerService taxpayerService;
        private IListPropertyService listPropertyService;
        private IHttpContextAccessor httpContextAccessor;
        private INodeServices nodeServices;
        private IHostingEnvironment hostingEnvironment;
        private IServiceProvider serviceProvider;

        public RunDemandNoticeService(RemsDbContext _db,
            ILoggerFactory loggerFactory, IAddress _address,
            ILcdaService _lcdaService, IStateService _stateService,
            IImageService _imageService,
            IDnDownloadService _dnDownloadService, IBatchDwnRequestService _batchDwnRequestService,
            IDemandNoticeTaxpayerService _demandNoticeTaxpayerService
            , ITaxpayerService _taxpayerService,
            IListPropertyService _listPropertyService,
            IHttpContextAccessor _httpContextAccessor,
            INodeServices _nodeServices, IHostingEnvironment _hostingEnvironment,
            IServiceProvider _serviceProvider)
        {
            logger = loggerFactory.CreateLogger("Demand Notice Jobs");
            nodeServices = _nodeServices;
            hostingEnvironment = _hostingEnvironment;
            serviceProvider = _serviceProvider;
            taxpayerDao = new TaxpayerDao(_db);
            demandNoticeDao = new DemandNoticeDao(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersDao(_db);
            errorDao = new ErrorDao(_db, loggerFactory);
            demandNoticeItemDao = new DemandNoticeItemDao(_db);
            demandNoticeArrearDao = new DemandNoticeArrearDao(_db);
            demandNoticePenaltyDao = new DemandNoticePenaltyDao(_db);
            lcdaDao = new LcdaDao(_db, loggerFactory);
            wardDao = new WardDao(_db);
            streetDao = new StreetDao(_db, loggerFactory);
            address = _address;
            lcdaService = _lcdaService;
            imageService = _imageService;
            stateService = _stateService;
            dnDownloadService = _dnDownloadService;
            batchDwnRequestService = _batchDwnRequestService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
            listPropertyService = _listPropertyService;
            httpContextAccessor = _httpContextAccessor;
            taxpayerService = _taxpayerService;
        }

        public async Task RegisterTaxpayer()
        {
            DemandNotice demandNotice = await demandNoticeDao.DequeueDemandNotice();

            try
            {
                if (demandNotice != null)
                {
                    string query = EncryptDecryptUtils.FromHexString(demandNotice.query);
                    DemandNoticeRequest demandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequest>(query);
                    demandNoticeRequest.createdBy = demandNotice.createdBy;
                    List<Taxpayer> taxpayers = await taxpayerDao.Get(demandNoticeRequest);
                    string domainName = string.Empty;

                    if (taxpayers.Count > 0)
                    {
                        Lgda lcda = await lcdaService.Get(demandNoticeRequest);
                        if (lcda != null)
                        {
                            Domain dd = await lcdaDao.GetDomain(lcda.id);
                            if (dd != null)
                            {
                                domainName = dd.domainName;
                            }

                        }

                        List<DemandNoticeTaxpayersDetail> dt =
                            await demandNoticeTaxpayersDao.getTaxpayerByIds(taxpayers.Select(x => string.Format("'{0}'", x.id)).ToArray(), demandNotice.billingYear);
                        //make sure demand notice have not been raised for the taxpayers by year and taxpayersId

                        foreach (var tm in taxpayers)
                        {
                            var itExist = dt.FirstOrDefault(x => x.taxpayerId == tm.id);
                            if (itExist != null && !demandNoticeRequest.CloseOldData)
                            {
                                Error error = new Error()
                                {
                                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                    errorvalue = $"Demand notice have already been raised for {tm.surname} {tm.firstname} {tm.lastname} " +
                                      $"for billing year {demandNotice.billingYear}",
                                    ownerId = demandNotice.id
                                };
                                bool result = await errorDao.Add(error);
                                continue;
                            }
                            else
                            {
                                if (demandNoticeRequest.CloseOldData && itExist != null)
                                {
                                    //closed demand notice taxpaet
                                    await ClosedDDTaxpayer(itExist);
                                    Error error = new Error()
                                    {
                                        errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                        errorvalue = $"{itExist.billingNumber},{itExist.billingYr} has been closed",
                                        ownerId = itExist.taxpayerId
                                    };
                                    bool result = await errorDao.Add(error);
                                }

                                DemandNoticeTaxpayersDetail dntd = new DemandNoticeTaxpayersDetail();
                                dntd.billingYr = demandNotice.billingYear;
                                dntd.dnId = demandNotice.id;
                                dntd.createdBy = demandNoticeRequest.createdBy;
                                dntd.taxpayerId = tm.id;
                                dntd.domainName = domainName;
                                dntd.lcdaAddress = await address.AddressByOwnerId(lcda.id);
                                dntd.lcdaState = await stateService.StateNameByLcda(lcda.id);
                                dntd.lcdaLogoFileName = await imageService.ImageNameByOwnerIdAsync(lcda.id,
                                    ImgTypesEnum.LOGO.ToString());
                                dntd.revCoodinatorSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.id,
                                    ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString());
                                dntd.councilTreasurerSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.id,
                                    ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString());

                                Response response1 = await demandNoticeTaxpayersDao.Add(dntd);

                                if (response1.code == MsgCode_Enum.SUCCESS)
                                {
                                    dntd.billingNumber = response1.data.ToString().Trim();
                                    //run arrears 
                                    await RunArrears(dntd);
                                    await RunDemandNoticeItem(dntd); //run items 
                                }
                                else
                                {
                                    // log error
                                    Error error = new Error()
                                    {
                                        errorType = ErrorType.DEMAND_NOTICE.ToString(),
                                        errorvalue = response1.description,
                                        ownerId = dntd.dnId
                                    };
                                    bool result = await errorDao.Add(error);
                                }
                            }
                        }
                    }
                    else
                    {
                        Error error = new Error()
                        {
                            errorType = ErrorType.DEMAND_NOTICE.ToString(),
                            errorvalue = $"{taxpayers.Count} taxpayer was found from the query request ",
                            ownerId = demandNotice.id
                        };
                        bool result = await errorDao.Add(error);
                    }


                    //update demand notice
                    demandNotice.demandNoticeStatus = DemandNoticeStatus.COMPLETED.ToString();
                    Response response = await demandNoticeDao.UpdateStatus(demandNotice);
                    if (response.code == MsgCode_Enum.SUCCESS)
                    {
                        logger.LogInformation($"{demandNotice.id} has been completed");
                    }
                }
            }
            catch (Exception x)
            {
                // cancel no retry
                demandNotice.demandNoticeStatus = DemandNoticeStatus.ERROR.ToString();
                Response response = await demandNoticeDao.UpdateStatus(demandNotice);
                if (response.code == MsgCode_Enum.SUCCESS)
                {
                    logger.LogInformation($"{demandNotice.id} has been completed");
                }
                logger.LogError(x.Message);
            }
        }

        private async Task<bool> ClosedDDTaxpayer(DemandNoticeTaxpayersDetail itExist)
        {
            return await demandNoticeTaxpayersDao.UpdateTaxPayer(itExist.id, DemandNoticeStatus.CLOSED.ToString());
        }

        public async Task GenerateBulkDemandNotice()
        {
            BatchDemandNoticeModel bdnm = null;// await batchDwnRequestService.Dequeue();
            try
            {
                bdnm = await batchDwnRequestService.Dequeue();
                if (bdnm != null)
                {
                    List<DemandNoticeTaxpayersDetail> lstOfDN = await demandNoticeTaxpayerService.GetDNTaxpayerByBatchNoAsync(bdnm.batchNo);
                    if (lstOfDN.Count > 0)
                    {
                        var firstTaxpayer = lstOfDN[0];
                        Lgda lgda = await taxpayerService.getLcda(firstTaxpayer.taxpayerId);
                        string template = await dnDownloadService.LcdaTemlateByLcda(lgda.id);

                        string rootUrl = this.hostingEnvironment.WebRootPath;

                        string rootPath = Path.Combine(this.hostingEnvironment.WebRootPath, "zipReports", bdnm.batchNo);
                        if (!Directory.Exists(rootPath))
                        {
                            Directory.CreateDirectory(rootPath);
                        }

                        List<string> lstContent = new List<string>();
                        var htmlContent = await File.ReadAllTextAsync($"{rootUrl}/templates/{template}");
                        string htmlContents = string.Empty;
                        for (int i = 0; i < lstOfDN.Count; i++)
                        {
                            string s = await dnDownloadService.PopulateReportHtml(htmlContent, lstOfDN[i].billingNumber, rootUrl, bdnm.createdBy);
                            if (s == string.Empty)
                            {
                                // log bill number
                                continue;
                            }
                            htmlContents = htmlContents + s;
                            if (i < 1)
                            {
                                continue;
                            }
                            if (i % 90 == 0 || i == (lstOfDN.Count - 1))
                            {
                                lstContent.Add(htmlContents);
                                htmlContents = string.Empty;
                            }
                        }

                        using (FileStream zipToOpen = new FileStream($"{rootPath}/{bdnm.batchNo}.zip", FileMode.Create))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                int count = 0;
                                foreach (var dnt in lstContent)
                                {
                                    var result = await nodeServices.InvokeAsync<byte[]>("./pdf", dnt);

                                    string filePath = Path.Combine(rootPath, $"Batch {count + 1}.pdf");
                                    using (FileStream fs = System.IO.File.Create(filePath))
                                    {
                                        await fs.WriteAsync(result, 0, result.Length);
                                        fs.Flush();
                                    }
                                    archive.CreateEntryFromFile(filePath, $"batch {count + 1}.pdf");
                                    count++;
                                }
                            }
                        }
                    }

                    //update request
                    Response response = await batchDwnRequestService.UpdateBatchRequest(new BatchDemandNoticeModel
                    {
                        id = bdnm.id,
                        batchFileName = $"{bdnm.batchNo}.zip",
                        requestStatus = "COMPLETED",
                        createdBy = "APPLICATION"

                    });
                }
            }
            catch (Exception x)
            {
                if (bdnm != null)
                {
                    Response response = await batchDwnRequestService.UpdateBatchRequest(new BatchDemandNoticeModel
                    {
                        id = bdnm.id,
                        batchFileName = $"{bdnm.batchNo}.zip",
                        requestStatus = "ERROR",
                        createdBy = "APPLICATION"
                    });
                }
                logger.LogError(x.Message);
            }
        }

        public async Task GenerateBulkDemandNotice2()
        {
            BatchDemandNoticeModel bdnm = null;// await batchDwnRequestService.Dequeue();
            try
            {
                bdnm = await batchDwnRequestService.Dequeue();
                if (bdnm != null)
                {
                    List<DemandNoticeTaxpayersDetail> lstOfDN = await demandNoticeTaxpayerService.GetDNTaxpayerByBatchNoAsync(bdnm.batchNo);
                    if (lstOfDN.Count > 0)
                    {
                        var firstTaxpayer = lstOfDN[0];
                        Lgda lgda = await taxpayerService.getLcda(firstTaxpayer.taxpayerId);
                        string template = await dnDownloadService.LcdaTemlateByLcda(lgda.id);
                        string rootUrl = this.hostingEnvironment.WebRootPath;
                        string rootPath = Path.Combine(this.hostingEnvironment.WebRootPath, "zipReports", bdnm.batchNo);
                        if (!Directory.Exists(rootPath))
                        {
                            Directory.CreateDirectory(rootPath);
                        }
                        using (FileStream zipToOpen = new FileStream($"{rootPath}/{bdnm.batchNo}.zip", FileMode.Create))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                foreach (var dnt in lstOfDN)
                                {
                                    var htmlContent = await File.ReadAllTextAsync($"{rootUrl}/templates/{template}");
                                    htmlContent = await dnDownloadService.PopulateReportHtml(htmlContent, dnt.billingNumber, rootUrl, bdnm.createdBy);

                                    htmlContent = htmlContent.Replace("PATCH1", "");
                                    htmlContent = htmlContent.Replace("PATCH2", "");

                                    var result = await nodeServices.InvokeAsync<byte[]>("./pdf",
                                        htmlContent);

                                    string filePath = Path.Combine(rootPath, $"{dnt.billingNumber}.pdf");
                                    using (FileStream fs = System.IO.File.Create(filePath))
                                    {
                                        await fs.WriteAsync(result, 0, result.Length);
                                        fs.Flush();
                                    }
                                    archive.CreateEntryFromFile(filePath, $"{dnt.billingNumber}.pdf");
                                    //ZipArchiveEntry readmeEntry = archive.CreateEntry(filePath);
                                }
                            }
                        }
                    }
                }

                Response response = await batchDwnRequestService.UpdateBatchRequest(new BatchDemandNoticeModel
                {
                    id = bdnm.id,
                    batchFileName = $"{bdnm.batchNo}.zip",
                    requestStatus = "COMPLETED",
                    createdBy = "APPLICATION"

                });
            }
            catch (Exception x)
            {
                if (bdnm != null)
                {
                    Response response = await batchDwnRequestService.UpdateBatchRequest(new BatchDemandNoticeModel
                    {
                        id = bdnm.id,
                        batchFileName = $"{bdnm.batchNo}.zip",
                        requestStatus = "ERROR",
                        createdBy = "APPLICATION"
                    });
                }
                logger.LogError(x.Message);
            }
        }

        public async Task TaxpayerPenalty()
        {
            // scan through demandnoticeitem() check the year/penalty duration compare, if passed then create penalty

            List<DemandNoticeItem> demandNoticeItem = await demandNoticePenaltyDao.OverDueDemandNotice();

            string query = string.Empty;
            string ids = string.Empty;
            foreach (var tm in demandNoticeItem)
            {
                DateTime dt = tm.dateCreated.Value;
                DateTime startDuration = new DateTime(dt.Year, dt.Month, dt.Day);
                List<string> lstOfdurations = CommonList.CurrentDurations(startDuration);

                if (!lstOfdurations.Contains(tm.duration))
                {
                    continue;
                }

                ids = ids + $"{tm.id}|";
                // add to demand notice penalty
                DemandNoticeItemPenalty dnp = new DemandNoticeItemPenalty()
                {
                    billingNo = tm.billingNo,
                    amountPaid = 0,
                    billingYr = tm.billingYr,
                    itemId = tm.itemId,
                    itemPenaltyStatus = DemandNoticeStatus.PENDING.ToString(),
                    originatedYear = tm.billingYr,
                    taxpayerId = tm.taxpayerId,
                    totalAmount = tm.penaltyAmount.Value
                };
                query = query + demandNoticePenaltyDao.AddQuery(dnp);
            }

            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    Response response = demandNoticePenaltyDao.RunQuery(query);
                    if (response.code == MsgCode_Enum.SUCCESS)
                    {
                        logger.LogInformation($"Ids: {ids}, description: {response.description}");
                    }
                    else
                    {
                        logger.LogError($"Ids: {ids}, description: {response.description}");
                    }
                }
                catch (Exception x)
                {
                    logger.LogError(x.Message);
                }
            }

        }

        private async Task RunArrears(DemandNoticeTaxpayersDetail dntd)
        {
            // check for unpaid arrears  paid or partly paid
            // change current arrear status to move
            // create a new arrears with the status unpaid
            //get previous year demand notice

            DN_ArrearsModel dN_ArrearsModel = new DN_ArrearsModel()
            {
                arrearstatus = DemandNoticeStatus.PENDING.ToString(),
                billingNo = dntd.billingNumber,
                billingYr = dntd.billingYr,
                previousBillingYr = dntd.billingYr,
                createdBy = dntd.createdBy,
                taxpayerId = dntd.taxpayerId
            };

            Response responseUnpaidArrears = await demandNoticeArrearDao.AddUnpaidArrearsAsync(dN_ArrearsModel);
            if (responseUnpaidArrears.code != MsgCode_Enum.SUCCESS)
            {
                // logger error
                logger.LogError(responseUnpaidArrears.description);
                Error error = new Error()
                {
                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                    errorvalue = $"Unpaid arrears section : {responseUnpaidArrears.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    ownerId = dntd.dnId
                };
                bool result = await errorDao.Add(error);
            }

            //get position of the year

            Response responseUnpaidDemandNotice = await demandNoticeArrearDao.AddUnpaidDemandNoticeToArrearsAsync(dN_ArrearsModel);
            if (responseUnpaidDemandNotice.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(responseUnpaidDemandNotice.description);
                Error error = new Error()
                {
                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                    errorvalue = $"Unpaid demand notice arrears section: {responseUnpaidDemandNotice.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    ownerId = dntd.dnId
                };
                bool result = await errorDao.Add(error);
            }

            await MovedUnpaidPenalty(dN_ArrearsModel);
        }

        private async Task MovedUnpaidPenalty(DN_ArrearsModel dN_ArrearsModel)
        {
            Response responseUnpaidPenalty = await demandNoticePenaltyDao.AddUnpaidPenaltyAsync(dN_ArrearsModel);
            if (responseUnpaidPenalty.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(responseUnpaidPenalty.description);
                Error error = new Error()
                {
                    errorType = ErrorType.DEMAND_NOTICE.ToString(),
                    errorvalue = $"Unpaid demand notice Penalty movement section: {responseUnpaidPenalty.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    ownerId = dN_ArrearsModel.dnId
                };
                bool result = await errorDao.Add(error);
            }
        }

        private async Task RunDemandNoticeItem(DemandNoticeTaxpayersDetail dntd)
        {
            Response response = await demandNoticeItemDao.Add(dntd);
            if (response.code != MsgCode_Enum.SUCCESS)
            {
                logger.LogError(response.description, dntd);
            }
        }

        public async Task ReconcileDemandNotice()
        {
            List<DemandNotice> lst = await demandNoticeDao.GetUnSyncData();
            if (lst.Count > 0)
            {
                string query = string.Empty;
                foreach (var tm in lst)
                {
                    string qy = EncryptDecryptUtils.FromHexString(tm.query);
                    DemandNoticeRequest dnr = JsonConvert.DeserializeObject<DemandNoticeRequest>(qy);
                    if (dnr.wardId == null )
                    {
                        continue;
                    }
                    query = query + $"update tbl_demandnotice set wardId='{dnr.wardId}'," +
                        $" streetId='{dnr.streetId}' where id='{tm.id}';";
                }

                if (!string.IsNullOrEmpty(query))
                {
                    bool result = await demandNoticeDao.updateData(query);
                    if (!result)
                    {
                        logger.LogError("no record(s)");
                    }
                }
            }
        }

    }
}

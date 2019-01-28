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
        private readonly DemandNoticePaymentHistoryDao _dnpHisotryDao;
        private readonly IDNAmountDueMgtService _admService;
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
            IServiceProvider _serviceProvider,
            IDNAmountDueMgtService dNAmountDueMgtService)
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
            _admService = dNAmountDueMgtService;
            _dnpHisotryDao = new DemandNoticePaymentHistoryDao(_db);
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
                    List<Taxpayer> taxpayers = await taxpayerDao.GetActiveTaxpayers(demandNoticeRequest);
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

                        //if (demandNoticeRequest.CloseOldData)
                        //{
                        //    string[] taxpyIds = taxpayers.Select(x => string.Format("'{0}'", x.id)).ToArray();
                        //    await ClosedDDTaxpayers(taxpyIds, demandNotice.billingYear);
                        //    Error error = new Error()
                        //    {
                        //        errorType = ErrorType.DEMAND_NOTICE.ToString(),
                        //        errorvalue = $"{lcda.lcdaName} has been closed. Created by {demandNotice.createdBy}",
                        //        ownerId = demandNotice.id
                        //    };
                        //    bool result = await errorDao.Add(error);
                        //}

                        List<DemandNoticeTaxpayersDetail> dt =
                            await demandNoticeTaxpayersDao.getTaxpayerByIds(taxpayers
                            .Select(x => string.Format("'{0}'", x.id)).ToArray(), demandNotice.billingYear);

                        foreach (var tm in taxpayers)
                        {
                            var itExist = dt.FirstOrDefault(x => x.taxpayerId == tm.id);
                            if (itExist != null)
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
                                dntd.isUnbilled = demandNotice.isUnbilled;

                                Response response1 = await demandNoticeTaxpayersDao.Add(dntd);

                                if (response1.code == MsgCode_Enum.SUCCESS)
                                {
                                    dntd.billingNumber = response1.data.ToString().Trim();
                                    if (demandNoticeRequest.RunPenalty)
                                    {
                                        await RunTaxpayerPenalty(tm.id, dntd.billingNumber, dntd.billingYr);
                                    }

                                    if (demandNoticeRequest.RunArrears)
                                    {
                                        await RunArrears(dntd, dntd.billingNumber, demandNoticeRequest.RunArrearsCategory);
                                    }

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

        private async Task<bool> ClosedDDTaxpayers(string[] taxpayerIds, int billingYr)
        {
            return await demandNoticeTaxpayersDao.UpdateTaxpayers(taxpayerIds, billingYr, DemandNoticeStatus.CLOSED.ToString());
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

                        string rootUrl = hostingEnvironment.WebRootPath == null ? @"C:\" : hostingEnvironment.WebRootPath;

                        string rootPath = Path.Combine(rootUrl, "zipReports", bdnm.batchNo);
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

                            htmlContents = htmlContents + (s == null || string.IsNullOrEmpty(s) ? "" : s);
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
                        string rootUrl = hostingEnvironment.WebRootPath;
                        string rootPath = Path.Combine(hostingEnvironment.WebRootPath, "zipReports", bdnm.batchNo);
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

        private async Task RunArrears1(DemandNoticeTaxpayersDetail dntd)
        {
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

            // await MovedUnpaidPenalty(dN_ArrearsModel);
        }

        private async Task RunArrears2(DemandNoticeTaxpayersDetail dntd)
        {
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

            Response responseUnpaidDemandNotice = await demandNoticeArrearDao.AddUnpaidDemandNoticeToArrearsAsync2(dN_ArrearsModel);
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
        }

        private async Task<bool> RunArrears(DemandNoticeTaxpayersDetail dntd, string billNumber, int arrearCategory)
        {
            int billingYr = arrearCategory > 3 ? dntd.billingYr - 1 : dntd.billingYr;
            try
            {
                List<DemandNoticeItem> items = await demandNoticeItemDao.UnpaidBillsByTaxpayerId(dntd.taxpayerId, billNumber, billingYr);
                if (items.Count > 0)
                {
                    string oldBillnumber = items.FirstOrDefault().billingNo;
                    List<DemandNoticePaymentHistory> payments = await _dnpHisotryDao.ApprovedPaymentHistory(dntd.taxpayerId, billingYr);
                    var arrears = await demandNoticeArrearDao.ByTaxpayer(dntd.taxpayerId);
                    var penalty = await demandNoticePenaltyDao.ByTaxpayerId(dntd.taxpayerId, billingYr);

                    decimal amountDue = items.Sum(x => x.itemAmount) + arrears.Sum(x => x.totalAmount)
                        + penalty.Sum(x => x.totalAmount)
                         - payments.Sum(x => x.amount);

                    if (amountDue > 0)
                    {
                        DemandNoticeArrears dna = new DemandNoticeArrears()
                        {
                            amountPaid = payments.Sum(x => x.amount),
                            arrearsStatus = DemandNoticeStatus.PENDING.ToString(),
                            billingNo = billNumber,
                            billingYr = dntd.billingYr,
                            createdBy = "Application",
                            id = Guid.NewGuid(),
                            itemId = dntd.taxpayerId,
                            originatedYear = billingYr,
                            taxpayerId = dntd.taxpayerId,
                            totalAmount = amountDue
                        };

                        string query = string.Empty;
                        query = query + demandNoticeArrearDao.AddQuery(dna);
                        string itemQ = items.Select(x => x.id.ToString()).ToArray().FormatString();
                        if (!string.IsNullOrEmpty(itemQ))
                        {
                            query = query + $"update tbl_demandNoticeItem set itemStatus='MOVE_TO_ARREARS' " +
                                        $"where id in ({itemQ});";
                        }

                        if (arrears.Count > 0)
                        {
                            string arrearsQ = arrears.Select(x => x.id.ToString()).ToArray().FormatString();
                            query = query + $"update tbl_demandNoticeArrears set arrearsStatus = 'MOVED' " +
                                              $"where id in ({arrearsQ});";
                        }

                        query = query + $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = 'CLOSED' where billingNumber = '{oldBillnumber}'";

                        if (!string.IsNullOrEmpty(query))
                        {
                            return await demandNoticeArrearDao.AddArrears(query);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

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
                    if (dnr.wardId == null)
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

        public async Task RunTaxpayerPenalty(Guid taxpayerId, string billingNumber, int billingYr)
        {
            Guid[] taxpayerIds = new Guid[] { taxpayerId };
            var recievables = await demandNoticeTaxpayersDao.GetAllReceivables(taxpayerIds);// unpaid taxpayer

            var recievable = recievables.FirstOrDefault();
            List<DemandNoticeItem> items =
                    await demandNoticeItemDao.UnpaidBillsByTaxpayerId(taxpayerId, billingNumber, billingYr - 1);
            if (recievable != null && items.Count > 0)
            {
                List<DemandNoticePaymentHistory> payments = await _dnpHisotryDao.ApprovedPaymentHistory(taxpayerId, billingYr);
                var arrears = await demandNoticeArrearDao.ByTaxpayer(taxpayerId);
                decimal amountDue = items.Sum(x => x.itemAmount) + arrears.Sum(x => x.totalAmount)
                     - payments.Sum(x => x.amount);

                string query = string.Empty;
                if (amountDue > 0)
                {
                    DemandNoticeItemPenalty dnp = new DemandNoticeItemPenalty()
                    {
                        billingNo = billingNumber,
                        amountPaid = 0,
                        billingYr = billingYr,
                        itemId = Guid.Empty,
                        itemPenaltyStatus = DemandNoticeStatus.PENDING.ToString(),
                        originatedYear = recievable.billingYr,
                        taxpayerId = taxpayerId,
                        totalAmount = amountDue * (decimal)(0.1)
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
                            logger.LogInformation("Error running penalty " + response.description);
                        }
                        else
                        {
                            logger.LogInformation("Error running penalty " + response.description);
                        }
                    }
                    catch (Exception x)
                    {
                        logger.LogError(x.Message);
                    }
                }
            }
        }

    }
}

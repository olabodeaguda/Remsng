using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Dao;
using RemsNG.Data.Entities;
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
        private readonly CompanyItemRepository _companyItemDao;
        private readonly DNPaymentHistoryRepository _dnPaymentHistoryDao;
        private readonly DemandNoticePaymentHistoryRepository _dnpHisotryDao;
        private readonly IDNAmountDueMgtService _admService;
        private readonly TaxpayerRepository taxpayerDao;
        private readonly DemandNoticeRepository demandNoticeDao;
        private readonly DemandNoticeTaxpayersRepository demandNoticeTaxpayersDao;
        private readonly ErrorRepository errorDao;
        private readonly ILogger logger;
        private readonly DemandNoticeItemRepository demandNoticeItemDao;
        private readonly DemandNoticeArrearRepository demandNoticeArrearDao;
        private readonly DemandNoticePenaltyRepository demandNoticePenaltyDao;
        private readonly LcdaRepository lcdaDao;
        private readonly WardRepository wardDao;
        private readonly StreetRepository streetDao;
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
            taxpayerDao = new TaxpayerRepository(_db);
            demandNoticeDao = new DemandNoticeRepository(_db);
            demandNoticeTaxpayersDao = new DemandNoticeTaxpayersRepository(_db);
            errorDao = new ErrorRepository(_db, loggerFactory);
            demandNoticeItemDao = new DemandNoticeItemRepository(_db);
            demandNoticeArrearDao = new DemandNoticeArrearRepository(_db);
            demandNoticePenaltyDao = new DemandNoticePenaltyRepository(_db);
            lcdaDao = new LcdaRepository(_db, loggerFactory);
            wardDao = new WardRepository(_db);
            streetDao = new StreetRepository(_db, loggerFactory);
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
            _dnpHisotryDao = new DemandNoticePaymentHistoryRepository(_db);
            _dnPaymentHistoryDao = new DNPaymentHistoryRepository(_db);
            _companyItemDao = new CompanyItemRepository(_db);
        }

        public async Task RegisterTaxpayer()
        {
            DemandNotice demandNotice = await demandNoticeDao.DequeueDemandNotice();

            try
            {
                if (demandNotice != null)
                {
                    string query = EncryptDecryptUtils.FromHexString(demandNotice.Query);
                    DemandNoticeRequestModel demandNoticeRequest = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(query);
                    demandNoticeRequest.createdBy = demandNotice.CreatedBy;
                    List<TaxPayer> taxpayers = await taxpayerDao.GetActiveTaxpayers(demandNoticeRequest);
                    string domainName = string.Empty;
                    if (taxpayers.Count > 0)
                    {
                        Lcda lcda = await lcdaService.Get(demandNoticeRequest);
                        if (lcda != null)
                        {
                            Domain dd = await lcdaDao.GetDomain(lcda.Id);
                            if (dd != null)
                            {
                                domainName = dd.DomainName;
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

                        List<DemandNoticeTaxpayers> dt =
                            await demandNoticeTaxpayersDao.getTaxpayerByIds(taxpayers
                            .Select(x => string.Format("'{0}'", x.Id)).ToArray(), demandNotice.BillingYear);

                        foreach (var tm in taxpayers)
                        {
                            var itExist = dt.FirstOrDefault(x => x.TaxpayerId == tm.Id);
                            if (itExist != null)
                            {
                                Error error = new Error()
                                {
                                    ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                                    Errorvalue = $"Demand notice have already been raised for {tm.Surname} {tm.Firstname} {tm.Lastname} " +
                                      $"for billing year {demandNotice.BillingYear}",
                                    OwnerId = demandNotice.Id
                                };
                                bool result = await errorDao.Add(error);
                                continue;
                            }
                            else
                            {
                                DemandNoticeTaxpayers dntd = new DemandNoticeTaxpayers();
                                dntd.BillingYr = demandNotice.BillingYear;
                                dntd.DnId = demandNotice.Id;
                                dntd.CreatedBy = demandNoticeRequest.createdBy;
                                dntd.TaxpayerId = tm.Id;
                                dntd.DomainName = domainName;
                                dntd.LcdaAddress = await address.AddressByOwnerId(lcda.Id);
                                dntd.LcdaState = await stateService.StateNameByLcda(lcda.Id);
                                dntd.LcdaLogoFileName = await imageService.ImageNameByOwnerIdAsync(lcda.Id,
                                    ImgTypesEnum.LOGO.ToString());
                                dntd.RevCoodinatorSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.Id,
                                    ImgTypesEnum.REVENUE_COORDINATOR_SIGNATURE.ToString());
                                dntd.CouncilTreasurerSigFilen = await imageService.ImageNameByOwnerIdAsync(lcda.Id,
                                    ImgTypesEnum.COUNCIL_TREASURER_SIGNATURE.ToString());
                                dntd.IsUnbilled = demandNotice.IsUnbilled;

                                // get taxpayer company item
                                var items = await _companyItemDao.ByTaxpayer(tm.Id);
                                if (items.Count < 1)
                                {
                                    continue;
                                }

                                Response response1 = await demandNoticeTaxpayersDao.Add(dntd);

                                if (response1.code == MsgCode_Enum.SUCCESS)
                                {
                                    dntd.BillingNumber = response1.data.ToString().Trim();
                                    if (demandNoticeRequest.RunPenalty)
                                    {
                                        await RunTaxpayerPenalty(tm.Id, dntd.BillingNumber, dntd.BillingYr);
                                    }

                                    if (demandNoticeRequest.RunArrears)
                                    {
                                        await RunArrears(dntd, dntd.BillingNumber, demandNoticeRequest.RunArrearsCategory);
                                    }

                                    await RunDemandNoticeItem(dntd); //run items
                                }
                                else
                                {
                                    // log error
                                    Error error = new Error()
                                    {
                                        ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                                        Errorvalue = response1.description,
                                        OwnerId = dntd.DnId
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
                            ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                            Errorvalue = $"{taxpayers.Count} taxpayer was found from the query request ",
                            OwnerId = demandNotice.Id
                        };
                        bool result = await errorDao.Add(error);
                    }
                    //update demand notice
                    demandNotice.DemandNoticeStatus = DemandNoticeStatus.COMPLETED.ToString();
                    Response response = await demandNoticeDao.UpdateStatus(demandNotice);
                    if (response.code == MsgCode_Enum.SUCCESS)
                    {
                        logger.LogInformation($"{demandNotice.Id} has been completed");
                    }
                }
            }
            catch (Exception x)
            {
                // cancel no retry
                demandNotice.DemandNoticeStatus = DemandNoticeStatus.ERROR.ToString();
                Response response = await demandNoticeDao.UpdateStatus(demandNotice);
                if (response.code == MsgCode_Enum.SUCCESS)
                {
                    logger.LogInformation($"{demandNotice.Id} has been completed");
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
                    List<DemandNoticeTaxpayers> lstOfDN = await demandNoticeTaxpayerService.GetDNTaxpayerByBatchNoAsync(bdnm.batchNo);
                    if (lstOfDN.Count > 0)
                    {
                        var firstTaxpayer = lstOfDN[0];
                        Lcda lgda = await taxpayerService.getLcda(firstTaxpayer.TaxpayerId);
                        string template = await dnDownloadService.LcdaTemlateByLcda(lgda.Id);

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
                    List<DemandNoticeTaxpayers> lstOfDN = await demandNoticeTaxpayerService.GetDNTaxpayerByBatchNoAsync(bdnm.batchNo);
                    if (lstOfDN.Count > 0)
                    {
                        var firstTaxpayer = lstOfDN[0];
                        Lcda lgda = await taxpayerService.getLcda(firstTaxpayer.TaxpayerId);
                        string template = await dnDownloadService.LcdaTemlateByLcda(lgda.Id);
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
                                    htmlContent = await dnDownloadService.PopulateReportHtml(htmlContent, dnt.BillingNumber, rootUrl, bdnm.createdBy);

                                    htmlContent = htmlContent.Replace("PATCH1", "");
                                    htmlContent = htmlContent.Replace("PATCH2", "");

                                    var result = await nodeServices.InvokeAsync<byte[]>("./pdf",
                                        htmlContent);

                                    string filePath = Path.Combine(rootPath, $"{dnt.BillingNumber}.pdf");
                                    using (FileStream fs = System.IO.File.Create(filePath))
                                    {
                                        await fs.WriteAsync(result, 0, result.Length);
                                        fs.Flush();
                                    }
                                    archive.CreateEntryFromFile(filePath, $"{dnt.BillingNumber}.pdf");
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

        private async Task RunArrears1(DemandNoticeTaxpayers dntd)
        {
            DN_ArrearsModel dN_ArrearsModel = new DN_ArrearsModel()
            {
                arrearstatus = DemandNoticeStatus.PENDING.ToString(),
                billingNo = dntd.BillingNumber,
                billingYr = dntd.BillingYr,
                previousBillingYr = dntd.BillingYr,
                createdBy = dntd.CreatedBy,
                taxpayerId = dntd.TaxpayerId
            };

            Response responseUnpaidArrears = await demandNoticeArrearDao.AddUnpaidArrearsAsync(dN_ArrearsModel);
            if (responseUnpaidArrears.code != MsgCode_Enum.SUCCESS)
            {
                // logger error
                logger.LogError(responseUnpaidArrears.description);
                Error error = new Error()
                {
                    ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                    Errorvalue = $"Unpaid arrears section : {responseUnpaidArrears.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    OwnerId = dntd.DnId
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
                    ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                    Errorvalue = $"Unpaid demand notice arrears section: {responseUnpaidDemandNotice.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    OwnerId = dntd.DnId
                };
                bool result = await errorDao.Add(error);
            }

            // await MovedUnpaidPenalty(dN_ArrearsModel);
        }

        private async Task RunArrears2(DemandNoticeTaxpayers dntd)
        {
            DN_ArrearsModel dN_ArrearsModel = new DN_ArrearsModel()
            {
                arrearstatus = DemandNoticeStatus.PENDING.ToString(),
                billingNo = dntd.billingNumber,
                billingYr = dntd.BillingYr,
                previousBillingYr = dntd.BillingYr,
                createdBy = dntd.CreatedBy,
                taxpayerId = dntd.TaxpayerId
            };

            Response responseUnpaidArrears = await demandNoticeArrearDao.AddUnpaidArrearsAsync(dN_ArrearsModel);
            if (responseUnpaidArrears.code != MsgCode_Enum.SUCCESS)
            {
                // logger error
                logger.LogError(responseUnpaidArrears.description);
                Error error = new Error()
                {
                    ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                    Errorvalue = $"Unpaid arrears section : {responseUnpaidArrears.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    OwnerId = dntd.DnId
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
                    ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                    Errorvalue = $"Unpaid demand notice arrears section: {responseUnpaidDemandNotice.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    OwnerId = dntd.DnId
                };
                bool result = await errorDao.Add(error);
            }
        }

        private async Task<bool> RunArrears(DemandNoticeTaxpayers dntd, string billNumber, int arrearCategory)
        {
            int billingYr = arrearCategory > 3 ? dntd.BillingYr - 1 : dntd.BillingYr;
            try
            {
                List<DemandNoticeItem> items = await demandNoticeItemDao.UnpaidBillsByTaxpayerId(dntd.TaxpayerId, billNumber, billingYr);
                if (items.Count > 0)
                {
                    string oldBillnumber = items.FirstOrDefault().BillingNo;
                    List<DemandNoticePaymentHistory> payments = await _dnpHisotryDao.ApprovedPaymentHistory(dntd.TaxpayerId, billingYr);
                    var arrears = await demandNoticeArrearDao.ByTaxpayer(dntd.TaxpayerId);
                    var penalty = await demandNoticePenaltyDao.ByTaxpayerId(dntd.TaxpayerId, billingYr);

                    decimal amountDue = items.Sum(x => x.ItemAmount) + arrears.Sum(x => x.TotalAmount)
                        + penalty.Sum(x => x.totalAmount)
                         - payments.Sum(x => x.Amount);

                    if (amountDue > 0)
                    {
                        DemandNoticeArrears dna = new DemandNoticeArrears()
                        {
                            AmountPaid = payments.Sum(x => x.Amount),
                            ArrearsStatus = DemandNoticeStatus.PENDING.ToString(),
                            BillingNo = billNumber,
                            BillingYear = dntd.BillingYr,
                            CreatedBy = "Application",
                            Id = Guid.NewGuid(),
                            ItemId = dntd.TaxpayerId,
                            OriginatedYear = billingYr,
                            TaxpayerId = dntd.TaxpayerId,
                            TotalAmount = amountDue
                        };

                        string query = string.Empty;
                        query = query + demandNoticeArrearDao.AddQuery(dna);
                        string itemQ = items.Select(x => x.Id.ToString()).ToArray().FormatString();
                        if (!string.IsNullOrEmpty(itemQ))
                        {
                            query = query + $"update tbl_demandNoticeItem set itemStatus='MOVE_TO_ARREARS' " +
                                        $"where id in ({itemQ});";
                        }

                        if (arrears.Count > 0)
                        {
                            string arrearsQ = arrears.Select(x => x.Id.ToString()).ToArray().FormatString();
                            query = query + $"update tbl_demandNoticeArrears set arrearsStatus = 'MOVED' " +
                                              $"where id in ({arrearsQ});";
                        }

                        query = query + $"update tbl_demandNoticeTaxpayers set demandNoticeStatus = 'CLOSED' where billingNumber = '{oldBillnumber}'";

                        if (!string.IsNullOrEmpty(query))
                        {
                            return await demandNoticeArrearDao.AddArrears(query);
                        }
                    }
                    else if (amountDue < 0)
                    {
                        // register the prepayment
                        //await _dnPaymentHistoryDao.AddPrepaymentForAlreadyRegisterdAmount(new Prepayment()
                        //{
                        //    amount = amountDue * -1,
                        //    prepaymentStatus = "ACTIVE",
                        //    taxpayerId = dntd.taxpayerId
                        //});
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
                    ErrorType = ErrorType.DEMAND_NOTICE.ToString(),
                    Errorvalue = $"Unpaid demand notice Penalty movement section: " +
                    $"{responseUnpaidPenalty.description}, {JsonConvert.SerializeObject(dN_ArrearsModel)}",
                    OwnerId = dN_ArrearsModel.dnId
                };
                bool result = await errorDao.Add(error);
            }
        }

        private async Task RunDemandNoticeItem(DemandNoticeTaxpayers dntd)
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
                    string qy = EncryptDecryptUtils.FromHexString(tm.Query);
                    DemandNoticeRequestModel dnr = JsonConvert.DeserializeObject<DemandNoticeRequestModel>(qy);
                    if (dnr.wardId == null)
                    {
                        continue;
                    }
                    query = query + $"update tbl_demandnotice set wardId='{dnr.wardId}'," +
                        $" streetId='{dnr.streetId}' where id='{tm.Id}';";
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
                decimal amountDue = items.Sum(x => x.ItemAmount) + arrears.Sum(x => x.TotalAmount)
                     - payments.Sum(x => x.Amount);

                string query = string.Empty;
                if (amountDue > 0)
                {
                    DemandNoticeItemPenaltyModelExt dnp = new DemandNoticeItemPenaltyModelExt()
                    {
                        billingNo = billingNumber,
                        amountPaid = 0,
                        billingYear = billingYr,
                        itemId = Guid.Empty,
                        itemPenaltyStatus = DemandNoticeStatus.PENDING.ToString(),
                        originatedYear = recievable.BillingYr,
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

﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class RunDemandNoticeManager : IRunDemandNoticeManager
    {

        private readonly ILogger logger;
        private readonly IDnDownloadManager dnDownloadService;
        private readonly IBatchDwnRequestManager batchDwnRequestService;
        private readonly IDemandNoticeTaxpayerManager demandNoticeTaxpayerService;
        private readonly ITaxpayerManager taxpayerService;
        private IHostingEnvironment _hostEnvironment;
        private INodeServices nodeServices;

        public RunDemandNoticeManager(
            ILoggerFactory loggerFactory,
            IDnDownloadManager _dnDownloadService, IBatchDwnRequestManager _batchDwnRequestService,
            IDemandNoticeTaxpayerManager _demandNoticeTaxpayerService
            , ITaxpayerManager _taxpayerService,
            INodeServices _nodeServices,
            IHostingEnvironment hostingEnvironment
           )
        {
            logger = loggerFactory.CreateLogger("Demand Notice Jobs");
            nodeServices = _nodeServices;
            _hostEnvironment = hostingEnvironment;
            dnDownloadService = _dnDownloadService;
            batchDwnRequestService = _batchDwnRequestService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
        }

        public async Task GenerateBulkDemandNotice1()
        {
            BatchDemandNoticeModel bdnm = null;// await batchDwnRequestService.Dequeue();
            try
            {
                bdnm = await batchDwnRequestService.Dequeue();
                if (bdnm != null)
                {
                    List<DemandNoticeTaxpayersModel> lstOfDN = await demandNoticeTaxpayerService.GetDNTaxpayerByBatchNoAsync(bdnm.batchNo);
                    if (lstOfDN.Count > 0)
                    {
                        var firstTaxpayer = lstOfDN[0];
                        LcdaModel lgda = await taxpayerService.getLcda(firstTaxpayer.TaxpayerId);
                        string template = await dnDownloadService.LcdaTemlateByLcda(lgda.Id);

                        string rootUrl = _hostEnvironment.WebRootPath == null ? @"C:\" : _hostEnvironment.WebRootPath;

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
                            string s = await dnDownloadService.PopulateReportHtml(htmlContent, lstOfDN[i].BillingNumber, rootUrl, bdnm.createdBy);

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

        public async Task GenerateBulkDemandNotice()
        {
            BatchDemandNoticeModel bdnm = null;// await batchDwnRequestService.Dequeue();
            try
            {
                bdnm = await batchDwnRequestService.Dequeue();
                if (bdnm != null)
                {
                    List<DemandNoticeTaxpayersModel> lstOfDN = await demandNoticeTaxpayerService.GetDNTaxpayerByBatchNoAsync(bdnm.batchNo);
                    if (lstOfDN.Count > 0)
                    {
                        var firstTaxpayer = lstOfDN[0];
                        //LcdaModel lgda = await taxpayerService.getLcda(firstTaxpayer.TaxpayerId);
                        //string template = await dnDownloadService.LcdaTemlateByLcda(lgda.Id);

                        string template = await dnDownloadService.LcdaTemlate(firstTaxpayer.BillingNumber);
                        string rootUrl = _hostEnvironment.WebRootPath == null ? @"C:\" : _hostEnvironment.WebRootPath;
                        var htmlContent = await File.ReadAllTextAsync($"{rootUrl}/templates/{template}");
                        string rootPath = Path.Combine(rootUrl, "zipReports", bdnm.batchNo);
                        if (!Directory.Exists(rootPath))
                        {
                            Directory.CreateDirectory(rootPath);
                        }
                        List<byte[]> lstContent = new List<byte[]>();

                        int count = (lstOfDN.Count % 20 > 1 ? 1 : 0) + lstOfDN.Count / 20;

                        for (int i = 0; i < count; i++)
                        {
                            var billNos = lstOfDN.Select(x => x.BillingNumber).Skip(i * 20).Take(20).ToArray();
                            if (billNos.Length > 0)
                            {
                                var result = await dnDownloadService.PopulateReportHtml(htmlContent, billNos, rootUrl, bdnm.createdBy);
                                if (result.Length > 0)
                                {
                                    lstContent.Add(result);
                                }
                            }
                        }
                        count = 0;
                        using (FileStream zipToOpen = new FileStream($"{rootPath}/{bdnm.batchNo}.zip", FileMode.Create))
                        {
                            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                            {
                                foreach (var result in lstContent)
                                {
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
    }
}
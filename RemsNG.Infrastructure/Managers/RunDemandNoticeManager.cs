using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly TemplateDetail _templateDetails;

        public RunDemandNoticeManager(
            ILoggerFactory loggerFactory,
            IDnDownloadManager _dnDownloadService,
            IBatchDwnRequestManager _batchDwnRequestService,
            IDemandNoticeTaxpayerManager _demandNoticeTaxpayerService,
            TemplateDetail templateDetails)
        {
            logger = loggerFactory.CreateLogger("Demand Notice Jobs");
            _templateDetails = templateDetails;
            batchDwnRequestService = _batchDwnRequestService;
            demandNoticeTaxpayerService = _demandNoticeTaxpayerService;
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

                        string rootPath = Path.Combine(_templateDetails.ZipRepository, bdnm.batchNo);
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
                                var result = await dnDownloadService.GenerateDemandNotice(billNos, bdnm.createdBy);
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
                        requestStatus = $"{x.Message} {x.StackTrace} {x.InnerException}",
                        createdBy = "APPLICATION"
                    });
                }
                logger.LogError(x.Message);
            }
        }
    }
}

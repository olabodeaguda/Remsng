﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ExcelService : IExcelService
    {
        ILogger logger;
        private IHostingEnvironment hostingEnvironment;
        public ExcelService(ILoggerFactory loggerFactory, IHostingEnvironment _hostingEnvironment)
        {
            logger = loggerFactory.CreateLogger("Excel Service");
            hostingEnvironment = _hostingEnvironment;
        }

        private void CellStyleHeader(ICell cell, IWorkbook workbook, int fontSize)
        {
            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            IFont font = workbook.CreateFont();
            AddBold(font, fontSize);
            cellStyle.SetFont(font);
            cell.CellStyle = cellStyle;
        }

        private void AddBold(IFont font, int size)
        {
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.FontHeightInPoints = (short)size;
        }

        public async Task<byte[]> WriteReportSummary(List<ItemReportSummaryModel> rptLst,
            List<ItemReportSummaryModel> previousYearList
            , string domainName, string lcdaName, DateTime startDate, DateTime enndDate)
        {
            try
            {
                if (rptLst.Count < 1)
                {
                    return null;
                }

                return await Task.Run(() =>
                {
                    string[] wards = rptLst.Select(x => x.wardName).Distinct().ToArray();
                    int rowCount = wards.Length + 6;

                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet1 = workbook.CreateSheet("Report Summary");

                    #region sub heading
                    sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, rowCount));
                    sheet1.AddMergedRegion(new CellRangeAddress(1, 1, 0, rowCount));
                    sheet1.AddMergedRegion(new CellRangeAddress(2, 2, 0, rowCount));
                    sheet1.AddMergedRegion(new CellRangeAddress(3, 3, 0, rowCount));

                    var rowIndex = 0;
                    IRow rowDomain = sheet1.CreateRow(rowIndex);
                    rowDomain.Height = 400;
                    ICell cellDomain = rowDomain.CreateCell(0);
                    cellDomain.SetCellValue(domainName.ToUpper());
                    CellStyleHeader(cellDomain, workbook, 13);
                    rowIndex++;

                    IRow rowLcda = sheet1.CreateRow(rowIndex);
                    rowLcda.Height = 300;
                    ICell cellLcda = rowLcda.CreateCell(0);
                    cellLcda.SetCellValue(lcdaName.ToUpper());
                    CellStyleHeader(cellLcda, workbook, 10);
                    rowIndex++;

                    IRow rowTitle1 = sheet1.CreateRow(rowIndex);
                    rowTitle1.Height = 300;
                    ICell cellTitle1 = rowTitle1.CreateCell(0);
                    cellTitle1.SetCellValue($"INTERNALLY GENERATED REVENUE " +
                        $"FOR THE PERIOD OF {startDate.ToString("dd/MM/yyyy")} - {enndDate.ToString("dd/MM/yyyy")}");
                    CellStyleHeader(cellTitle1, workbook, 10);
                    rowIndex++;

                    IRow rowTitle2 = sheet1.CreateRow(rowIndex);
                    rowTitle2.Height = 300;
                    ICell cellTitle2 = rowTitle2.CreateCell(0);
                    cellTitle2.SetCellValue("REVENUE APPRAISAL SHEET");
                    CellStyleHeader(cellTitle2, workbook, 10);
                    rowIndex++;

                    #endregion

                    #region header

                    IRow rowHeader = sheet1.CreateRow(rowIndex++);
                    int colCount = 0;
                    rowHeader.CreateCell(colCount++).SetCellValue("SN");
                    rowHeader.CreateCell(colCount++).SetCellValue("DETAILS OF REVENUE");
                    rowHeader.CreateCell(colCount++).SetCellValue("ITEM CODE");
                    rowHeader.CreateCell(colCount++).SetCellValue("APPROVED ESTIMATE");
                    foreach (var tm in wards)
                    {
                        rowHeader.CreateCell(colCount++).SetCellValue(tm.ToUpper());
                    }

                    rowHeader.CreateCell(colCount++).SetCellValue("CURRENT WEEK");
                    rowHeader.CreateCell(colCount++).SetCellValue("PREVIOUS AMOUNT");
                    rowHeader.CreateCell(colCount++).SetCellValue("TO DATE");
                    rowHeader.CreateCell(colCount++).SetCellValue("Cumulative");

                    #endregion

                    string[] items = rptLst.Select(x => x.itemDescription).Distinct().ToArray();
                    decimal totalApproveEstimate = 0;
                    decimal totalCurrentAMountPaid = 0;
                    decimal totalPreviousAMountPaid = 0;

                    decimal cumulative = 0;
                    for (int i = 0; i < items.Length; i++)
                    {
                        #region body
                        colCount = 0;
                        IRow rowbody = sheet1.CreateRow(rowIndex++);
                        rowbody.CreateCell(colCount++).SetCellValue(i + 1);
                        rowbody.CreateCell(colCount++).SetCellValue(items[i]);
                        decimal estmatAmount = rptLst.Where(x => x.itemDescription == items[i]).Sum(x => x.itemAmount);
                        var itemCode = rptLst.Where(x => x.itemDescription == items[i]).FirstOrDefault();
                        rowbody.CreateCell(colCount++).SetCellValue(itemCode == null ? "Empty" : itemCode.itemCode);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(estmatAmount, 2)));
                        totalApproveEstimate = totalApproveEstimate + estmatAmount;

                        foreach (var tm in wards)
                        {
                            decimal wardPaid = rptLst.Where(x => x.wardName == tm && x.itemDescription == items[i]).Sum(x => x.amountPaid);
                            rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(wardPaid, 2)));
                        }

                        decimal currentAmountPaid = rptLst.Where(x => x.itemDescription == items[i]).Sum(x => x.amountPaid);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(currentAmountPaid, 2)));
                        totalCurrentAMountPaid = totalCurrentAMountPaid + currentAmountPaid;

                        decimal previousAmount = previousYearList.Where(x => x.itemDescription == items[i]).Sum(x => x.amountPaid);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(previousAmount, 2)));
                        totalPreviousAMountPaid = totalPreviousAMountPaid + previousAmount;

                        cumulative = cumulative + decimal.Round((previousAmount + currentAmountPaid), 2);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round((previousAmount + currentAmountPaid), 2)));
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", cumulative));
                        #endregion
                    }

                    IRow rowbody1 = sheet1.CreateRow(rowIndex++);
                    rowbody1.CreateCell(3).SetCellValue(String.Format("{0:n}", decimal.Round(totalApproveEstimate, 2)));
                    rowbody1.CreateCell(colCount - 4).SetCellValue(String.Format("{0:n}", decimal.Round(totalCurrentAMountPaid, 2)));
                    rowbody1.CreateCell(colCount - 3).SetCellValue(String.Format("{0:n}", decimal.Round(totalPreviousAMountPaid, 2)));
                    rowbody1.CreateCell(colCount - 2).SetCellValue(String.Format("{0:n}", decimal.Round(cumulative, 2)));

                    sheet1.AutoSizeColumn(0);
                    MemoryStream memo = new MemoryStream();
                    workbook.Write(memo);
                    return memo.ToArray();
                });
            }
            catch (Exception x)
            {
                logger.LogError(x.Message);
                return null;
            }
        }


        public async Task<byte[]> WriteReportSummary(List<ItemReportSummaryModel> rptLst,
            string domainName, string lcdaName, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (rptLst.Count < 1)
                {
                    return null;
                }

                return await Task.Run(() =>
                {
                    string[] items = rptLst.Select(x => x.itemDescription).Distinct().OrderBy(x => x).ToArray();
                    int rowCount = items.Length + 8;

                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet1 = workbook.CreateSheet("Report Summary");

                    #region sub heading
                    sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, rowCount));
                    sheet1.AddMergedRegion(new CellRangeAddress(1, 1, 0, rowCount));
                    sheet1.AddMergedRegion(new CellRangeAddress(2, 2, 0, rowCount));
                    sheet1.AddMergedRegion(new CellRangeAddress(3, 3, 0, rowCount));

                    var rowIndex = 0;
                    IRow rowDomain = sheet1.CreateRow(rowIndex);
                    rowDomain.Height = 400;
                    ICell cellDomain = rowDomain.CreateCell(0);
                    cellDomain.SetCellValue(domainName.ToUpper());
                    CellStyleHeader(cellDomain, workbook, 13);
                    rowIndex++;

                    IRow rowLcda = sheet1.CreateRow(rowIndex);
                    rowLcda.Height = 300;
                    ICell cellLcda = rowLcda.CreateCell(0);
                    cellLcda.SetCellValue(lcdaName.ToUpper());
                    CellStyleHeader(cellLcda, workbook, 10);
                    rowIndex++;

                    IRow rowTitle1 = sheet1.CreateRow(rowIndex);
                    rowTitle1.Height = 300;
                    ICell cellTitle1 = rowTitle1.CreateCell(0);
                    cellTitle1.SetCellValue($"INTERNALLY GENERATED REVENUE " +
                        $"FOR THE PERIOD OF {startDate.ToString("dd/MM/yyyy")} - {endDate.ToString("dd/MM/yyyy")}");
                    CellStyleHeader(cellTitle1, workbook, 10);
                    rowIndex++;

                    IRow rowTitle2 = sheet1.CreateRow(rowIndex);
                    rowTitle2.Height = 300;
                    ICell cellTitle2 = rowTitle2.CreateCell(0);
                    cellTitle2.SetCellValue("REVENUE APPRAISAL SHEET BREAKDOWN");
                    CellStyleHeader(cellTitle2, workbook, 10);
                    rowIndex++;

                    #endregion

                    #region header

                    IRow rowHeader = sheet1.CreateRow(rowIndex++);
                    int colCount = 0;
                    rowHeader.CreateCell(colCount++).SetCellValue("SN");
                    rowHeader.CreateCell(colCount++).SetCellValue("TAXPAYER'S");
                    rowHeader.CreateCell(colCount++).SetCellValue("WARD");
                    rowHeader.CreateCell(colCount++).SetCellValue("BILLING NO");
                    foreach (var tm in items)
                    {
                        rowHeader.CreateCell(colCount++).SetCellValue(tm.ToUpper());
                    }

                    rowHeader.CreateCell(colCount++).SetCellValue("ARREARS");
                    rowHeader.CreateCell(colCount++).SetCellValue("PENALTIES");
                    rowHeader.CreateCell(colCount++).SetCellValue("AMOUNT PAID");
                    rowHeader.CreateCell(colCount++).SetCellValue("AMOUNT OWE");

                    #endregion

                    var allitems = rptLst.GroupBy(x => x.billingNo);

                    decimal totalAmountPaid = 0;
                    decimal totalAllAMount = 0;
                    foreach (var eachGrp in allitems)
                    {
                        #region body
                        colCount = 0;
                        IRow rowbody = sheet1.CreateRow(rowIndex++);
                        var firstTaxpayer = eachGrp.FirstOrDefault();

                        rowbody.CreateCell(colCount++).SetCellValue(rowIndex - 5);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.taxpayersName);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.wardName);
                        rowbody.CreateCell(colCount++).SetCellValue(eachGrp.Key);

                        var taxPItems = eachGrp.Where(x => x.category == "ITEMS");
                        foreach (var tm in items)
                        {
                            decimal h = taxPItems.Where(x => x.itemDescription == tm).Sum(x => x.itemAmount);
                            rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(h, 2)));
                        }
                        decimal arrears = eachGrp.Where(x => x.category == "ARREARS").Sum(x => x.itemAmount);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(arrears, 2)));

                        decimal penalties = eachGrp.Where(x => x.category == "PENALTY").Sum(x => x.itemAmount);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(penalties, 2)));

                        decimal amountPaid = eachGrp.Sum(x => x.amountPaid);
                        totalAmountPaid = totalAmountPaid + amountPaid;
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(amountPaid, 2)));

                        decimal totalAmount = eachGrp.Sum(x => x.itemAmount);
                        totalAllAMount = totalAllAMount + totalAmount;
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(totalAmount - amountPaid, 2)));

                        #endregion
                    }

                    colCount = items.Count() + 8;
                    IRow rowbody1 = sheet1.CreateRow(rowIndex++);
                    rowbody1.CreateCell(colCount - 2).SetCellValue(String.Format("{0:n}", decimal.Round(totalAmountPaid, 2)));
                    rowbody1.CreateCell(colCount - 1).SetCellValue(String.Format("{0:n}", decimal.Round(totalAllAMount, 2)));

                    sheet1.AutoSizeColumn(0);
                    MemoryStream memo = new MemoryStream();
                    workbook.Write(memo);
                    return memo.ToArray();
                });
            }
            catch (Exception x)
            {
                logger.LogError(x.Message);
                return null;
            }
        }

        public async Task<byte[]> WriteReportSummaryConsolidated(List<ItemReportSummaryModel> rptLst,
           string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistory> dnph)
        {
            try
            {
                if (rptLst.Count < 1)
                {
                    return null;
                }

                return await Task.Run(() =>
                {
                    string[] items = rptLst.Select(x => x.itemDescription).Distinct().OrderBy(x => x).ToArray();
                    int rowCount = items.Length + 8;

                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet1 = workbook.CreateSheet("Report Summary");

                    #region sub heading
                    sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 13));
                    sheet1.AddMergedRegion(new CellRangeAddress(1, 1, 0, 13));
                    sheet1.AddMergedRegion(new CellRangeAddress(2, 2, 0, 13));
                    sheet1.AddMergedRegion(new CellRangeAddress(3, 3, 0, 13));

                    var rowIndex = 0;
                    IRow rowDomain = sheet1.CreateRow(rowIndex);
                    rowDomain.Height = 400;
                    ICell cellDomain = rowDomain.CreateCell(0);
                    cellDomain.SetCellValue(domainName.ToUpper());
                    CellStyleHeader(cellDomain, workbook, 13);
                    rowIndex++;

                    IRow rowLcda = sheet1.CreateRow(rowIndex);
                    rowLcda.Height = 300;
                    ICell cellLcda = rowLcda.CreateCell(0);
                    cellLcda.SetCellValue(lcdaName.ToUpper());
                    CellStyleHeader(cellLcda, workbook, 10);
                    rowIndex++;

                    IRow rowTitle1 = sheet1.CreateRow(rowIndex);
                    rowTitle1.Height = 300;
                    ICell cellTitle1 = rowTitle1.CreateCell(0);
                    cellTitle1.SetCellValue($"INTERNALLY GENERATED REVENUE " +
                        $"FOR THE PERIOD OF {startDate.ToString("dd/MM/yyyy")} - {endDate.ToString("dd/MM/yyyy")}");
                    CellStyleHeader(cellTitle1, workbook, 10);
                    rowIndex++;

                    IRow rowTitle2 = sheet1.CreateRow(rowIndex);
                    rowTitle2.Height = 300;
                    ICell cellTitle2 = rowTitle2.CreateCell(0);
                    cellTitle2.SetCellValue("REVENUE APPRAISAL SHEET BREAKDOWN");
                    CellStyleHeader(cellTitle2, workbook, 10);
                    rowIndex++;

                    #endregion

                    #region header

                    IRow rowHeader = sheet1.CreateRow(rowIndex++);
                    int colCount = 0;
                    rowHeader.CreateCell(colCount++).SetCellValue("SN");
                    rowHeader.CreateCell(colCount++).SetCellValue("TAXPAYER'S");
                    rowHeader.CreateCell(colCount++).SetCellValue("WARD");
                    rowHeader.CreateCell(colCount++).SetCellValue("ADDRESS");
                    rowHeader.CreateCell(colCount++).SetCellValue("BILLING NO");
                    rowHeader.CreateCell(colCount++).SetCellValue("Items");
                    //foreach (var tm in items)
                    //{
                    //    rowHeader.CreateCell(colCount++).SetCellValue(tm.ToUpper());
                    //}

                    rowHeader.CreateCell(colCount++).SetCellValue("ARREARS");
                    rowHeader.CreateCell(colCount++).SetCellValue("PENALTIES");
                    rowHeader.CreateCell(colCount++).SetCellValue("AMOUNT PAID");
                    rowHeader.CreateCell(colCount++).SetCellValue("OUTSTANDING AMOUNT");
                    rowHeader.CreateCell(colCount++).SetCellValue("PAYMENT DATE");
                    rowHeader.CreateCell(colCount++).SetCellValue("BANK");
                    rowHeader.CreateCell(colCount++).SetCellValue("TELLER NO");

                    #endregion

                    var allitems = rptLst.GroupBy(x => x.billingNo);

                    decimal totalAmountPaid = 0;
                    decimal totalAllAMount = 0;
                    foreach (var eachGrp in allitems)
                    {
                        #region body
                        colCount = 0;
                        IRow rowbody = sheet1.CreateRow(rowIndex++);
                        var firstTaxpayer = eachGrp.FirstOrDefault();

                        rowbody.CreateCell(colCount++).SetCellValue(rowIndex - 5);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.taxpayersName);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.wardName);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.addressName);
                        rowbody.CreateCell(colCount++).SetCellValue(eachGrp.Key);

                        var taxPItems = eachGrp.Where(x => x.category == "ITEMS");
                        //foreach (var tm in items)
                        //{
                        //    decimal h = taxPItems.Where(x => x.itemDescription == tm).Sum(x => x.itemAmount);
                        //    rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(h, 2)));
                        //}
                        string itms = string.Join(",", taxPItems.Select(x => x.itemDescription).ToArray());
                        rowbody.CreateCell(colCount++).SetCellValue(itms);

                        decimal arrears = eachGrp.Where(x => x.category == "ARREARS").Sum(x => x.itemAmount);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(arrears, 2)));

                        decimal penalties = eachGrp.Where(x => x.category == "PENALTY").Sum(x => x.itemAmount);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(penalties, 2)));

                        decimal amountPaid = eachGrp.Sum(x => x.amountPaid);
                        totalAmountPaid = totalAmountPaid + amountPaid;
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(amountPaid, 2)));

                        decimal totalAmount = eachGrp.Sum(x => x.itemAmount);
                        totalAllAMount = totalAllAMount + totalAmount;
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(totalAmount - amountPaid, 2)));
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.lastModifiedDate != null ?
                            firstTaxpayer.lastModifiedDate.Value.ToShortDateString() : "");

                        var res = dnph.Where(x => x.billingNumber == firstTaxpayer.billingNo);
                        if (res.Count() > 0)
                        {
                            rowbody.CreateCell(colCount++).SetCellValue(string.Join(",", res.Select(x => x.bankName)));
                            rowbody.CreateCell(colCount++).SetCellValue(string.Join(",", res.Select(x => x.referenceNumber)));
                        }

                        #endregion
                    }

                    colCount = items.Count() + 8;
                    IRow rowbody1 = sheet1.CreateRow(rowIndex++);
                    rowbody1.CreateCell(8).SetCellValue(String.Format("{0:n}", decimal.Round(totalAmountPaid, 2)));
                    rowbody1.CreateCell(9).SetCellValue(String.Format("{0:n}", decimal.Round(totalAllAMount, 2)));

                    sheet1.AutoSizeColumn(0);
                    MemoryStream memo = new MemoryStream();
                    workbook.Write(memo);
                    return memo.ToArray();
                });
            }
            catch (Exception x)
            {
                logger.LogError(x.Message);
                return null;
            }
        }

        public async Task<string> WriteReportSummaryConsolidatedSeperate(List<ItemReportSummaryModel> rptLst,
          string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistory> dnph)
        {
            try
            {
                if (rptLst.Count < 1)
                {
                    return null;
                }

                return await Task.Run(() =>
                {
                    string[] items = rptLst.Select(x => x.itemDescription).Distinct().OrderBy(x => x).ToArray();
                    int rowCount = items.Length + 8;

                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet1 = workbook.CreateSheet("Report Summary");

                    #region sub heading
                    sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 13));
                    sheet1.AddMergedRegion(new CellRangeAddress(1, 1, 0, 13));
                    sheet1.AddMergedRegion(new CellRangeAddress(2, 2, 0, 13));
                    sheet1.AddMergedRegion(new CellRangeAddress(3, 3, 0, 13));

                    var rowIndex = 0;
                    IRow rowDomain = sheet1.CreateRow(rowIndex);
                    rowDomain.Height = 400;
                    ICell cellDomain = rowDomain.CreateCell(0);
                    cellDomain.SetCellValue(domainName.ToUpper());
                    CellStyleHeader(cellDomain, workbook, 13);
                    rowIndex++;

                    IRow rowLcda = sheet1.CreateRow(rowIndex);
                    rowLcda.Height = 300;
                    ICell cellLcda = rowLcda.CreateCell(0);
                    cellLcda.SetCellValue(lcdaName.ToUpper());
                    CellStyleHeader(cellLcda, workbook, 10);
                    rowIndex++;

                    IRow rowTitle1 = sheet1.CreateRow(rowIndex);
                    rowTitle1.Height = 300;
                    ICell cellTitle1 = rowTitle1.CreateCell(0);
                    cellTitle1.SetCellValue($"INTERNALLY GENERATED REVENUE " +
                        $"FOR THE PERIOD OF {startDate.ToString("dd/MM/yyyy")} - {endDate.ToString("dd/MM/yyyy")}");
                    CellStyleHeader(cellTitle1, workbook, 10);
                    rowIndex++;

                    IRow rowTitle2 = sheet1.CreateRow(rowIndex);
                    rowTitle2.Height = 300;
                    ICell cellTitle2 = rowTitle2.CreateCell(0);
                    cellTitle2.SetCellValue("REVENUE APPRAISAL SHEET BREAKDOWN");
                    CellStyleHeader(cellTitle2, workbook, 10);
                    rowIndex++;

                    #endregion

                    #region header
                    IRow rowHeader = sheet1.CreateRow(rowIndex++);
                    int colCount = 0;
                    rowHeader.CreateCell(colCount++).SetCellValue("SN");
                    rowHeader.CreateCell(colCount++).SetCellValue("TAXPAYER'S");
                    rowHeader.CreateCell(colCount++).SetCellValue("WARD");
                    rowHeader.CreateCell(colCount++).SetCellValue("ADDRESS");
                    rowHeader.CreateCell(colCount++).SetCellValue("BILLING NO");
                    rowHeader.CreateCell(colCount++).SetCellValue("Items");
                    rowHeader.CreateCell(colCount++).SetCellValue("OUTSTANDING AMOUNT");
                    #endregion

                    var allitems = rptLst.GroupBy(x => x.billingNo);

                    foreach (var eachGrp in allitems)
                    {
                        #region body
                        colCount = 0;
                        IRow rowbody = sheet1.CreateRow(rowIndex++);
                        var firstTaxpayer = eachGrp.FirstOrDefault();
                        rowbody.CreateCell(colCount++).SetCellValue(rowIndex - 5);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.taxpayersName);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.wardName);
                        rowbody.CreateCell(colCount++).SetCellValue(firstTaxpayer.addressName);
                        rowbody.CreateCell(colCount++).SetCellValue(eachGrp.Key);
                        rowbody.CreateCell(colCount++).SetCellValue(string.Join(",", eachGrp.Select(x => x.itemDescription).ToArray()));
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(eachGrp.Sum(x => x.itemAmount), 2)));
                        #endregion
                    }
                    IRow rowbody1 = sheet1.CreateRow(rowIndex++);
                    rowbody1.CreateCell(6).SetCellValue(String.Format("{0:n}", decimal.Round(rptLst.Sum(x => x.itemAmount), 2)));

                    sheet1.AutoSizeColumn(0);
                    WritePayment(workbook, domainName,
                        lcdaName, startDate, endDate, dnph);

                    string rootUrl = Directory.GetCurrentDirectory() == null ? @"C:\" : Directory.GetCurrentDirectory();
                    string pathname = $"{startDate.ToString("dd-MM-yyyy")} to {endDate.ToString("dd-MM-yyyy")} {DateTime.Now.ToString("HHmmss")}.xlsx";
                    string rootPath = Path.Combine(rootUrl, "QuaterlyReport");
                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);

                    }

                    rootPath = Path.Combine(rootPath, pathname);
                    if (File.Exists(rootPath))
                    {
                        File.Delete(rootPath);
                    }
                    using (var file2 = new FileStream(rootPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        workbook.Write(file2);
                        file2.Close();
                    }

                    return pathname;
                });
            }
            catch (Exception x)
            {
                logger.LogError(x.Message);
                return null;
            }
        }


        public void WritePayment(IWorkbook workbook,
            string domainName, string lcdaName, DateTime startDate, DateTime endDate, List<DemandNoticePaymentHistory> dnph)
        {
            ISheet sheet2 = workbook.CreateSheet("Payment Summary");

            #region sub heading
            sheet2.AddMergedRegion(new CellRangeAddress(0, 0, 0, 13));
            sheet2.AddMergedRegion(new CellRangeAddress(1, 1, 0, 13));
            sheet2.AddMergedRegion(new CellRangeAddress(2, 2, 0, 13));
            sheet2.AddMergedRegion(new CellRangeAddress(3, 3, 0, 13));

            var rowIndex = 0;
            IRow rowDomain = sheet2.CreateRow(rowIndex);
            rowDomain.Height = 400;
            ICell cellDomain = rowDomain.CreateCell(0);
            cellDomain.SetCellValue(domainName.ToUpper());
            CellStyleHeader(cellDomain, workbook, 13);
            rowIndex++;

            IRow rowLcda = sheet2.CreateRow(rowIndex);
            rowLcda.Height = 300;
            ICell cellLcda = rowLcda.CreateCell(0);
            cellLcda.SetCellValue(lcdaName.ToUpper());
            CellStyleHeader(cellLcda, workbook, 10);
            rowIndex++;

            IRow rowTitle1 = sheet2.CreateRow(rowIndex);
            rowTitle1.Height = 300;
            ICell cellTitle1 = rowTitle1.CreateCell(0);
            cellTitle1.SetCellValue($"INTERNALLY GENERATED REVENUE " +
                $"FOR THE PERIOD OF {startDate.ToString("dd/MM/yyyy")} - {endDate.ToString("dd/MM/yyyy")}");
            CellStyleHeader(cellTitle1, workbook, 10);
            rowIndex++;

            IRow rowTitle2 = sheet2.CreateRow(rowIndex);
            rowTitle2.Height = 300;
            ICell cellTitle2 = rowTitle2.CreateCell(0);
            cellTitle2.SetCellValue("REVENUE PAYMENT APPRAISAL SHEET BREAKDOWN");
            CellStyleHeader(cellTitle2, workbook, 10);
            rowIndex++;

            #endregion

            #region header
            IRow rowHeader = sheet2.CreateRow(rowIndex++);
            int colCount = 0;
            rowHeader.CreateCell(colCount++).SetCellValue("SN");
            rowHeader.CreateCell(colCount++).SetCellValue("BILLING NUMBER");
            rowHeader.CreateCell(colCount++).SetCellValue("AMOUNT");
            rowHeader.CreateCell(colCount++).SetCellValue("PAYMENT MODE");
            rowHeader.CreateCell(colCount++).SetCellValue("REFERENCE NUMBER");
            rowHeader.CreateCell(colCount++).SetCellValue("BANK NAME");
            rowHeader.CreateCell(colCount++).SetCellValue("PAYMENT STATUS");
            #endregion

            foreach (var tm in dnph)
            {
                #region body
                colCount = 0;
                IRow rowbody = sheet2.CreateRow(rowIndex++);
                rowbody.CreateCell(colCount++).SetCellValue(rowIndex - 5);
                rowbody.CreateCell(colCount++).SetCellValue(tm.billingNumber);
                rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(tm.amount, 2)));
                rowbody.CreateCell(colCount++).SetCellValue(tm.paymentMode);
                rowbody.CreateCell(colCount++).SetCellValue(tm.referenceNumber);
                rowbody.CreateCell(colCount++).SetCellValue(tm.bankName);
                rowbody.CreateCell(colCount++).SetCellValue(tm.paymentStatus);
                #endregion
            }
            IRow rowbody1 = sheet2.CreateRow(rowIndex++);
            rowbody1.CreateCell(2).SetCellValue(String.Format("{0:n}", decimal.Round(dnph.Sum(x => x.amount), 2)));

            sheet2.AutoSizeColumn(0);
        }

        public async Task<byte[]> WriteReportCategory(string domainName, string lcdaName, DateTime startDate,
            DateTime endDate, List<DemandNoticeItemExt> dnitem, List<DemandNoticeItemPenaltyExt> dnPenalty,
            List<DemandNoticeArrearsExt> dnArrears)
        {
            return await Task.Run(() =>
            {
                IWorkbook workbook = new XSSFWorkbook();
                if (dnitem.Count < 1)
                {
                    return null;
                }

                var result = dnitem.GroupBy(x => x.category);

                foreach (var tm in result)
                {
                    var rptCategory = tm.ToList();
                    // int rowCount = 0;
                    ISheet sheet1 = workbook.CreateSheet(tm.Key);

                    #region sub heading
                    sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));
                    sheet1.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));
                    sheet1.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));
                    sheet1.AddMergedRegion(new CellRangeAddress(3, 3, 0, 4));

                    var rowIndex = 0;
                    IRow rowDomain = sheet1.CreateRow(rowIndex);
                    rowDomain.Height = 400;
                    ICell cellDomain = rowDomain.CreateCell(0);
                    cellDomain.SetCellValue(domainName.ToUpper());
                    CellStyleHeader(cellDomain, workbook, 13);
                    rowIndex++;

                    IRow rowLcda = sheet1.CreateRow(rowIndex);
                    rowLcda.Height = 300;
                    ICell cellLcda = rowLcda.CreateCell(0);
                    cellLcda.SetCellValue(lcdaName.ToUpper());
                    CellStyleHeader(cellLcda, workbook, 10);
                    rowIndex++;

                    IRow rowTitle1 = sheet1.CreateRow(rowIndex);
                    rowTitle1.Height = 300;
                    ICell cellTitle1 = rowTitle1.CreateCell(0);
                    cellTitle1.SetCellValue($"INTERNALLY GENERATED REVENUE " +
                        $"FOR THE PERIOD OF {startDate.ToString("dd/MM/yyyy")} - {endDate.ToString("dd/MM/yyyy")}");
                    CellStyleHeader(cellTitle1, workbook, 10);
                    rowIndex++;

                    IRow rowTitle2 = sheet1.CreateRow(rowIndex);
                    rowTitle2.Height = 300;
                    ICell cellTitle2 = rowTitle2.CreateCell(0);
                    cellTitle2.SetCellValue("REVENUE APPRAISAL SHEET BREAKDOWN");
                    CellStyleHeader(cellTitle2, workbook, 10);
                    rowIndex++;

                    #endregion

                    var rptWard = rptCategory.GroupBy(x => x.wardName);
                    foreach (var rmp in rptWard)
                    {
                        IRow rowWardTitle = sheet1.CreateRow(rowIndex++);
                        rowWardTitle.Height = 400;
                        ICell cellWardTitle = rowWardTitle.CreateCell(0);
                        cellWardTitle.SetCellValue(rmp.Key.ToUpper());
                        CellStyleHeader(cellWardTitle, workbook, 13);
                        rowIndex++;

                        var rptWardData = rmp.ToList();

                        #region header
                        IRow rowHeader = sheet1.CreateRow(rowIndex++);
                        rowHeader.CreateCell(0).SetCellValue("SN");
                        rowHeader.CreateCell(1).SetCellValue("Item");
                        rowHeader.CreateCell(2).SetCellValue("Amount  Due");
                        rowHeader.CreateCell(3).SetCellValue("Amount Paid");
                        #endregion


                        int cc = 0;
                        foreach (var t in rptWardData.GroupBy(x => x.itemName))
                        {
                            IRow rowHeader1 = sheet1.CreateRow(rowIndex++);
                            rowHeader1.CreateCell(0).SetCellValue(cc + 1);
                            rowHeader1.CreateCell(1).SetCellValue(t.Key);
                            rowHeader1.CreateCell(2).SetCellValue(String.Format("{0:n}", decimal.Round(t.Sum(x => x.itemAmount), 2)));
                            rowHeader1.CreateCell(3).SetCellValue(String.Format("{0:n}", decimal.Round(t.Sum(x => x.amountPaid), 2)));
                            cc++;
                        }

                        var arrears = dnArrears.Where(x => x.wardName == rmp.Key && x.category == tm.Key);
                        if (arrears.Count() > 0)
                        {

                            IRow rowHeader1 = sheet1.CreateRow(rowIndex++);
                            rowHeader1.CreateCell(0).SetCellValue(cc + 1);
                            rowHeader1.CreateCell(1).SetCellValue("Arrears");
                            rowHeader1.CreateCell(2).SetCellValue(String.Format("{0:n}", decimal.Round(arrears.Sum(x => x.totalAmount), 2)));
                            rowHeader1.CreateCell(3).SetCellValue(String.Format("{0:n}", decimal.Round(arrears.Sum(x => x.amountPaid), 2)));
                            cc++;
                        }

                        var penalty = dnPenalty.Where(x => x.wardName == rmp.Key && x.category == tm.Key);
                        if (penalty.Count() > 0)
                        {

                            IRow rowHeader1 = sheet1.CreateRow(rowIndex++);
                            rowHeader1.CreateCell(0).SetCellValue(cc + 1);
                            rowHeader1.CreateCell(1).SetCellValue("Penalty");
                            rowHeader1.CreateCell(2).SetCellValue(String.Format("{0:n}", decimal.Round(penalty.Sum(x => x.totalAmount), 2)));
                            rowHeader1.CreateCell(3).SetCellValue(String.Format("{0:n}", decimal.Round(penalty.Sum(x => x.amountPaid), 2)));
                            cc++;
                        }
                    }
                }

                MemoryStream memo = new MemoryStream();
                workbook.Write(memo);
                return memo.ToArray();
            });
        }

        public async Task<byte[]> TaxpayerWithOutDemandNotice(TaxpayerExtension2[] taxpayers, int billingYear)
        {
            return await Task.Run(() =>
            {
                IWorkbook workbook = new XSSFWorkbook();
                if (taxpayers.Length < 1)
                {
                    return null;
                }

                foreach (var tm in taxpayers.GroupBy(x => x.StreetName))
                {
                    string valu = Regex.Replace(tm.Key, "[!@#$%^&*()_+|\\}{[]';:/.,<>?]+", "");
                    valu = valu.Replace("[", "");
                    valu = valu.Replace("]", "");
                    valu = valu.Replace("/", "");
                    valu = valu.Replace("\\", "");
                    valu = valu.Replace("-", "");
                    ISheet sheet1;
                    var r = workbook.GetSheet(valu);
                    if (r != null)
                    {
                        sheet1 = workbook.CreateSheet(valu + Guid.NewGuid().ToString().Substring(0, 6));
                    }
                    else
                    {
                        sheet1 = workbook.CreateSheet(valu);
                    }

                    #region sub heading
                    sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 4));
                    sheet1.AddMergedRegion(new CellRangeAddress(1, 1, 0, 4));
                    //sheet1.AddMergedRegion(new CellRangeAddress(2, 2, 0, 4));
                    //sheet1.AddMergedRegion(new CellRangeAddress(3, 3, 0, 4));

                    var rowIndex = 0;
                    IRow rowDomain = sheet1.CreateRow(rowIndex);
                    rowDomain.Height = 400;
                    ICell cellDomain = rowDomain.CreateCell(0);
                    cellDomain.SetCellValue(tm.Key.ToUpper());
                    CellStyleHeader(cellDomain, workbook, 13);
                    rowIndex++;

                    IRow rowLcda = sheet1.CreateRow(rowIndex);
                    rowLcda.Height = 300;
                    ICell cellLcda = rowLcda.CreateCell(0);
                    cellLcda.SetCellValue($"Unbilled taxpayer for the year {billingYear}");
                    CellStyleHeader(cellLcda, workbook, 10);
                    rowIndex++;
                    #endregion

                    #region header
                    IRow rowHeader = sheet1.CreateRow(rowIndex++);
                    rowHeader.CreateCell(0).SetCellValue("SN");
                    rowHeader.CreateCell(1).SetCellValue("Company Name");
                    rowHeader.CreateCell(2).SetCellValue("Taxpayer Name");
                    rowHeader.CreateCell(3).SetCellValue("Ward Name");
                    rowHeader.CreateCell(4).SetCellValue("Street");
                    #endregion

                    int count = 1;
                    foreach (var tmm in tm.ToArray())
                    {
                        IRow rowHeader1 = sheet1.CreateRow(rowIndex++);
                        rowHeader1.CreateCell(0).SetCellValue(count++);
                        rowHeader1.CreateCell(1).SetCellValue(tmm.companyName);
                        rowHeader1.CreateCell(2).SetCellValue($"{tmm.lastname} {tmm.firstname} {tmm.surname}");
                        rowHeader1.CreateCell(3).SetCellValue(tmm.WardName);
                        rowHeader1.CreateCell(4).SetCellValue($"{tmm.AddressNumber} {tmm.StreetName}");
                    }
                }

                MemoryStream memo = new MemoryStream();
                workbook.Write(memo);
                return memo.ToArray();
            });
        }
    }
}

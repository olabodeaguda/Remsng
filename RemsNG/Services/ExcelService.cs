using Microsoft.Extensions.Logging;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ExcelService : IExcelService
    {
        ILogger logger;
        public ExcelService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger("Excel Service");
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

                    #endregion

                    string[] items = rptLst.Select(x => x.itemDescription).Distinct().ToArray();

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

                        foreach (var tm in wards)
                        {
                            decimal wardPaid = rptLst.Where(x => x.wardName == tm && x.itemDescription == items[i]).Sum(x => x.amountPaid);
                            rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(wardPaid, 2)));
                        }

                        decimal currentAmountPaid = rptLst.Where(x => x.itemDescription == items[i]).Sum(x => x.amountPaid);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(currentAmountPaid, 2)));

                        decimal previousAmount = previousYearList.Where(x => x.itemDescription == items[i]).Sum(x => x.amountPaid);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(previousAmount, 2)));

                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round((previousAmount + currentAmountPaid), 2)));
                        #endregion
                    }

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
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(amountPaid, 2)));

                        decimal totalAmount = eachGrp.Sum(x => x.itemAmount);
                        rowbody.CreateCell(colCount++).SetCellValue(String.Format("{0:n}", decimal.Round(totalAmount - amountPaid, 2)));

                        #endregion
                    }

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

    }
}

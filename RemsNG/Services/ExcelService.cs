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
        public async Task<bool> WriteReportSummary(List<ItemReportSummaryModel> rptLst, string path)
        {
            if (rptLst.Count < 1)
            {
                return false;
            }
            await Task.Run(() =>
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet1 = workbook.CreateSheet("Report Summary");

                    sheet1.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));
                    var rowIndex = 0;
                    IRow row = sheet1.CreateRow(rowIndex);
                    row.Height = 30 * 80;
                    row.CreateCell(0).SetCellValue("this is content");
                    sheet1.AutoSizeColumn(0);
                    rowIndex++;
                }



            });

            return true;
        }
    }
}

using RemsNG.Dao;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ReportService : IReportService
    {
        private readonly ReportDao reportDao;
        public ReportService(RemsDbContext _db)
        {
            reportDao = new ReportDao(_db);
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            return await reportDao.ByDate(startDate, endDate);
        }

        public async Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst,
            List<ItemReportSummaryModel> previousYearList)
        {
            if (rptLst.Count < 1)
            {
                return null;
            }

            return await Task.Run(() =>
            {
                string[] wards = rptLst.Select(x => x.wardName).Distinct().ToArray();
                int rowCount = wards.Length + 6;

                string html = "<table class='table table-bordered table - hover'><thead>";

                html = html + $"<tr> " +
                $"<td>SN</td>" +
                $"<td>DETAILS OF REVENUE</td>" +
                $"<td>ITEM CODE</td>" +
                $"<td>APPROVED ESTIMATE</td>";

                foreach (var tm in wards)
                {
                    html = html + $"<td>{tm}</td>";
                }

                html = html + $"<td>CURRENT WEEK</td>";
                html = html + $"<td>PREVIOUS AMOUNT</td>";
                html = html + $"<td>TO DATE</td>";
                html = html + $"</tr>";

                html = html + "</thead>";

                html = html + "<tbody>";

                string[] items = rptLst.Select(x => x.itemDescription).Distinct().ToArray();

                for (int i = 0; i < items.Length; i++)
                {
                    #region body
                    html = html + "<tr>";
                    html = html + $"<td>{(i + 1)}</td>";
                    html = html + $"<td>{items[i]}</td>";
                    var itemCode = rptLst.Where(x => x.itemDescription == items[i]).FirstOrDefault();
                    html = html + $"<td>{(itemCode == null ? "Empty" : itemCode.itemCode)}</td>";

                    decimal estmatAmount = rptLst.Where(x => x.itemDescription == items[i]).Sum(x => x.itemAmount);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round(estmatAmount, 2))}</td>";
                    foreach (var tm in wards)
                    {
                        decimal wardPaid = rptLst.Where(x => x.wardName == tm && x.itemDescription == items[i]).Sum(x => x.amountPaid);
                        html = html + $"<td>{String.Format("{0:n}", decimal.Round(wardPaid, 2))}</td>";
                    }

                    decimal currentAmountPaid = rptLst.Where(x => x.itemDescription == items[i]).Sum(x => x.amountPaid);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round(currentAmountPaid, 2))}</td>";

                    decimal previousAmount = previousYearList.Where(x => x.itemDescription == items[i]).Sum(x => x.amountPaid);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round(previousAmount, 2))}</td>";

                    html = html + $"<td>{String.Format("{0:n}", decimal.Round((previousAmount + currentAmountPaid), 2))}</td>";
                    html = html + "</tr>";
                    #endregion
                }

                html = html + "</tbody>";
                html = html + "</table>";
                return html;
            });
        }

        public async Task<List<ChartReport>> ReportByCurrentYear()
        {
            return await reportDao.ReportByYear();
        }

        public async void ReportSummaryByDate(DateTime startDate, DateTime endDate)
        {
            List<ItemReportSummaryModel> datas = await ByDate(startDate, endDate);

        }
    }
}

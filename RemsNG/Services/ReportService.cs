using RemsNG.Dao;
using RemsNG.Models;
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
        private readonly DemandNoticePenaltyRepository dnPenaltyDao;
        private readonly DemandNoticeArrearRepository dnArrearsDao;
        private readonly DemandNoticeItemRepository dnitemDao;
        private readonly ReportRepository reportDao;
        public ReportService(RemsDbContext _db)
        {
            reportDao = new ReportRepository(_db);
            dnitemDao = new DemandNoticeItemRepository(_db);
            dnArrearsDao = new DemandNoticeArrearRepository(_db);
            dnPenaltyDao = new DemandNoticePenaltyRepository(_db);
        }

        public async Task<List<ItemReportSummaryModel>> ByDate(DateTime startDate, DateTime endDate)
        {
            return await reportDao.ByDate(startDate, endDate);
        }

        public async Task<List<ItemReportSummaryModel>> ByDate2(DateTime startDate, DateTime endDate)
        {
            return await reportDao.ByDate2(startDate, endDate);
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
                html = html + $"<td>CUMULATIVE</td>";
                html = html + $"</tr>";

                html = html + "</thead>";

                html = html + "<tbody>";

                string[] items = rptLst.Select(x => x.itemDescription).Distinct().ToArray();

                decimal cumulative = 0;
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

                    cumulative = cumulative + decimal.Round((previousAmount + currentAmountPaid), 2);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round((previousAmount + currentAmountPaid), 2))}</td>";
                    html = html + $"<td>{String.Format("{0:n}", cumulative)}</td>";
                    html = html + "</tr>";
                    #endregion
                }

                html = html + "</tbody>";
                html = html + "</table>";
                return html;
            });
        }

        public async Task<string> HtmlByDate(List<ItemReportSummaryModel> rptLst)
        {
            if (rptLst.Count < 1)
            {
                return null;
            }

            return await Task.Run(() =>
            {
                string[] items = rptLst.Select(x => x.itemDescription).Distinct().OrderBy(x => x).ToArray();

                string html = "<table class='table table-bordered table - hover'><thead>";

                html = html + $"<tr> " +
                $"<td>SN</td>" +
                $"<td>TAXPAYER'S</td>" +
                $"<td>WARD</td>" +
                $"<td>BILLING NO</td>";

                foreach (var tm in items)
                {
                    html = html + $"<td>{tm}</td>";
                }

                html = html + $"<td>ARREARS</td>";
                html = html + $"<td>PENALTIES</td>";
                html = html + $"<td>AMOUNT PAID</td>";
                html = html + $"<td>AMOUNT OWE</td>";
                html = html + $"</tr>";
                html = html + "</thead>";

                html = html + "<tbody>";

                var allitems = rptLst.GroupBy(x => x.billingNo);
                int rowCount = 1;
                foreach (var eachGrp in allitems)
                {
                    var firstTaxpayer = eachGrp.FirstOrDefault();

                    #region body
                    html = html + "<tr>";
                    html = html + $"<td>{rowCount++}</td>";
                    html = html + $"<td>{firstTaxpayer.taxpayersName}</td>";
                    html = html + $"<td>{firstTaxpayer.wardName}</td>";
                    html = html + $"<td>{eachGrp.Key}</td>";

                    var taxPItems = eachGrp.Where(x => x.category == "ITEMS");
                    foreach (var tm in items)
                    {
                        decimal h = taxPItems.Where(x => x.itemDescription == tm).Sum(x => x.itemAmount);
                        html = html + $"<td>{String.Format("{0:n}", decimal.Round(h, 2))}</td>";
                    }

                    decimal arrears = eachGrp.Where(x => x.category == "ARREARS").Sum(x => x.itemAmount);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round(arrears, 2))}</td>";

                    decimal penalties = eachGrp.Where(x => x.category == "PENALTY").Sum(x => x.itemAmount);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round(penalties, 2))}</td>";

                    decimal amountPaid = eachGrp.Sum(x => x.amountPaid);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round(amountPaid, 2))}</td>";

                    decimal totalAmount = eachGrp.Sum(x => x.itemAmount);
                    html = html + $"<td>{String.Format("{0:n}", decimal.Round(totalAmount - amountPaid, 2))}</td>";

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

        public async Task<List<DemandNoticeItemExt>> ReportitemsByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnitemDao.ReportByCategory(startDate, endDate);
        }
        public async Task<List<DemandNoticeArrearsExt>> ReportArrearsByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnArrearsDao.ReportByCategory(startDate, endDate);
        }
        public async Task<List<DemandNoticeItemPenaltyExt>> ReportPenaltyByCategory(DateTime startDate, DateTime endDate)
        {
            return await dnPenaltyDao.ReportByCategory(startDate, endDate);
        }
    }
}

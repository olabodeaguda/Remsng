using RemsNG.Common.Models;
using System;
using System.Linq;

namespace RemsNG.Common.Utilities
{
    public class DemandNoticeComponents
    {
        public static string HtmlBuildItems1(DemandNoticeReportModel dnrm)
        {
            string htmlmarkup = string.Empty;
            int count = 1;
            foreach (var tm in dnrm.items)
            {
                htmlmarkup = htmlmarkup + $"<tr style = 'border: 1px solid black;'>";
                htmlmarkup = htmlmarkup + $"<td style = 'border-width:0px 1px 0px 1px'> {count++} </td>";
                htmlmarkup = htmlmarkup + $"<td style = 'text -align:left;'> {tm.itemTitle} </td>";
                htmlmarkup = htmlmarkup + $"<td>{ String.Format("{0:n}", decimal.Round(tm.itemAmount, 2))}</ td ></tr>";
            }

            return htmlmarkup;
        }

        public static string HtmlBuildItems(DemandNoticeReportModel dnrm)
        {
            string htmlmarkup = string.Empty;
            int count = 1;
            foreach (var tm in dnrm.items)
            {
                htmlmarkup = htmlmarkup + $"<tr>";
                htmlmarkup = htmlmarkup + $"<td width=\"90\" colspan=\"1\"> {count++} </td>";
                htmlmarkup = htmlmarkup + $"<td width=\"45%\" colspan=\"8\"> {tm.itemTitle} </td>";
                htmlmarkup = htmlmarkup + $"<td width=\"20%\" colspan=\"2\"></td><td width=\"30%\" colspan=\"2\"  text-align:'center'>{ String.Format("{0:n}", decimal.Round(tm.itemAmount, 2))}</ td ></tr>";
            }

            return htmlmarkup;
        }

        public static string HtmlBuildBanks(DemandNoticeReportModel dnrm)
        {
            string htmlmarkup = string.Empty;
            foreach (var tm in dnrm.banks)
            {
                htmlmarkup = htmlmarkup + $"<li>{tm.bankName}:{tm.bankAccount}</li>";
            }

            return htmlmarkup;
        }
        public static string HtmlBuildBanks(DemandNoticeReportModel dnrm, BankCategory bankCategory, TaxpayerCategoryModel taxpayerCategory)
        {
            string htmlmarkup = string.Empty;
            if (bankCategory.CatgoryName.ToLower() == taxpayerCategory.TaxpayerCategoryName.ToLower())
            {
                var t = dnrm.banks.FirstOrDefault(x => x.bankId == bankCategory.BankId);
                if (t != null)
                {
                    htmlmarkup = htmlmarkup + $"<li><b>{t.bankName}:  {t.bankAccount}</b></li>";
                }
            }
            else
            {
                var r = dnrm.banks.Where(x => x.bankId != bankCategory.BankId);
                foreach (var tm in r)
                {
                    htmlmarkup = htmlmarkup + $"<li><b>{tm.bankName}:{tm.bankAccount}</b></li>";
                }
            }

            return htmlmarkup;
        }
    }
}

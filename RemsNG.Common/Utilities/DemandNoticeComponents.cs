using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
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
                htmlmarkup = htmlmarkup + $"<td colspan=\"1\"> {count++} </td>";
                htmlmarkup = htmlmarkup + $"<td colspan=\"7\" align=\"left\"> {tm.itemTitle} </td>";
                htmlmarkup = htmlmarkup + $"<td colspan=\"2\"  align=\"center\">{ String.Format("{0:n}", decimal.Round(tm.itemAmount, 2))}</td></tr>";
            }

            return htmlmarkup;
        }

        public static string HtmlBuildBanks(DemandNoticeReportModel dnrm)
        {
            string[] str = new string[] { "uba", "Polaris Bank" };
            string htmlmarkup = string.Empty;
            foreach (var tm in dnrm.banks)
            {
                if (tm.bankName.ToLower() == "united bank for africa plc" || tm.bankName.ToLower() == "polaris bank")
                {
                    continue;
                }
                htmlmarkup = htmlmarkup + $"<li>{tm.bankName}:{tm.bankAccount}</li>";
            }

            return htmlmarkup;
        }
        public static string HtmlBuildBanks(DemandNoticeReportModel dnrm, BankCategory bankCategory, TaxpayerCategoryModel taxpayerCategory)
        {
            List<BankLcdaModel> lst = new List<BankLcdaModel>();
            Dictionary<string, string[]> config = ConvertCatgeory(bankCategory.CatgoryAccountNumbers);

            string htmlmarkup = string.Empty;
            if (config.ContainsKey(taxpayerCategory.TaxpayerCategoryName.ToLower()))
            {
                var bk = config[taxpayerCategory.TaxpayerCategoryName.ToLower()];
                lst = dnrm.banks.Where(x => bk.Any(p => p == x.bankAccount)).ToList();
            }
            else
            {
                string[] deft = bankCategory.DefaultAccountNumber.Split(new char[] { ',' });
                lst = dnrm.banks.Where(x => deft.Any(p => p == x.bankAccount)).ToList();
            }

            foreach (var tm in lst)
            {
                htmlmarkup = htmlmarkup + $"<li><b>{tm.bankName}:{tm.bankAccount}</b></li>";
            }

            return htmlmarkup;
        }

        private static Dictionary<string, string[]> ConvertCatgeory(string value)
        {
            string[] arr = value.Split(new char[] { ';' });

            Dictionary<string, string[]> s = new Dictionary<string, string[]>();
            foreach (var tm in arr)
            {
                string[] d = tm.Split(new char[] { ':' });
                if (d.Length <= 1)
                    continue;
                string key = d[0].ToLower();
                string[] accountNumbers = d[1].Split(new char[] { ',' });
                s.Add(key, accountNumbers);
            }

            return s;
        }
    }
}

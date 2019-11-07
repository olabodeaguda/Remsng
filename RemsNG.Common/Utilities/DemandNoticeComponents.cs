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

            string htmlmarkup = string.Empty;
            if (bankCategory != null && taxpayerCategory != null)
            {
                Guid[] guids = bankCategory.BankIds.Split(new char[] { ';' })
                    .Select(x => Guid.Parse(x))
                    .ToArray();
                var re = dnrm.banks.Where(x => guids.Any(s => s == x.bankId)).ToList();// x.bankId == bankCategory.BankId).ToList();
                if (bankCategory.CatgoryName.ToLower() == taxpayerCategory.TaxpayerCategoryName.ToLower())
                {
                    if (re.Count > 0)
                    {
                        lst = re;
                    }
                }
                else
                {
                    lst = dnrm.banks.Where(s => !guids.Any(x => x == s.bankId)).ToList();
                }
            }

            if (lst.Count <= 0)
            {
                lst = dnrm.banks;
            }

            if (!string.IsNullOrEmpty(bankCategory.ExceludeBanks))
            {
                var exemptBanks = bankCategory.ExceludeBanks.Split(new char[] { ';' })
                            .Select(x => Guid.Parse(x))
                            .ToList();

                lst = lst.Where(x => !exemptBanks.Any(s => s == x.bankId)).ToList();
            }

            foreach (var tm in lst)
            {
                htmlmarkup = htmlmarkup + $"<li><b>{tm.bankName}:{tm.bankAccount}</b></li>";
            }

            return htmlmarkup;
        }
    }
}

using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class DemandNoticeComponents
    {
        public static string HtmlBuildItems(DemandNoticeReportModel dnrm)
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

        public static string HtmlBuildBanks(DemandNoticeReportModel dnrm)
        {
            string htmlmarkup = string.Empty;
            foreach (var tm in dnrm.banks)
            {
                htmlmarkup = htmlmarkup + $"<li>{tm.bankName}:{tm.bankAccount}</li>";
            }

            return htmlmarkup;
        }
    }
}

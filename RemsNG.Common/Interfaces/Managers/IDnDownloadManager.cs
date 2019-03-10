using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDnDownloadManager
    {
        Task<string> PopulateReportHtml(string htmlContent, string billingno, string rootUrl, string createdBy);
        Task<string> PopulateReportHtmlBase64(string htmlContent, string billingno, string rootUrl, string createdBy);
        Task<string> LcdaTemlate(string billingno);
        Task<string> LcdaTemlateByLcda(Guid lcdaId);
        Task<string> ReceiptTemlate(string billingno);
        Task<string> PopulateReceiptHtml(string htmlContent, string rootUrl,
            string createdBy, DemandNoticePaymentHistoryModel dnph);
    }
}

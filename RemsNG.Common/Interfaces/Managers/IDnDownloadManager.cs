using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDnDownloadManager
    {
        Task<string> PopulateReportHtml(string htmlContent, long billingno, string rootUrl, string createdBy);
        Task<string> PopulateReportHtmlBase64(string htmlContent, long billingno, string rootUrl, string createdBy);
        Task<string> LcdaTemlate(long billingno);
        Task<string> LcdaTemlateByLcda(Guid lcdaId);
        Task<string> ReceiptTemlate(long billingno);
        Task<string> PopulateReceiptHtml(string htmlContent, string rootUrl,
            string createdBy, DemandNoticePaymentHistoryModel dnph);

        Task<byte[]> PopulateReportHtml(string htmlContent, long[] billingno,
            string rootUrl, string createdBy);

        Task<string> PopulateReportHtml1(string htmlContent, long billingno,
            string rootUrl, string createdBy);
    }
}

using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDnDownloadService
    {
        Task<string> PopulateReportHtml(string htmlContent, string billingno, string rootUrl, string createdBy);
        Task<string> LcdaTemlate(string billingno);
        Task<string> LcdaTemlateByLcda(Guid lcdaId);
        Task<string> ReceiptTemlate(string billingno);
        Task<string> PopulateReceiptHtml(string htmlContent, string rootUrl,
            string createdBy, DemandNoticePaymentHistory dnph);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IDnDownloadService
    {
        Task<string> PopulateReportHtml(string htmlContent, string billingno, string rootUrl, string createdBy);
    }
}

using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDnDownloadManager
    {
        Task<string> GenerateReceipt(string createdBy, DemandNoticePaymentHistoryModel dnph);

        Task<byte[]> GenerateDemandNotice(long[] billingno, string createdBy);

        Task<byte[]> GenerateReminder(long[] billingno, string createdBy);

        Task<string> LoadTemplate(string htmlContent, long billingno, string createdBy, TemplateType templateType);
    }
}

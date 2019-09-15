using RemsNG.Common.Models;
using System;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IDnDownloadManager
    {
        Task<byte[]> GenerateReceipt(string createdBy, DemandNoticePaymentHistoryModel dnph);

        Task<byte[]> GenerateDemandNotice(long[] billingno, string createdBy);

        Task<byte[]> GenerateReminder(long[] billingno, string createdBy);

        Task<string> LoadTemplateDemandNotice(string htmlContent, long billingno, string createdBy, TemplateType templateType);
    }
}

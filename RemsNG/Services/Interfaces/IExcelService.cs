using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IExcelService
    {
        Task<bool> WriteReportSummary(List<ItemReportSummaryModel> rptLst, string path);
    }
}

using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IDemandNoticePenaltyRepository
    {
        Task<DemandNoticePenaltyModel> CreatePenalty(DemandNoticePenaltyModel dnp);
        Task<bool> CreatePenalty(DemandNoticePenaltyModel[] models);
        string AddQuery(DemandNoticePenaltyModel dnp);
        Response RunQuery(string query);
        Task<List<DemandNoticePenaltyModel>> ByTaxpayerId(Guid taxpayerId);
        Task<List<DemandNoticePenaltyModel>> ByTaxpayerId(Guid taxpayerId, int billingYr);
        Task<List<DemandNoticePenaltyModel>> ReportByCategory(Guid[] taxpayerIds);
        Task<List<DemandNoticePenaltyModel>> ReportByCategory(DateTime fromDate, DateTime toDate);
        Task<DemandNoticePenaltyModel[]> ByTaxpayerId(Guid[] taxpayerIds);
        Task<bool> UpdatePenaltyStatus(DemandNoticePenaltyModel[] models, string status);
    }
}

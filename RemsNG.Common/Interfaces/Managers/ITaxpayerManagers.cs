﻿using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface ITaxpayerManagers
    {
        Task<Response> Create(TaxPayerModel taxpayer, bool confirmCompany);
        Task<TaxpayerExtensionModel> ById(Guid id);
        Task<List<TaxpayerExtensionModel>> ByStreetId(Guid streetId);
        Task<object> ByStreetId(Guid streetId, PageModel pageModel);
        Task<List<TaxpayerExtensionModel>> ByCompanyId(Guid companyId);
        Task<Response> Update(TaxPayerModel taxpayer);
        Task<List<TaxpayerExtensionModel>> ByLcdaId(Guid lcdaId);
        Task<object> ByLcdaId(Guid lcdaId, PageModel pageModel);
        Task<LcdaModel> getLcda(Guid taxpayerId);
        Task<List<TaxpayerExtensionModel>> Search(Guid lcdaId, string qu);
        Task<List<DemandNoticePaymentHistoryModel>> PaymentHistory(Guid id);
        Task<bool> Delete(Guid taxpayerId);
        Task<List<TaxpayerExtensionModel>> SearchInStreet(Guid streetId, string query);
        Task<TaxpayerExtensionModel2[]> UnBilledTaxpayer(int billingYear);
    }
}
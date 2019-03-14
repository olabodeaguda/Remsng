﻿using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IItemPenaltyManager
    {
        Task<Response> Add(ItemPenaltyModel item);
        Task<Response> Update(ItemPenaltyModel item);
        Task<Response> UpdateStatus(ItemPenaltyModel item);
        Task<object> GetById(Guid id);
        Task<object> ListByItemId(Guid itemId);
        Task<object> ListByItemId(Guid itemId, PageModel pageModel);
        Task<Response> RunTaxpayerPenalty(Guid[] taxpayerIds);
        Task<List<DemandNoticePenaltyModel>> ActivePenalty(Guid taxpayerId);
    }
}
﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IWardService
    {
        Task<List<Ward>> all();
        Task<List<Ward>> ActiveWard();
        Task<bool> Add(Ward ward);
        Task<object> Paginated(Models.PageModel pageModel);
        Task<object> Paginated(Models.PageModel pageModel, Guid lgdaId);
        Task<Ward> GetWard(Guid id);
        Task<List<Ward>> GetWardByLGDAId(Guid lgdaId);
        Task<bool> Update(Ward ward);
        Task<Ward> GetWard(string wardName, Guid lgdaid);
        Task<Domain> GetDomain(Guid wardId);
    }
}
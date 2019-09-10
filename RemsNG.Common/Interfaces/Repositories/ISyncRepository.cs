using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface ISyncRepository
    {
        Task<List<SyncDataModel>> Get();
        Task<List<SyncDataModel>> GetApprovalUpdate();
        Task<bool> UpdateSyncStatus(Guid[] ids);
        Task<bool> UpdatePaymentStatus(List<SyncDataModel> approveBills);

    }
}

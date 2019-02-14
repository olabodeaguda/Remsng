using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Managers
{
    public interface IContactManagers
    {
        Task<bool> Add(ContactDetailModel contactDetail);
        Task<bool> Update(ContactDetailModel contactDetail);
        Task<List<ContactDetailModel>> ByOwnerId(Guid ownerId);
        Task<ContactDetailModel> ByContactValue(ContactDetailModel contactDetail);
        Task<ContactDetailModel> ById(Guid id);
        Task<bool> Remove(ContactDetailModel contactDetail);
    }
}

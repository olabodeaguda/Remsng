using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IContactService
    {
        Task<bool> Add(ContactDetail contactDetail);
        Task<bool> Update(ContactDetail contactDetail);
        Task<List<ContactDetail>> ByOwnerId(Guid ownerId);
        Task<ContactDetail> ByContactValue(ContactDetail contactDetail);
        Task<ContactDetail> ById(Guid id);
        Task<bool> Remove(ContactDetail contactDetail);
        void sample(ref int x);
    }
}

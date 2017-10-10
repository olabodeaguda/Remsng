using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IContactService
    {
        Task<bool> Add(ContactDetail contactDetail);
        Task<bool> Update(ContactDetail contactDetail);
        Task<List<ContactDetail>> ByOwnerId(Guid ownerId);
        Task<ContactDetail> ByContactValue(ContactDetail contactDetail);
        Task<ContactDetail> ById(Guid id);
        Task<bool> Remove(ContactDetail contactDetail);
    }
}

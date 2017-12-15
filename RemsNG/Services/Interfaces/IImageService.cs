using RemsNG.Models;
using RemsNG.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services.Interfaces
{
    public interface IImageService
    {
        Task<Response> Add(Images images);
        Task<List<Images>> ByOwnerId(Guid ownerId);
        Task<Images> ByOwnerId(Guid ownerId, string fileType);
        Task<string> ImageNameByOwnerIdAsync(Guid ownerId, string fileType);
    }
}

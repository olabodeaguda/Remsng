using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Repositories
{
    public interface IImageRepository
    {
        Task<Response> Add(ImagesModel images);
        Task<List<ImagesModel>> ByOwnerId(Guid ownerId);
        Task<ImagesModel> ByOwnerId(Guid ownerId, string fileType);
    }
}

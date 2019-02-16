using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ImageManagers : IImageManagers
    {
        private readonly ImageRepository imageDao;
        public ImageManagers(RemsDbContext _db)
        {
            imageDao = new ImageRepository(_db);
        }

        public async Task<Response> Add(ImagesModel images)
        {
            return await imageDao.Add(images);
        }

        public async Task<List<ImagesModel>> ByOwnerId(Guid ownerId)
        {
            return await imageDao.ByOwnerId(ownerId);
        }

        public async Task<ImagesModel> ByOwnerId(Guid ownerId, string fileType)
        {
            return await imageDao.ByOwnerId(ownerId, fileType);
        }

        public async Task<string> ImageNameByOwnerIdAsync(Guid ownerId, string fileType)
        {
            ImagesModel images = await ByOwnerId(ownerId, fileType);
            if (images != null)
            {
                return images.ImgFilename;
            }

            return string.Empty;
        }

        Task<List<ImagesModel>> IImageManagers.ByOwnerId(Guid ownerId)
        {
            throw new NotImplementedException();
        }

        Task<ImagesModel> IImageManagers.ByOwnerId(Guid ownerId, string fileType)
        {
            throw new NotImplementedException();
        }
    }
}

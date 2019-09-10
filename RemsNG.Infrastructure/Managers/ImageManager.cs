using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Interfaces.Managers;
using RemsNG.Common.Interfaces.Repositories;
using RemsNG.Common.Models;
using RemsNG.Data.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Infrastructure.Managers
{
    public class ImageManager : IImageManager
    {
        private readonly IImageRepository imageDao;
        public ImageManager(IImageRepository imageRepository)
        {
            imageDao = imageRepository;
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

        Task<List<ImagesModel>> IImageManager.ByOwnerId(Guid ownerId)
        {
            throw new NotImplementedException();
        }

        Task<ImagesModel> IImageManager.ByOwnerId(Guid ownerId, string fileType)
        {
            throw new NotImplementedException();
        }
    }
}

using RemsNG.Dao;
using RemsNG.Models;
using RemsNG.ORM;
using RemsNG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Services
{
    public class ImageService: IImageService
    {
        private readonly ImageDao imageDao;
        public ImageService(RemsDbContext _db)
        {
            imageDao = new ImageDao(_db);
        }
        public async Task<Response> Add(Images images)
        {
            return await imageDao.Add(images);
        }

        public async Task<List<Images>> ByOwnerId(Guid ownerId)
        {
            return await imageDao.ByOwnerId(ownerId);
        }

        public async Task<Images> ByOwnerId(Guid ownerId, string fileType)
        {
            return await imageDao.ByOwnerId(ownerId, fileType);
        }

        public async Task<string> ImageNameByOwnerIdAsync(Guid ownerId, string fileType)
        {
            Images images = await ByOwnerId(ownerId, fileType);
            if (images != null)
            {
                return images.imgFilename;
            }

            return string.Empty;
        }
        
    }
}

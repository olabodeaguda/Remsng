﻿using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemsNG.Common.Interfaces.Services
{
    public interface IImageService
    {
        Task<Response> Add(ImagesModel images);
        Task<List<ImagesModel>> ByOwnerId(Guid ownerId);
        Task<ImagesModel> ByOwnerId(Guid ownerId, string fileType);
        Task<string> ImageNameByOwnerIdAsync(Guid ownerId, string fileType);
    }
}

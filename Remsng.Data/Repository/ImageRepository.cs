using Microsoft.EntityFrameworkCore;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using RemsNG.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class ImageRepository : AbstractRepository
    {
        public ImageRepository(DbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(ImagesModel images)
        {
            //DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addImages @p0, @p1, @p2, @p3", new object[] {
            //   images.ImgFilename,
            //   images.OwnerId,
            //   images.ImgType,
            //   images.CreatedBy
            //}).FirstOrDefaultAsync();

            db.Set<Images>()
                .Add(new Images
                {
                    CreatedBy = images.CreatedBy,
                    DateCreated = DateTime.Now,
                    Id = Guid.NewGuid(),
                    ImgFilename = images.ImgFilename,
                    ImgType = images.ImgType,
                    OwnerId = images.OwnerId
                });
            await db.SaveChangesAsync();
            return new Response()
            {
                code = MsgCode_Enum.SUCCESS,
                description = "Images has been save successfuly"
            };
        }

        public async Task<List<ImagesModel>> ByOwnerId(Guid ownerId)
        {
            var r = await db.Set<Images>()
                .Where(x => x.OwnerId == ownerId).ToListAsync();
            return r.Select(x => new ImagesModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated.Value,
                ImgFilename = x.ImgFilename,
                Id = x.Id,
                ImgType = x.ImgType,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OwnerId = x.OwnerId
            }).ToList();
        }

        public async Task<ImagesModel> ByOwnerId(Guid ownerId, string fileType)
        {
            var x = await db.Set<Images>()
                .Where(p => p.OwnerId == ownerId && p.ImgType == fileType).FirstOrDefaultAsync();
            if (x == null)
            {
                return null;
            }
            return new ImagesModel()
            {
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated.Value,
                ImgFilename = x.ImgFilename,
                Id = x.Id,
                ImgType = x.ImgType,
                Lastmodifiedby = x.Lastmodifiedby,
                LastModifiedDate = x.LastModifiedDate,
                OwnerId = x.OwnerId
            };
        }
    }
}

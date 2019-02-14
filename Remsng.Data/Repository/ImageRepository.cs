using Microsoft.EntityFrameworkCore;
using Remsng.Data;
using RemsNG.Common.Models;
using RemsNG.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Data.Repository
{
    public class ImageRepository : AbstractRepository
    {
        public ImageRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(ImagesModel images)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addImages @p0, @p1, @p2, @p3", new object[] {
               images.ImgFilename,
               images.OwnerId,
               images.ImgType,
               images.CreatedBy
            }).FirstOrDefaultAsync();

            if (dbResponse.success)
            {
                return new Response()
                {
                    code = MsgCode_Enum.SUCCESS,
                    description = dbResponse.msg
                };
            }
            else
            {
                return new Response()
                {
                    code = MsgCode_Enum.FAIL,
                    description = dbResponse.msg
                };
            }
        }

        public async Task<List<ImagesModel>> ByOwnerId(Guid ownerId)
        {
            var r = await db.Imagess.Where(x => x.OwnerId == ownerId).ToListAsync();
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
            var x = await db.Imagess.Where(p => p.OwnerId == ownerId && p.ImgType == fileType).FirstOrDefaultAsync();
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

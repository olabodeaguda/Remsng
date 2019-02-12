using Microsoft.EntityFrameworkCore;
using Remsng.Data;
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
        public ImageRepository(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(Images images)
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

        public async Task<List<Images>> ByOwnerId(Guid ownerId)
        {
            return await db.Imagess.Where(x => x.OwnerId == ownerId).ToListAsync();
        }

        public async Task<Images> ByOwnerId(Guid ownerId, string fileType)
        {
            return await db.Imagess.Where(x => x.OwnerId == ownerId && x.ImgType == fileType).FirstOrDefaultAsync();
        }
    }
}

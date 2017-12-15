using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemsNG.ORM;
using RemsNG.Models;
using Microsoft.EntityFrameworkCore;
using RemsNG.Utilities;

namespace RemsNG.Dao
{
    public class ImageDao : AbstractDao
    {
        public ImageDao(RemsDbContext _db) : base(_db)
        {
        }

        public async Task<Response> Add(Images images)
        {
            DbResponse dbResponse = await db.Set<DbResponse>().FromSql("sp_addImages @p0, @p1, @p2, @p3", new object[] {
               images.imgFilename,
               images.ownerId,
               images.imgType,
               images.createdBy
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
            return await db.Imagess.Where(x => x.ownerId == ownerId).ToListAsync();
        }

        public async Task<Images> ByOwnerId(Guid ownerId, string fileType)
        {
            return await db.Imagess.Where(x => x.ownerId == ownerId && x.imgType == fileType).FirstOrDefaultAsync();
        }


    }
}

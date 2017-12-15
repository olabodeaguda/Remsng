using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class ImgUtils
    {
        public bool WriteFile(String path, string imgBase64)
        {
            bool result = false;
            byte[] img = getByte(imgBase64);   
            using (var stream = new FileStream(path, FileMode.Create))
            {
                stream.Write(img, 0, img.Length);
                stream.Flush();
                result = true;
            }
            return result;
        }

        public byte[] getByte(string imgBase64)
        {
            string bse64 = imgBase64.Substring(imgBase64.IndexOf(',') + 1);
            byte[] byteImg = Convert.FromBase64String(bse64);
            return byteImg;
        }
    }
}

using System;

namespace RemsNG.Common.Models
{
    public class ImagesModel
    {
        public Guid Id { get; set; }
        public string ImgFilename { get; set; }
        public Guid OwnerId { get; set; }
        public string ImgType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

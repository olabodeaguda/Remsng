using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Data.Entities
{
    [Table("tbl_images")]
    public partial class Images
    {
        public Guid Id { get; set; }
        public string ImgFilename { get; set; }
        public Guid OwnerId { get; set; }
        public string ImgType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

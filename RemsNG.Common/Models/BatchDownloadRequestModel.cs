using System;

namespace RemsNG.Common.Models
{
    public class BatchDownloadRequestModel
    {
        public Guid Id { get; set; }
        public string BatchNo { get; set; }
        public string RequestStatus { get; set; }
        public Guid? LcdaId { get; set; }
        public string BatchFileName { get; set; }
        public string Createdby { get; set; }
        public DateTime DateCreated { get; set; }
        public string Lastmodifiedby { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

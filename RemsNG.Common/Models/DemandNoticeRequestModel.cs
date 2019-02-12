using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemsNG.Common.Models
{
    public class DemandNoticeRequestModel
    {
        public Nullable<Guid> lcdaId { get; set; }
        public Nullable<Guid> wardId { get; set; }
        public Nullable<Guid> streetId { get; set; }
        public string searchByName { get; set; }
        public int dateYear { get; set; }
        public string createdBy { get; set; }

        public bool CloseOldData { get; set; }
        [NotMapped]
        public string streetName { get; set; }

        [NotMapped]
        public string wardName { get; set; }
        public bool RunArrears { get; set; }
        public bool isUnbilled { get; set; }

        public bool RunPenalty { get; set; } = false;
        public int RunArrearsCategory { get; set; }
    }
}


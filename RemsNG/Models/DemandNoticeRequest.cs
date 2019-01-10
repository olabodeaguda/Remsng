using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Models
{
    public class DemandNoticeRequest
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
    }
}


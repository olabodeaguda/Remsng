using System;

namespace RemsNG.Common.Models
{
    public class DemandNoticeRequestModel
    {
        public Guid lcdaId { get; set; }

        public Guid wardId { get; set; }
        public Guid streetId { get; set; }
        public string searchByName { get; set; }
        public int dateYear { get; set; }

        public string createdBy { get; set; }


        public string streetName { get; set; }
        public string wardName { get; set; }


        public bool RunArrears { get; set; }
        public bool isUnbilled { get; set; }
        public bool RunPenalty { get; set; } = false;
        public int RunArrearsCategory { get; set; }
        public int Period { get; set; }
        public bool useSingleBill { get; set; }

        public Guid[] TaxpayerIds { get; set; } = { };

        public string LcdaAddress { get; set; }
        public string LcdaState { get; set; }
        public string TreasurerMobile { get; set; }
    }
}


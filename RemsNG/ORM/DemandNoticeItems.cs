﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.ORM
{
    public class DemandNoticeItems: AbstractModel
    {
        public Guid id { get; set; }
        public Guid dn_taxpayersDetailsId { get; set; }
        public Guid taxpayerId { get; set; }
        public Guid itemId { get; set; }
        public string itemName { get; set; }
        public decimal itemAmount { get; set; }
        public string itemStatus { get; set; }
    }
}

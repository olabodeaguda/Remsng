using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public enum DemandNoticeStatus
    {
        ACTIVE,
        DEACTIVE,
        CANCELED,
        PENDING,
        SUBMITTED,
        PART_PAYMENT,
        COMPLETED,
        COMPLETED_WITH_LEFTOVER,
        MOVE_TO_ARREARS,
        PAID,
        ERROR,
        CLOSED
    }
}

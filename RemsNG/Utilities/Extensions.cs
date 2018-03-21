using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public static class Extensions
    {
        // 30 sept, april june and nov
        // other 31 days
        public static int DaysInAMonth(this DateTime dateTime)
        {
            List<int> monthsWith30Days = new List<int>()
            {
                4,6,9,11
            };

            if (monthsWith30Days.Contains(dateTime.Month))
            {
                return 30;
            }
            return 31;
        }
    }
}

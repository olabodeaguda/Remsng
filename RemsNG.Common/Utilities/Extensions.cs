using System;
using System.Collections.Generic;
using System.Text;

namespace RemsNG.Common.Utilities
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

        public static string FormatString(this string[] arr)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < arr.Length; i++)
            {
                builder.AppendFormat("'{0}'", arr[i]);
                if (i < (arr.Length - 1))
                {
                    builder.Append(",");
                }
            }
            return builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class CommonList
    {
        public static List<string> StatusLst
        {
            get
            {
                return Enum.GetNames(typeof(UserStatus)).Cast<string>().ToList();
            }
        }

        public static List<string> DemandNoticeStatusLst
        {
            get
            {
                return Enum.GetNames(typeof(DemandNoticeStatus)).Cast<string>().ToList();
            }
        }

        public static string GetBatchNo()
        {
            string batchno = "";
            char[] str = Guid.NewGuid().ToString().ToCharArray();
            foreach (var tm in str)
            {
                int newct = 0;
                bool ct = int.TryParse(tm.ToString(), out newct);
                if (!ct)
                {
                    continue;
                }
                if (batchno.Length == 7)
                {
                    break;
                }
                batchno += newct.ToString();
            }

            return batchno;
        }

        public static List<string> CurrentDurations(DateTime dateDuration)
        {
            List<string> duration = new List<string>();
            DateTime currentDate = DateTime.Now;
            
            if (currentDate.CompareTo(dateDuration.AddDays(1))>= 1)
            {
                duration.Add(DurationEnum.DAILY.ToString());
            }
            if(currentDate.CompareTo(dateDuration.AddDays(7)) >= 1)
            {
                duration.Add(DurationEnum.WEEKELY.ToString());
            }
            if (currentDate.Month > dateDuration.Month)
            {
                duration.Add(DurationEnum.MONTHLY.ToString());
            }
            if (currentDate.Month > 4)
            {
                duration.Add(DurationEnum.QUARTERLY.ToString());
            }
            if (currentDate.Month > 10)
            {
                duration.Add(DurationEnum.YEARLY.ToString());
            }

            return duration;

        }
    }
}

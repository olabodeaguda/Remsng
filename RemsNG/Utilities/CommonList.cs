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
    }
}

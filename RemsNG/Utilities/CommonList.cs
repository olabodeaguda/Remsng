using System;
using System.Collections.Generic;
using System.Linq;
using RemsNG.Utilities;

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

            if (currentDate.CompareTo(dateDuration.AddDays(1)) >= 1)
            {
                duration.Add(DurationEnum.DAILY.ToString());
            }
            if (currentDate.CompareTo(dateDuration.AddDays(7)) >= 1)
            {
                duration.Add(DurationEnum.WEEKLY.ToString());
            }
            if (currentDate.Month > dateDuration.Month)
            {
                duration.Add(DurationEnum.MONTHLY.ToString());
            }

            if (currentDate.Month == 3 || currentDate.Month == 6
                || currentDate.Month == 9 || currentDate.Month == 12)
            {
                int days = dateDuration.DaysInAMonth();
                if (days == dateDuration.Day)
                {
                    duration.Add(DurationEnum.QUARTERLY.ToString());
                }
            }

            if (currentDate.Month > 10)
            {
                duration.Add(DurationEnum.YEARLY.ToString());
            }

            return duration;
        }

        public static string Template(string allowPayment, string allowHeader)
        {
            if (allowPayment == "1" && allowHeader == "1")
            {
                return "dnTemplatePaymentHeader.html";
            }
            else if (allowPayment == "1" && allowHeader == "0")
            {
                return "dnTemplatePayment.html";
            }
            else if (allowPayment == "0" && allowHeader == "1")
            {
                return "dnTemplateHeader.html";
            }
            else if (allowPayment == "0" && allowHeader == "0")
            {
                return "dnTemplate.html";
            }
            else
            {
                return "dnTemplate.html";
            }
        }

        public static string ReceiptTemplate(string allowHeader)
        {

            if (allowHeader == "1")
            {
                return "receiptheader.html";
            }
            else
            {
                return "receipt.html";
            }
        }

        //public static void CopyAllTo<T>(this T source, T target)
        //{
        //    var type = typeof(T);
        //    foreach (var sourceProperty in type.GetProperties())
        //    {
        //        var targetProperty = type.GetProperty(sourceProperty.Name);
        //        targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
        //    }
        //    foreach (var sourceField in type.GetFields())
        //    {
        //        var targetField = type.GetField(sourceField.Name);
        //        targetField.SetValue(target, sourceField.GetValue(source));
        //    }
        //}
    }
}

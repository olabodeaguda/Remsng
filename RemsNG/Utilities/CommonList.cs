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
        
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class MSgCode_Enum
    {
        public static readonly String SUCCESS = "00";
        public static readonly String FAIL = "01";
        public static readonly String DUPLICATE = "03";
        public static readonly String UNKNOWN = "04";
        public static string NOTFOUND = "O5";
        public static readonly string WRONG_CREDENTIALS = "06";
        public static readonly string RETRY = "07";
        public static string UNAUTHORIZED = "08";
        public static string INVALID_TOKEN = "09";
        public static string INVALID_ISSUER = "10";
        public static string TOKEN_EXPIRED = "11";
        public static string VERIFY_EMAIL = "12";
        public static string RESEND_VERIFICATION = "13";
        public static string NO_DOMAIN_ACCESS = "14";
    }
}
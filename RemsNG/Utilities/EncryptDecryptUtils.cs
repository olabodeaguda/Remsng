using Newtonsoft.Json;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemsNG.Utilities
{
    public class EncryptDecryptUtils
    {
        public static String Encrypt(string data)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
        }

        public static string Decrypt(string encryptData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encryptData));
        }

        public static String EncryptSecurityStampModel(SecurityStampModel ssm)
        {
            return Encrypt(JsonConvert.SerializeObject(ssm));
        }

        public static SecurityStampModel DecryptSecurityStampModel(string value)
        {
            string jObj = Decrypt(value);
            return JsonConvert.DeserializeObject<SecurityStampModel>(jObj);
        }
    }
}

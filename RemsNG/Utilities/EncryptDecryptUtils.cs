using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using RemsNG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
            return ToHexString(JsonConvert.SerializeObject(ssm));
        }

        public static SecurityStampModel DecryptSecurityStampModel(string value)
        {
            string jObj = Decrypt(value);
            return JsonConvert.DeserializeObject<SecurityStampModel>(jObj);
        }

        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string FromHexString(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return Encoding.Unicode.GetString(bytes);
        }
    }
}

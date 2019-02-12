using RemsNG.Common.Models;
using System;
using System.Text;

namespace RemsNG.Common.Utilities
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

            string t = Encoding.Unicode.GetString(bytes); ;
            return t;
        }
    }
}

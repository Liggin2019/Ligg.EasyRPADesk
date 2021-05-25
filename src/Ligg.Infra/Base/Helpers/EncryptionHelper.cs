using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Ligg.Infrastructure.Base.Extension;

namespace Ligg.Infrastructure.Base.Helpers
{
    public static class EncryptionHelper
    {
        public static string Key1 = _selfKey1;
        public static string Key2 = _selfKey2;
        private static string _selfKey1 = "mOtXb01/2Mp8kIOYD/hbAg==";
        private static string _selfKey2 = "wEdL50/eAJFSx+0thR2hhg==";

        private static readonly string
            TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //#md5 
        public static String Md5Encrypt(String source)
        {
            char[] hashedDatachars;
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(source));
            long arrayLength = (long)((4.0d / 3.0d) * hashedDataBytes.Length);
            if (arrayLength % 4 != 0)
            {
                arrayLength += 4 - arrayLength % 4;
            }

            hashedDatachars = new char[arrayLength];
            Convert.ToBase64CharArray(hashedDataBytes, 0, hashedDataBytes.Length, hashedDatachars, 0);
            String tmp = new String(hashedDatachars);
            return tmp;
        }



    }
}


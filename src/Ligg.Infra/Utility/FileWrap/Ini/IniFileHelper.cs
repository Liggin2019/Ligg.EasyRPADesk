using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Ligg.Infrastructure.Utility.FileWrap
{
    public class IniFileHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);
        public static string ReadIniString(string filePath, string section, string key, string defVal)
        {
            try
            {
                var buffer = new Byte[65535];
                int bufLen = GetPrivateProfileString(section, key, defVal, buffer, buffer.GetUpperBound(0), filePath);
                string s = Encoding.GetEncoding(0).GetString(buffer);
                s = s.Substring(0, bufLen);
                return s.Trim();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ReadIniString Error: " + ex.Message);
            }


        }


    }
}
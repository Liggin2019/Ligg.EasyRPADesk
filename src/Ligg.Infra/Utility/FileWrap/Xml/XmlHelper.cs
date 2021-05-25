using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Data;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;

namespace Ligg.Infrastructure.Utility.FileWrap
{
    public static class XmlHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public static T ConvertToGeneric<T>(string xmlStr)
        {
            try
            {
                return ConvertToGeneric<T>(xmlStr, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }

        public static T ConvertToGeneric<T>(string xmlStr, Encoding encoding)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new MemoryStream(encoding.GetBytes(xmlStr)))
                {
                    return (T)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }









    }
}

using System;
using System.IO;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using Ligg.Infrastructure.Extensions;

namespace Ligg.Infrastructure.Helpers
{
    internal static class XmlHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static T ConvertToGeneric<T>(string xmlStr, Encoding encoding = null)
        {
            try
            {
                if (encoding == null) encoding = Encoding.UTF8;
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new MemoryStream(encoding.GetBytes(xmlStr)))
                {
                    return (T)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".ConvertToGeneric Error: " + ex.Message);
            }
        }

    }
}

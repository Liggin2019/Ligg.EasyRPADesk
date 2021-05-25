using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace Ligg.Infrastructure.Base.Helpers
{
    public static class GenericHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*get
        public static long GetListLargestId<T>(List<T> list)
        {
            try
            {
                long result = 0;
                if (list.Count > 0)
                {
                    Type tp = typeof(T);
                    var idField = tp.GetField("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    for (int i = 0; i < list.Count; i++)
                    {
                        var id = Convert.ToInt64(idField.GetValue(list[i]));
                        if (id > result) result = id;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".GetLargestId Error: " + ex.Message);
            }
        }

        //*convert
        //##ConvertToRichText
        public static string ConvertListToRichText<T>(List<T> list, bool ignoreFields)
        {
            var strBlder = new StringBuilder();
            try
            {
                if (list.Count > 0)
                {
                    Type tp = typeof(T);
                    var fields = tp.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (!ignoreFields)
                    {
                        var txt = "";
                        var ct = 0;
                        foreach (var field in fields)
                        {
                            txt = ct == 0 ? field.Name : txt + "\t" + field.Name;
                            ct++;
                        }
                        strBlder.AppendLine(txt);
                    }


                    for (int i = 0; i < list.Count; i++)
                    {
                        var ct = 0;
                        var txt1 = "";
                        foreach (var field in fields)
                        {
                            object obj = field.GetValue(list[i]);
                            txt1 = ct == 0 ? "" + obj : txt1 + "\t" + obj;
                            ct++;
                        }
                        strBlder.AppendLine(txt1);
                    }
                }

                return strBlder.ToString();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertListToRichText Error: " + ex.Message);
            }
        }



        public static string ConvertToXmlInUtf8<T>(T gnrc)
        {
            try
            {
                return ConvertToXml<T>(gnrc, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToXmlInUtf8 Error: " + ex.Message);
            }
        }

        private static string ConvertToXml<T>(T gnrc, Encoding encoding)
        {
            try
            {
                var serializer = new XmlSerializer(gnrc.GetType());
                Encoding utf8EncodingWithNoByteOrderMark = new UTF8Encoding(false);
                using (var mem = new MemoryStream())
                {
                    var writerSettings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true,
                        Encoding = utf8EncodingWithNoByteOrderMark
                    };
                    using (var xmlWriter = XmlWriter.Create(mem, writerSettings))
                    {
                        var serializerNamespaces = new XmlSerializerNamespaces();
                        serializerNamespaces.Add("", "");
                        serializer.Serialize(xmlWriter, gnrc, serializerNamespaces);
                        return encoding.GetString(mem.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToXml Error: " + ex.Message);
            }
        }

        private static string ConvertToXml<T>(T gnrc) //same as above
        {
            try
            {
                var stream = new MemoryStream();
                var xml = new XmlSerializer(typeof(T));
                try
                {
                    xml.Serialize(stream, gnrc);
                }
                catch (InvalidOperationException)
                {
                    throw;
                }
                stream.Position = 0;
                var streamReader = new StreamReader(stream);
                string str = streamReader.ReadToEnd();

                streamReader.Dispose();
                stream.Dispose();
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToXml Error: " + ex.Message);
            }
        }

        private static string ConvertListToXml<T>(List<T> list)   //old version, keep for ref
        {
            try
            {
                Type tp = typeof(T);
                string xml = "<root>";
                if (tp == typeof(string) | tp == typeof(int) | tp == typeof(long) | tp == typeof(DateTime) |
                    tp == typeof(double) | tp == typeof(decimal))
                {
                    foreach (T obj in list)
                    {
                        xml = xml + "<item>" + obj.ToString() + "</item>";
                    }
                }
                else
                {
                    xml = xml + "<items>";
                    foreach (T obj in list)
                    {
                        xml = xml + GenericHelper.ConvertToXmlInUtf8<T>(obj);
                    }
                    xml = xml + "</items>";
                }

                xml = xml + "</root>";
                return xml;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ConvertToXml Error: " + ex.Message);
            }
        }

    }
}


using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utility.DataParser;
using System;


namespace Ligg.Infrastructure.Utilities.DataParserUtil
{
    public static class DataParserHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static T ConvertToGeneric<T>(this string target, bool necessary, TxtDataType fomart, string tableName = null, bool trim = true, bool clear = true)
        {
            if (target.IsLegalAbsolutePath())
            {
                if (ConfigFileHelper.IsFileExisting(target))
                {
                    return new ConfigFileManager(target).ConvertToGeneric<T>();
                }
                else
                {
                    if (necessary)
                    {
                        FileHelper.CheckPathExistence(target);
                        throw new ArgumentException(_typeFullName + ".ConvertToGeneric error: " + "Data file postfix incorrect; filePath= " + target);
                    }
                }
            }

            else
            {
                return ConvertTextDataToGeneric<T>(target, necessary, fomart, trim, clear);
            }
            return default(T);
        }
        public static TxtDataType GetTextDataType(this string target)
        {
            var type = EnumHelper.GetByName(target, TxtDataType.Undefined);
            return type;
        }



        private static T ConvertTextDataToGeneric<T>(string str, bool necessary, TxtDataType fomart, bool trim = true, bool clear = true)
        {
            var type = typeof(T);
            if (fomart == TxtDataType.Lstring)
            {
                if (type.Name == "String")
                {
                    return (T)(object)str;
                    //return (T)Convert.ChangeType(arr, typeof(T));
                }
                else
                {
                    //if (necessary)
                    //{
                    //    throw new ArgumentException(_typeFullName + ".ConvertToGeneric error: " + "Convert format incorrect; fomart= " + fomart.ToString());
                    //}
                }

            }
            else if (fomart == TxtDataType.Larray)
            {
                if (type.Name == "String[]" & fomart == TxtDataType.Larray)
                {
                    var obj = TextDataHelper.GetLarrayArray(str, trim, clear);
                    return (T)(object)obj;
                }
                else if (type.Name == "UniversalResult")
                {
                    var obj = TextDataHelper.ConvertLarryToUnivaralResult(str, trim, clear);
                    return (T)(object)obj;
                }
                else if (type.Name == "ScoreResult")
                {
                    var obj = TextDataHelper.ConvertLarryToScoreResult(str, trim, clear);
                    return (T)(object)obj;
                }
                else if (type.Name == "List`1" & type.FullName.Contains("KeyValue"))
                {
                    var obj = TextDataHelper.ConvertToKeyValueList(str, fomart, trim, clear);
                    return (T)(object)obj;
                }
                else if (type.Name == "String[]" & fomart == TxtDataType.Larray)
                {
                    var obj = TextDataHelper.GetLarrayArray(str, trim, clear);
                    return (T)(object)obj;
                }
                else
                {
                    //if (necessary)
                    //{
                    //    throw new ArgumentException(_typeFullName + ".ConvertToGeneric error: " + "Convert incorrect; TxtDataType= " + fomart.ToString() + "; TargetTypeName= " + type.Name);
                    //}
                }

            }
            else if (fomart == TxtDataType.Ldict)
            {
                if (type.Name == "List`1" & type.FullName.Contains("KeyValue"))
                {
                    var obj = TextDataHelper.ConvertToKeyValueList(str, fomart, trim, clear);
                    return (T)(object)obj;
                    //return (T)Convert.ChangeType(arr, typeof(T));
                }
                else if (type.Name == "Dictionary`2" & type.FullName.Contains("String"))
                {
                    var obj = TextDataHelper.ConvertLdictToDictionary(str, trim, clear, true);
                    return (T)(object)obj;
                }

                else if (type.Name != "List`1")
                {
                    var dict = TextDataHelper.ConvertLdictToDictionary(str, trim, clear, true);
                    var json = JsonHelper.Serialize(dict);
                    return JsonHelper.ConvertToGeneric<T>(json);
                }
                else
                {
                    //if (necessary)
                    //{
                    //    throw new ArgumentException(_typeFullName + ".ConvertToGeneric error: " + "TxtDataType= " + fomart.ToString() + "; TargetTypeName= " + type.Name);
                    //}
                }

            }
            else if (fomart == TxtDataType.Json)
            {
                return JsonHelper.ConvertToGeneric<T>(str);
            }
            else
            {
                if (necessary)
                    throw new NotImplementedException(_typeFullName + ".ConvertToGeneric error: " + "TxtDataType = " + fomart.ToString() + "; TargetTypeName= " + type.Name);
            }

            if (necessary)
            {
                throw new ArgumentException(_typeFullName + ".ConvertToGeneric error: " + "TxtDataType= " + fomart.ToString() + "; TargetTypeName= " + type.Name);
            }
            else return default(T);
        }







    }
}

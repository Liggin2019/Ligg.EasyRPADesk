//using  Ligg.RpaDesk.Parser.DataModels;
//using Ligg.Infrastructure.DataModels;
//using Ligg.Infrastructure.Extensions;
//using Ligg.Infrastructure.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;


//namespace Ligg.RpaDesk.Parser.Helpers
//{
//    public static class TextDataHelperOld
//    {
//        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

//        private static readonly char[] _larraySeparators = new char[] { '|', '/', ',' };
//        private static readonly char[] _ldictSeparators = new char[] { '*', ':' };//--xml不支持:>输不进去； < &读取报错 XmlHelper.ConvertToGeneric Error: XML 文档(41, 126)中有错误。
//        private static readonly char[] _ltableSeparators = new char[] { '\n'};
//        //*convertto
//        public static Dictionary<string, string> ConvertToDictionary(this string target, TxtDataType txtDataFormat)
//        {
//            if (target.IsNullOrEmpty()) return null;
//            var dic = new Dictionary<string, string>();
//            if (txtDataFormat == TxtDataType.Larray)
//            {
//                var arry = target.GetLarrayArray(true, true);
//                if (arry != null)
//                {
//                    for (int i = 0; i < arry.Length; i++)
//                    {
//                        dic.Add(i.ToString(), arry[i]);
//                    }
//                }
//            }
//            else if (txtDataFormat == TxtDataType.Ldict)
//            {
//                var arry = target.GetLarrayArray(true, true);
//                {
//                    foreach (var str in arry)
//                    {
//                        var arry1 = str.GetLdictArray(true, true);
//                        dic.Add(arry1[0], arry1[1]);
//                    }
//                }
//            }
//            else
//            {
//                dic=JsonHelper.ConvertToGeneric<Dictionary<string, string>>(target);
//            }
//            return dic;

//        }
//        public static List<ValueText> ConvertToValueTextList(this string target, TxtDataType txtDataFormat)
//        {
//            if (target.IsNullOrEmpty()) return null;

//            var valTexts = new List<ValueText>();
//            if (txtDataFormat == TxtDataType.Larray)
//            {
//                var arry = target.GetLarrayArray(true, true);
//                if (arry != null)
//                {
//                    for (int i = 0; i < arry.Length; i++)
//                    {
//                        var valTxt = new ValueText();
//                        valTxt.Value = i.ToString();
//                        valTxt.Text = arry[i];
//                        valTexts.Add(valTxt);
//                    }
//                }
//            }
//            else if (txtDataFormat == TxtDataType.Ldict)
//            {
//                var arry = target.GetLarrayArray(true, true);
//                if (arry != null)
//                {
//                    foreach (var str in arry)
//                    {
//                        var arry1 = str.GetLdictArray(true, true);
//                        var valTxt = new ValueText();
//                        valTxt.Value = arry1[0];
//                        valTxt.Text = arry1[1];
//                        valTexts.Add(valTxt);
//                    }
//                }
//            }
//            else
//            {
//                valTexts = JsonHelper.ConvertToGeneric<List<ValueText>>(target);
//            }
//            return valTexts;
//        }



//        //##ConvertToUnversalResult
//        public static UniversalResult ConvertToUnversalResult(this string target, TxtDataType txtDataFormat)
//        {
//            if (target.IsNullOrEmpty()) return null;
//            if (txtDataFormat == TxtDataType.Larray)
//            {
//                var arry = target.GetLarrayArray(true, true);
//                if (arry != null)
//                    return new UniversalResult(arry[0].ToLower() == "true", arry.Length > 1 ? arry[1] : "");
//            }
//            else if (txtDataFormat == TxtDataType.Ldict)
//            {
//                var dic = target.ConvertToDictionary(txtDataFormat);
//                if (dic != null)
//                    return new UniversalResult(dic["Success"].ToLower() == "true", dic["Message"]);
//            }
//            else
//            {
//                return JsonHelper.ConvertToGeneric<UniversalResult>(target);
//            }
//            return null;
//        }



//        //*convertfr
//        public static string ConvertFromUniversalResult(UniversalResult univesalResult, TxtDataType txtDataFormat)
//        {
//            var retTxt = "";
//            if (txtDataFormat == TxtDataType.Larray)
//            {
//                var arry = new string[] { univesalResult.Success.ToString().ToLower(), univesalResult.Message };
//                retTxt = arry.UnwrapLarray();
//            }
//            else if (txtDataFormat == TxtDataType.Ldict)
//            {
//                var arry = new string[] { univesalResult.Success.ToString().ToLower(), univesalResult.Message };
//                var str = arry.UnwrapLarray();
//                var subParamSeparator = univesalResult.Message.GetLarraySeparator();
//                var keyValParamSeparator = univesalResult.Message.GetLdictSeparator();

//                retTxt = "Success" + keyValParamSeparator + univesalResult.Success.ToString() + subParamSeparator +
//                   "Message" + keyValParamSeparator + univesalResult.Message + subParamSeparator;

//            }
//            else
//            {
//                retTxt = GenericHelper.ConvertToJson(univesalResult);//json
//            }
//            return retTxt;
//        }



//        //*larray
//        public static string WashLarry(this string target)
//        {
//            if (target.IsNullOrEmpty()) return string.Empty;
//            var arry = target.GetLarrayArray(true, false);
//            var str1 = TextDataHelper.UnwrapLarray(arry);
//            return str1;
//        }

//        public static string UnwrapLarray(this String[] strArry)
//        {
//            var exInfo = "\n>> " + _typeFullName + ".UnwrapLarray Error: ";
//            string separator = _larraySeparators[0].ToString();
//            if (strArry.Any(x => x.Contains(_larraySeparators[0]))) throw new ArgumentException(exInfo + "any of strArry should not contain highest level Separator: " + _larraySeparators[0]);

//            //if (strArry.Any(x => !x.Contains(_larraySeparators[1]))& strArry.Any(x => !x.Contains(_larraySeparators[2]))) separator = _larraySeparators[2].ToString();
//            if (strArry.Any(x => x.Contains(_larraySeparators[1])) & strArry.Any(x => x.Contains(_larraySeparators[2]))) separator = _larraySeparators[0].ToString();
//            else if (strArry.Any(x => x.Contains(_larraySeparators[1]))) separator = _larraySeparators[0].ToString();
//            else if (strArry.Any(x => x.Contains(_larraySeparators[2]))) separator = _larraySeparators[1].ToString();
//            else separator = _larraySeparators[2].ToString();
//            return strArry.Unwrap(separator);
//        }

//        public static char GetLarraySeparator(this string target)
//        {
//            var separator = _larraySeparators[2];
//            if (target.IsNullOrEmpty()) return separator;
//            if (target.Contains(_larraySeparators[0])) separator = _larraySeparators[0];
//            else if (target.Contains(_larraySeparators[1])) separator = _larraySeparators[1];
//            return separator;
//        }

//        public static string[] GetLarrayArray(this string target, bool trim, bool clear)
//        {
//            if (target.IsNullOrEmpty()) return null;
//            var separator = target.GetLarraySeparator();
//            var arry = target.Split(separator);

//            if (trim) arry = StringArrayExtension.Trim(arry);
//            if (clear) arry = StringArrayExtension.Clear(arry);
//            return arry;
//        }

//        public static string ExtractLarray(this string target, string ids)
//        {
//            if (string.IsNullOrEmpty(target))
//                return string.Empty;
//            var strArry = target.GetLarrayArray(true, false);
//            var separator = ids.GetLarraySeparator();
//            strArry = strArry.Extract(ids, separator);
//            return strArry.UnwrapLarray();
//        }

//        public static bool ContainsParallelSeparator(this string target)
//        {
//            if (string.IsNullOrEmpty(target)) return false;
//            foreach (var separator in _larraySeparators)
//            {
//                if (target.Contains(separator)) return true;
//            }
//            return false;
//        }

//        //*ldict
//        public static string GetLdictFromDictionary(Dictionary<string, string> dict)
//        {
//            if (dict == null)
//                return string.Empty;
//            bool includeLdictSeparator0 = false;
//            bool includeLdictSeparator1 = false;
//            bool includeLArraySeparator0 = false;
//            bool includeLArraySeparator1 = false;
//            bool includeLArraySeparator2 = false;
//            foreach (var dictItem in dict)
//            {
//                if (dictItem.Key.Contains(_ldictSeparators[0])) includeLdictSeparator0 = true;
//                if (dictItem.Key.Contains(_ldictSeparators[1])) includeLdictSeparator0 = true;
//                if (dictItem.Value.Contains(_ldictSeparators[0])) includeLdictSeparator0 = true;
//                if (dictItem.Value.Contains(_ldictSeparators[1])) includeLdictSeparator1 = true;

//                if (dictItem.Key.Contains(_larraySeparators[0])) includeLArraySeparator1 = true;
//                if (dictItem.Key.Contains(_larraySeparators[1])) includeLArraySeparator0 = true;
//                if (dictItem.Key.Contains(_larraySeparators[2])) includeLArraySeparator2 = true;
//                if (dictItem.Value.Contains(_larraySeparators[0])) includeLArraySeparator1 = true;
//                if (dictItem.Value.Contains(_larraySeparators[1])) includeLArraySeparator0 = true;
//                if (dictItem.Value.Contains(_larraySeparators[2])) includeLArraySeparator2 = true;
//            }
//            if (includeLdictSeparator0 & includeLdictSeparator1) throw new ArgumentException
//                    (_typeFullName + ".GetLdictFromDictionary Error: " + "canot include all Ldict Separators: " + _ldictSeparators[0] + " & " + _ldictSeparators[1]);
//            if (includeLArraySeparator0 & includeLArraySeparator1 & includeLArraySeparator2) throw new ArgumentException
//                    (_typeFullName + ".GetLdictFromDictionary Error: " + "canot include all Larray Separators: " + _larraySeparators[0] + " & " + _larraySeparators[1] + " & " + _larraySeparators[2]);

//            var ldictSeparator = _ldictSeparators[0];
//            if (!includeLdictSeparator1) ldictSeparator = _ldictSeparators[1];

//            var larraySeparator = _larraySeparators[0];
//            if (!includeLArraySeparator2) larraySeparator = _larraySeparators[2];
//            else if (!includeLArraySeparator1) larraySeparator = _larraySeparators[1];
//            var rst = "";
//            var i = 0;
//            foreach (var dictItem in dict)
//            {
//                rst = i == 0 ? dictItem.Key + ldictSeparator + dictItem.Value :
//                    rst + larraySeparator + dictItem.Key + ldictSeparator + dictItem.Value;
//                i++;
//            }

//            return rst;
//        }

//        public static string GetLdictValue(this string target, string key)
//        {
//            if (string.IsNullOrEmpty(target))
//                return string.Empty;
//            var strArry = target.GetLarrayArray(true, true);
//            for (int i = 0; i < strArry.Length; i++)
//            {
//                var strArry1 = strArry[i].GetLdictArray(true, false);
//                if (key.ToLower() == strArry1[0].ToLower())
//                    return strArry1[1];
//            }
//            return string.Empty;
//        }

//        private static char GetLdictSeparator(this string target)
//        {

//            var separator = _ldictSeparators[1];
//            if (target.Contains(_ldictSeparators[0])) separator = _ldictSeparators[0];
//            return separator;
//        }
//        private static string[] GetLdictArray(this string target, bool trim, bool clear)
//        {
//            var separator = target.GetLdictSeparator();
//            var arry = target.Split(separator);

//            if (trim) arry = StringArrayExtension.Trim(arry);
//            if (clear) arry = StringArrayExtension.Clear(arry);
//            return arry;
//        }
//        //*ltable
//        public static string ConvertLtableToJson(this string target)
//        {
//            if (target.IsNullOrEmpty()) return string.Empty;

//            var separator = target.GetLtableSeparator();
//            var arry = target.Split(separator);
//            var i=0;
//            var dt = new DataTable();
//            foreach (var item in arry)
//            {
//                var arr = item.GetLarrayArray(false,false);
//                if (i==0)
//                {
//                    foreach (string str in arr)
//                    {
//                        dt.Columns.Add(str);
//                    }
//                }
//                else
//                {
//                    DataRow dr = dt.NewRow();
//                    for (int j = 0; j < dt.Columns.Count; j++)
//                    {
//                        dr[j] = j < arr.Length ? arr[j] : "";
//                    }
//                    dt.Rows.Add(dr);
//                }
//                i++;
//            }
//            return dt.ConvertToJson();
//        }

//        private static char GetLtableSeparator(this string target)
//        {
//            //if(target.IsNullOrEmpty()) throw new ArgumentException(_typeFullName + "." + "GetLtableSeparator Error: target can't be empty");
//            //if (target.Contains(_ltableSeparators[0])&& target.Contains(_ltableSeparators[1]))
//            //    throw new ArgumentException(_typeFullName + "." + "GetLtableSeparator Error: target=" + target + " Contains both _ltableSeparators[1]-"+ _ltableSeparators[1]+ " &_ltableSeparators[0]-" + _ltableSeparators[0]);

//            var separator = _ltableSeparators[0];
//            return separator;
//        }

//        //*common
//        public static string GetJudgementFlag(this string target)
//        {
//            if (target.IsNullOrEmpty()) return "false";
//            if (target.ToLower() == "true") return "true";
//            return "false";
//        }

//        public static bool JudgeJudgementFlag(this string target)
//        {
//            if (target.IsNullOrEmpty()) return false;
//            if (target.ToLower() == "true") return true;
//            return false;
//        }







//    }


//}

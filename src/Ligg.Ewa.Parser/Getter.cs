using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;


using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.FileWrap;
using Ligg.EasyWinApp.Parser.Helpers;


namespace Ligg.EasyWinApp.Parser
{
    public static class Getter
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string GetText(string funcName, string[] funcParamArray)
        {
            //*common
            if (funcName.ToLower() == "empty" | funcName.ToLower() == "null")
            {
                return string.Empty;
            }   
            else if (funcName.ToLower() == "DateTime".ToLower())
            {
                var customFormat = "yyyy-MM-dd HH:mm:ss";
                if (funcParamArray[0].ToLower() == "UtcNow".ToLower())
                {
                    var time = SystemTimeHelper.UtcNow(); //
                    return time.ToString(customFormat, DateTimeFormatInfo.InvariantInfo);
                }
                else if (funcParamArray[0].ToLower() == "Now".ToLower())
                {
                    var time = SystemTimeHelper.Now(); //
                    return time.ToString(customFormat, DateTimeFormatInfo.InvariantInfo);
                }
                else throw new ArgumentException("funcName: " + funcName + " has no param " + funcParamArray[0] + "! ");
            }
            else if (funcName.ToLower() == "UniqueCode".ToLower())
            {
                if (funcParamArray[0] == "ByNow")
                {
                    var baseStr = funcParamArray.Length > 1 ? funcParamArray[1] : "";
                    var separator = funcParamArray.Length > 2 ? funcParamArray[2] : "";
                    return baseStr.ToUniqueStringByNow(separator);
                }
                else if (funcName.ToLower() == "ShortGuid".ToLower())
                {
                    var baseStr = funcParamArray.Length > 1 ? funcParamArray[1] : "";
                    var separator = funcParamArray.Length > 2 ? funcParamArray[2] : "";
                    return baseStr.ToUniqueStringByShortGuid(separator);
                }
                else throw new ArgumentException("funcName: " + funcName + " has no param '" + funcParamArray[0] + "'! ");
            }

            else if (funcName.ToLower() == "Format".ToLower())
            {
                if (funcParamArray[0].ToLower() == "upper")
                {
                    return funcParamArray[1].ToUpper();
                }
                else if (funcParamArray[0].ToLower() == "lower")
                {
                    return funcParamArray[1].ToLower();
                }
                else if (funcParamArray[0].ToLower() == "timespan")
                {
                    return SystemTimeHelper.GetTimeSpanString(Convert.ToDouble(funcParamArray[1]), funcParamArray[2],
                        false);
                }
                else if (funcParamArray[0].ToLower() == "real")
                {
                    return string.Format(funcParamArray[2], Convert.ToDouble(funcParamArray[1]));
                }

                else if (funcParamArray[0].ToLower() == "FormatString")
                {
                    return string.Format(funcParamArray[1], funcParamArray[2]);
                }
                else throw new ArgumentException(funcName + " has no param '" + funcParamArray[0] + "'! ");
            }
            else if (funcName.ToLower() == "Replace".ToLower())
            {
                return funcParamArray[1].Length == 0 ? funcParamArray[0]
                    : funcParamArray[0].Replace(funcParamArray[1], funcParamArray[2]);
            }
            else if (funcName.ToLower() == "Split".ToLower())
            {
                var separator = funcParamArray[1][0];
                var tmpStrArray = funcParamArray[0].Split(separator);
                var index = Convert.ToInt16(funcParamArray[2]);
                if (index > tmpStrArray.Length || index == tmpStrArray.Length)
                {
                    return "";
                }
                else
                {
                    return tmpStrArray[index];
                }
            }
            else if (funcName.ToLower() == "Combine".ToLower())
            {
                var tmpStrArray = funcParamArray[0].GetSubParamArray(false, false);
                var rtStr = "";
                if (funcParamArray.Length > 1)
                {
                    var joinSeparator = funcParamArray[1].Length == 1 ? Convert.ToChar(funcParamArray[1]) : ' ';
                    rtStr = Ligg.Infrastructure.Base.Helpers.StringHelper.UnwrapStringArrayBySeparator(tmpStrArray, joinSeparator);
                }
                else
                {
                    foreach (var tmpStr in tmpStrArray)
                    {
                        rtStr = rtStr + tmpStr;
                    }
                }
                return rtStr;
            }
            else if (funcName.ToLower() == "SubString".ToLower())
            {
                var tmStr = funcParamArray[0];
                Int16 sttIndex = Convert.ToInt16(funcParamArray[1]);
                Int16 len = Convert.ToInt16(funcParamArray[2]);
                return tmStr.Substring(sttIndex, len);
            }
            else if (funcName.ToLower() == "AddOrRemoveSubParam".ToLower())
            {
                var separator = ',';
                if (funcParamArray[0].ContainsSubParamSeparator())
                {
                    separator = funcParamArray[0].GetSubParamSeparator();
                }

                var add = funcParamArray[2].ToLower() == "true" ? true : false;
                return funcParamArray[0].AddOrDelToSeparatedStrings(funcParamArray[1], add, separator);

            }

            //*get
            else if (funcName.ToLower() == "GetLineQty".ToLower())
            {
                int qty = funcParamArray[0].Split('\n').Length;
                return Convert.ToString(qty);
            }
            else if (funcName.ToLower() == "GetLinesBySearch".ToLower())
            {
                var strArry = funcParamArray[0].Split('\n');
                var schStrArry = funcParamArray[1].Split(',');
                var strList = new List<string>();
                foreach (var v in strArry)
                {
                    foreach (var s in schStrArry)
                    {
                        if (v.ToLower().Contains(s.ToLower()))
                        {
                            strList.Add(v);
                        }
                    }
                }

                var strList1 = strList.Distinct();
                var strBlder = new StringBuilder();
                foreach (var v in strList1)
                {
                    if (!string.IsNullOrEmpty(v))
                    {
                        strBlder.AppendLine(v);
                    }
                }

                return strBlder.ToString();
            }

            //*convert
            else if (funcName.ToLower() == "ConvertJsonToRichText".ToLower())
            {
                var jsonStr = funcParamArray[0];
                var dt = JsonHelper.ConvertToDataTable(jsonStr);
                bool hasHead = false;
                if (funcParamArray.Length > 1)
                {
                    if (funcParamArray[1].ToLower() == "true") hasHead = true;
                }

                String[] strArray = null;
                if (funcParamArray.Length > 2)
                {
                    if (!funcParamArray[2].IsNullOrEmpty())
                    {
                        strArray = funcParamArray[2].GetSubParamArray(true, true);
                    }
                }
                var rtStr = DataTableHelper.ConvertToRichText(dt, hasHead, strArray);

                return rtStr;
            }

            //*file
            else if (funcName.ToLower() == "FileDetail".ToLower())
            {
                if (funcParamArray[1].IsNullOrEmpty()) throw new ArgumentException("file path can't be empty! ");
                if (funcParamArray[0].ToLower() == "Directory".ToLower())
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.Directory);
                }
                else if (funcParamArray[0].ToLower() == "FileName".ToLower())
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.FileName);
                }
                else if (funcParamArray[0].ToLower() == "FileTitle".ToLower())
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.FileTitle);
                }
                else if (funcParamArray[0].ToLower() == "Suffix".ToLower())
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.Suffix);
                }
                else if (funcParamArray[0].ToLower() == "Postfix".ToLower())
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.Postfix);
                }
                else throw new ArgumentException("funcName: " + funcName + " has no param '" + funcParamArray[0] + "'! ");
            }
            //*no use yet
            else if (funcName.ToLower() == "CompareFile".ToLower())
            {
                //var result = FileHelper.Compare2Files(funcParamArray[1], funcParamArray[2]).ToString();
                return string.Empty;
            }


            else if (funcName.ToLower() == "GetContentFromTextFile".ToLower() | funcName.ToLower() == "GetCttFrTextFile".ToLower())
            {
                return FileHelper.GetContentFromTextFile(funcParamArray[0]);
            }

            else if (funcName.ToLower() == "GetIniFileSetting".ToLower())
            {
                var filePath = funcParamArray[0];
                FileHelper.CheckFilePathExistence(filePath);
                var key = funcParamArray[1];
                var defVal = funcParamArray.Length > 2 ? funcParamArray[2] : "";
                return IniFileHelper.ReadIniString(filePath, "setting", key, defVal);
            }

            //*Json

            //*calc
            else if (funcName.ToLower() == "Calc".ToLower())
            {
                if (funcParamArray[0].ToLower() == "add".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) + Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "sub".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) - Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "mtp".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) * Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "div".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) / Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "rnd".ToLower())
                {
                    return (Math.Round(Convert.ToDouble(funcParamArray[1]))).ToString();
                }
                else if (funcParamArray[0].ToLower() == "spls".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) % (Convert.ToDouble(funcParamArray[2]))).ToString();
                }
                else throw new ArgumentException("funcName: " + funcName + " has no param: " + funcParamArray[0] + "! ");

            }

            //*Status
            else if (funcName.ToLower() == "GetFinalStatus".ToLower())
            {
                if (funcParamArray.All(v => v.ToLower() == "true"))
                {
                    return "true";
                }
                if (funcParamArray.Any(v => v.ToLower() == "unknown"))
                {
                    return "unknown";
                }
                return "false";
            }

            //*bool
            else if (funcName.ToLower() == "GetBool".ToLower())
            {
                if (funcParamArray[0].ToLower() == "TotalStatus".ToLower())
                {
                    var returnStr = "true";
                    var subfuncParamArray = funcParamArray[1].Split(',');
                    if (subfuncParamArray.Any(v => v.ToLower() != "1"))
                    {
                        returnStr = "false";
                    }
                    return returnStr;
                }
                else if (funcParamArray[0].ToLower() == "And".ToLower())
                {
                    var returnStr = "true";
                    var subfuncParamArray = funcParamArray[1].Split(',');
                    if (subfuncParamArray.Any(v => v.ToLower() != "true"))
                    {
                        returnStr = "false";
                    }
                    return returnStr;
                }
                else if (funcParamArray[0].ToLower() == "Or".ToLower())
                {
                    var returnStr = "false";
                    var subfuncParamArray = funcParamArray[1].Split(',');
                    if (subfuncParamArray.Any(v => v.ToLower() == "true"))
                    {
                        returnStr = "true";
                    }
                    return returnStr;
                }
                else if (funcParamArray[0].ToLower() == "Not".ToLower())
                {
                    var returnStr = "true";
                    if (funcParamArray[1].ToLower() == "true") returnStr = "false";
                    return returnStr;
                }

                else if (funcParamArray[0].ToLower() == "IsDirectoryExisting".ToLower())
                {
                    var returnStr = "false";
                    if (Directory.Exists(funcParamArray[1])) returnStr = "true";
                    return returnStr;
                }
                else if (funcParamArray[0].ToLower() == "IsDirectoryHidden".ToLower())
                {
                    if (!Directory.Exists(funcParamArray[1])) return "false";
                    var returnStr = "false";
                    var di = new DirectoryInfo(funcParamArray[1]);
                    if ((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        returnStr = "true";
                    }
                    return returnStr;
                }
                else if (funcParamArray[0].ToLower() == "IsFileExisting".ToLower())
                {
                    var returnStr = "false";
                    if (File.Exists(funcParamArray[1])) returnStr = "true";
                    return returnStr;
                }

                else if (funcParamArray[0].ToLower() == "IfElse".ToLower())
                {
                    var con = funcParamArray[1];
                    var conArry = con.GetSubParamArray(true, false);
                    var judgedText = conArry[0];
                    var judgeFlag = conArry[1];
                    var val = "";
                    if (conArry.Length > 2)
                        val = conArry[2];
                    if (GetHelper.Judge(judgeFlag, judgedText, val)) return "true";
                    return "false";
                }

                else throw new ArgumentException("funcName: " + funcName + " has no param: " + funcParamArray[0] + "! ");
            }

            //*if
            else if (funcName.ToLower() == "IfElse".ToLower())
            {
                var con = funcParamArray[0];
                var returnVal = funcParamArray[1];
                var returnVal1 = funcParamArray[2];
                var conArry = con.GetSubParamArray(true, false);
                var judgedText = conArry[0];
                var judgeFlag = conArry[1];
                var val = "";
                if (conArry.Length > 2)
                    val = conArry[2];
                if (GetHelper.Judge(judgeFlag, judgedText, val)) return returnVal;
                return returnVal1;

            }
            //*encrypt
            else if (funcName.ToLower() == "EncryptText".ToLower())
            {
                if (funcParamArray[0].ToLower() == "Md5".ToLower())
                {
                    return EncryptionHelper.Md5Encrypt(funcParamArray[1]);
                }

                else throw new ArgumentException("funcName: " + funcName + " has no param: " + funcParamArray[0] + "! ");
            }


            else if (funcName.ToLower() == "Validate".ToLower())
            {

                var retStr = Validator.Validate(funcParamArray[0], funcParamArray[1]);
                if (retStr == "OutOfScopeOfValidator") return "OutOfScope";
                else return retStr;
            }

            else return "OutOfScope";

        }

    }
}
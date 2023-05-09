using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.Infrastructure.Utilities.DataParserUtil;


namespace Ligg.RpaDesk.Parser
{
    public static class Getter
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string Get(string funcName, string[] funcParamArray)
        {
            var exInfo = _typeFullName + ".Get error: ";
            //*common
            if (funcName == "DateTime")
            {
                var customFormat = "yyyy-MM-dd HH:mm:ss";
                if (funcParamArray.Length > 1) customFormat = funcParamArray[1];
                if (funcParamArray[0] == "UtcNow")
                {
                    var time = SystemTimeHelper.UtcNow();
                    return time.ToString(customFormat, DateTimeFormatInfo.InvariantInfo);
                }
                else if (funcParamArray[0] == "Now")
                {
                    var time = SystemTimeHelper.Now();
                    return time.ToString(customFormat, DateTimeFormatInfo.InvariantInfo);
                }
                else throw new ArgumentException(exInfo + "funcName: " + funcName + " has no param " + funcParamArray[0] + "! ");
            }
            else if (funcName == "GetUniqueId")
            {
                var type = funcParamArray.Length > 0 ? funcParamArray[0] : "";
                var rst = "";
                var uniqueStringType = EnumHelper.GetByName(type, UniqueStringType.Guid);
                if (uniqueStringType == UniqueStringType.ShortGuid)
                {
                    return "".ToUniqueStringByShortGuid("");
                }
                else if (uniqueStringType == UniqueStringType.ShortGuid)
                {
                    return "".ToUniqueStringByShortGuid("");
                }
                else if (uniqueStringType == UniqueStringType.Now)
                {
                    return "".ToUniqueStringByNow("");
                }
                else if (uniqueStringType == UniqueStringType.UtcNow)
                {
                    return "".ToUniqueStringByUtcNow("");
                }
                else if (uniqueStringType == UniqueStringType.RandomInteger)
                {
                    int min = funcParamArray.Length > 1 ? Convert.ToInt32(funcParamArray[1]) : 0;
                    int max = funcParamArray.Length > 2 ? Convert.ToInt32(funcParamArray[2]) : min + 10;
                    return "".ToUniqueStringByRandomInteger(min, max, "");
                }
                else throw new ArgumentException(exInfo + "funcName: " + funcName + " has no param " + funcParamArray[0] + "! ");
            }

            else if (funcName == "Format")
            {
                if (funcParamArray[0] == "Upper")
                {
                    return funcParamArray[1].ToUpper();
                }
                else if (funcParamArray[0] == "Lower")
                {
                    return funcParamArray[1];
                }
                else if (funcParamArray[0] == "TimeSpan")
                {
                    return SystemTimeHelper.GetTimeSpanString(Convert.ToDouble(funcParamArray[1]), funcParamArray[2],
                        false);
                }
                else if (funcParamArray[0] == "Real")
                {
                    return string.Format(funcParamArray[2], Convert.ToDouble(funcParamArray[1]));
                }

                else if (funcParamArray[0] == "FormatString")
                {
                    return string.Format(funcParamArray[1], funcParamArray[2]);
                }
                else throw new ArgumentException(exInfo + funcName + " has no param '" + funcParamArray[0] + "'! ");
            }
            else if (funcName == "Replace")
            {
                return funcParamArray[1].Length == 0 ? funcParamArray[0]
                    : funcParamArray[0].Replace(funcParamArray[1], funcParamArray[2]);
            }
            else if (funcName == "Split")
            {
                var str = funcParamArray[0];
                var separator = funcParamArray[1];
                var index = 0;
                if (funcParamArray.Length > 2) index = Convert.ToInt32(funcParamArray[2]);
                var clear = false;
                if (funcParamArray.Length > 3) clear = funcParamArray[3].ToLower() == "true";
                var trim = false;
                if (funcParamArray.Length > 4) trim = funcParamArray[4].ToLower() == "true";
                var tmpStrArray = str.SplitByString(separator, clear, trim);
                if (index > tmpStrArray.Length || index == tmpStrArray.Length)
                {
                    return "";
                }
                else
                {
                    return tmpStrArray[index];
                }
            }
            else if (funcName == "Join")
            {
                var rtStr = "";
                rtStr = funcParamArray.Unwrap();

                return rtStr;
            }
            else if (funcName == "Combine")
            {
                var rtStr = "";
                var joinSeparator = funcParamArray[0];
                var funcParamArray1 = new string[funcParamArray.Length - 1];
                for (var i = 0; i < funcParamArray1.Length; i++)
                {
                    funcParamArray1[i] = funcParamArray[i + 1];
                }
                rtStr = funcParamArray1.Unwrap(joinSeparator);

                return rtStr;
            }
            else if (funcName == "SubString")
            {
                var tmStr = funcParamArray[0];
                Int16 sttIndex = Convert.ToInt16(funcParamArray[1]);
                Int16 len = Convert.ToInt16(funcParamArray[2]);
                return tmStr.Substring(sttIndex, len);
            }
            //*doing
            else if (funcName == "AddToLarray")
            {
                var str = funcParamArray[0].AddToLarry();
                return str;

            }
            else if (funcName == "RemoveFromLarray")//*doing
            {
                var str = funcParamArray[0].RemoveFromLarry();
                return str;

            }


            //*get
            else if (funcName == "GetLineQty")
            {
                int qty = funcParamArray[0].Split('\n').Length;
                return Convert.ToString(qty);
            }
            else if (funcName == "GetLinesBySearch")
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
            else if (funcName == "ConvertJsonToRichText")
            {
                var jsonStr = funcParamArray[0];
                var dt = JsonHelper.ConvertToDataTable(jsonStr);
                bool hasHead = false;
                if (funcParamArray.Length > 1) hasHead = funcParamArray[1].ToLower() == "true";

                String[] strArray = null;
                if (funcParamArray.Length > 2)
                {
                    if (!funcParamArray[2].IsNullOrEmpty())
                    {
                        strArray = funcParamArray[2].GetLarrayArray(true, true);
                    }
                }
                var rtStr = DataTableHelper.ConvertToRichText(dt, hasHead, strArray);

                return rtStr;
            }

            //*file
            else if (funcName == "GetDirectoryRelativePath")
            {
                if (funcParamArray.Length < 1) return "";
                var dir = funcParamArray[0];
                if (dir.IsNullOrEmpty()) return "";
                if (!DirectoryHelper.IsLegalAbsoluteDirectory(dir)) return "";
                if (!DirectoryHelper.IsDirectoryExisting(dir)) return "";
                var path = funcParamArray[1];
                if (!path.ToLower().StartsWith(dir.ToLower())) return "";
                var rPath = path.Substring(dir.Length + 1, path.Length - dir.Length - 1);
                return rPath;

            }
            else if (funcName == "FileDetail")
            {
                if (funcParamArray[1].IsNullOrEmpty()) throw new ArgumentException("file path can't be empty! ");
                if (funcParamArray[0] == "Directory")
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.Directory);
                }
                else if (funcParamArray[0] == "FileName")
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.FileName);
                }
                else if (funcParamArray[0] == "FileTitle")
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.FileTitle);
                }
                else if (funcParamArray[0] == "Suffix")
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.Suffix);
                }
                else if (funcParamArray[0] == "Postfix")
                {
                    return FileHelper.GetFileDetailByOption(funcParamArray[1], FilePathComposition.Postfix);
                }
                else throw new ArgumentException(exInfo + "funcName: " + funcName + " has no param '" + funcParamArray[0] + "'! ");
            }


            else if (funcName == "GetContentFromTextFile")
            {
                return FileHelper.GetContentFromTextFile(funcParamArray[0]);
            }

            else if (funcName == "GetIniFileSetting")
            {
                var filePath = funcParamArray[0];
                FileHelper.CheckPathExistence(filePath);
                var key = funcParamArray[1];
                var defVal = funcParamArray.Length > 2 ? funcParamArray[2] : "";
                return IniFileHelper.ReadIniString(filePath, "setting", key, defVal);
            }

            //*Json

            //*calc
            else if (funcName == "Calc")
            {
                if (funcParamArray[0].ToLower() == "Add".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) + Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "Subtract".ToLower() | funcParamArray[0].ToLower() == "Sub".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) - Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "Multiply".ToLower() | funcParamArray[0].ToLower() == "Mtp".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) * Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "Divide".ToLower() | funcParamArray[0].ToLower() == "Div".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) / Convert.ToDouble(funcParamArray[2])).ToString();
                }
                else if (funcParamArray[0].ToLower() == "Round".ToLower() | funcParamArray[0].ToLower() == "Rnd".ToLower())
                {
                    return (Math.Round(Convert.ToDouble(funcParamArray[1]))).ToString();
                }
                else if (funcParamArray[0].ToLower() == "Model".ToLower() | funcParamArray[0].ToLower() == "Mod".ToLower())
                {
                    return (Convert.ToDouble(funcParamArray[1]) % (Convert.ToDouble(funcParamArray[2]))).ToString();
                }
                else throw new ArgumentException(exInfo + "funcName: " + funcName + " has no param: " + funcParamArray[0] + "! ");

            }

            //*Status
            else if (funcName == "GetFinalStatus")
            {
                if (funcParamArray.All(v => v.ToLower() == "true"))
                {
                    return "true";
                }
                else if (funcParamArray.Any(v => v.ToLower() == "undefined"))
                {
                    return "Undefined";
                }
                return "false";
            }
            else if (funcName == "ValidateTotal")
            {
                if (funcParamArray.Length < 2) throw new ArgumentException("funcName: " + funcName + " funcParamArray length can't be less than 2! ");
                var arr = funcParamArray.Extract(1, funcParamArray.Length - 1);
                if (funcParamArray[0] == "And")
                {
                    return TextDataHelper.ValidateTotal(arr, TernaryOption.Forward);
                }
                else if (funcParamArray[0] == "Or")
                {
                    return TextDataHelper.ValidateTotal(arr, TernaryOption.Original);
                }
                else if (funcParamArray[0] == "Not")
                {
                    return TextDataHelper.ValidateTotal(arr, TernaryOption.Backward);
                }
                else throw new ArgumentException("funcName: " + funcName + " has no param: " + funcParamArray[0] + "! ");
            }

            //*judge
            else if (funcName == "Judge")
            {
                if (funcParamArray[0] == "IsEmpty" | funcParamArray[0] == "IsNull" | funcParamArray[0] == "IsNullOrEmpty")
                {
                    return funcParamArray[1].IsNullOrEmpty().ToString();
                }
                else if (funcParamArray[0] == "IsInLarry")
                {
                    var ignoreCase = false;
                    if (funcParamArray.Length > 3) ignoreCase = funcParamArray[1].JudgeJudgementFlag();
                    var judge = funcParamArray[1].IsInLarry(funcParamArray[2], ignoreCase);
                    return judge.ToString();

                }
                else if (funcParamArray[0] == "UniversalResult")
                {
                    var str = funcParamArray[1];
                    return str.JudgeUniversalResult();
                }
                else if (funcParamArray[0] == "IsNotEmpty" | funcParamArray[0] == "IsNotNull" | funcParamArray[0] == "IsNotNullOrEmpty")
                {
                    return (!funcParamArray[1].IsNullOrEmpty()).ToString();
                }

                else if (funcParamArray[0] == "IsDirectoryExisting")
                {
                    var returnStr = "false";
                    if (Directory.Exists(funcParamArray[1])) returnStr = "true";
                    return returnStr;
                }
                else if (funcParamArray[0] == "IsDirectoryHidden")
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
                else if (funcParamArray[0] == "IsFileExisting")
                {
                    var returnStr = "false";
                    if (File.Exists(funcParamArray[1])) returnStr = "true";
                    return returnStr;
                }
                else if (funcParamArray[0] == "IfElse")
                {
                    var judgedText = funcParamArray[1];
                    var judgeFlag = funcParamArray[2];
                    var val = funcParamArray[3];
                    if (GetHelper.Judge(judgeFlag, judgedText, val)) return "true";
                    return "false";
                }
                else if (funcParamArray[0] == "IfAnyElse")
                {
                    var judgedText = "";
                    var judgeFlag = funcParamArray[1];
                    var val = funcParamArray[2];
                    for (var i = 3; i < funcParamArray.Length; i++)
                    {
                        judgedText = funcParamArray[i];
                        if (GetHelper.Judge(judgeFlag, judgedText, val)) return "true";
                    }
                    return "false";
                }

                else if (funcParamArray[0] == "And")
                {
                    for (var i = 1; i < funcParamArray.Length; i++)
                    {
                        if (funcParamArray[i].ToLower() != "true") return "false";

                    }
                    return "true";
                }
                else if (funcParamArray[0].ToLower() == "Or".ToLower())
                {
                    for (var i = 1; i < funcParamArray.Length; i++)
                    {
                        if (funcParamArray[i].ToLower() == "true") return "true";

                    }
                    return "false";
                }

                else if (funcParamArray[0].ToLower() == "Not".ToLower())
                {
                    if (funcParamArray[1].ToLower() == "true") return "false";
                    return "true";
                }



                else throw new ArgumentException("funcName: " + funcName + " has no param: " + funcParamArray[0] + "! ");
            }

            //*if
            else if (funcName == "IfElse")
            {
                var judgedText = funcParamArray[0];
                var judgeFlag = funcParamArray[1];
                var val = funcParamArray[2];
                var returnVal = funcParamArray[3];
                var returnVal1 = funcParamArray.Length > 4 ? funcParamArray[4] : judgedText;
                if (GetHelper.Judge(judgeFlag, judgedText, val)) return returnVal;
                return returnVal1;

            }
            else if (funcName == "IfAnyElse")
            {
                var judgedText = "";
                var judgeFlag = funcParamArray[0];
                var val = funcParamArray[1];
                //var returnVal = funcParamArray[i];
                var returnVal1 = funcParamArray[2];
                for (var i = 3; i < funcParamArray.Length; i++)
                {
                    judgedText = funcParamArray[i];
                    if (GetHelper.Judge(judgeFlag, judgedText, val)) return judgedText;
                }
                return returnVal1;
            }

            else if (funcName == "Validate")
            {

                var rst = Validator.Validate(funcParamArray[0], funcParamArray[1]);
                if (rst.Message == "LRDUNDEFINED") return "LRDUNDEFINED";
                else return GenericHelper.ConvertToJson(rst);
            }
            else if (funcName == ("ExecCmds") | funcName == ("ExecCmd") | funcName == ("RunBat") | funcName == ("RunPython")
                    | funcName == ("ExecCmdsA") | funcName == ("ExecCmdA") | funcName == ("RunBatA") | funcName == ("RunPythonA")
                    | funcName == ("ExecCmdsU") | funcName == ("ExecCmdU") | funcName == ("RunBatU") | funcName == ("RunPythonU")
                    | funcName == ("ExecCmdsAU") | funcName == ("ExecCmdAU") | funcName == ("RunBatAU") | funcName == ("RunPythonAU"))
            {
                return GetHelper.Run(funcName, funcParamArray);
            }

            else return "LRDUNDEFINED";

        }

    }
}
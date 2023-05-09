using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Extensions;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.Infrastructure.Utilities.DataParserUtil;

namespace Ligg.RpaDesk.Parser.Helpers
{
    public static class GetHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string ResolveSpecialFolder(string filePath)
        {
            if (filePath.ToLower().Contains("%currentdirectory%")) //C:
            {
                var tempStr = Directory.GetCurrentDirectory();
                filePath = Regex.Replace(filePath, "%curDir%", tempStr, RegexOptions.IgnoreCase);
            }
            if (filePath.ToLower().Contains("%systemdrive%")) //C:
            {
                var tempStr = DirectoryHelper.GetSpecialDir("systemdrive");
                filePath = Regex.Replace(filePath, "%systemdrive%", tempStr, RegexOptions.IgnoreCase);
            }

            if (filePath.ToLower().Contains("%systemroot%")) //C:\WINDOWS
            {
                var tempStr = DirectoryHelper.GetSpecialDir("systemroot");
                filePath = Regex.Replace(filePath, "%systemroot%", tempStr, RegexOptions.IgnoreCase);
            }
            if (filePath.ToLower().Contains("%systemdirectory%")) //C:\WINDOWS\system32
            {
                var tempStr = DirectoryHelper.GetSpecialDir("systemdirectory");
                filePath = Regex.Replace(filePath, "%systemdirectory%", tempStr, RegexOptions.IgnoreCase);
            }
            if (filePath.ToLower().Contains("%myprofile%")) //win7 C:\Users\chris.li; xp C:\Documents and Settings\Administrator
            {
                var tempStr = DirectoryHelper.GetSpecialDir("myprofile");
                filePath = Regex.Replace(filePath, "%myprofile%", tempStr, RegexOptions.IgnoreCase);
            }

            if (filePath.ToLower().Contains("%mydocuments%")) //win7 C:\Users\chris.li\Documents; xp C:\Documents and Settings\Administrator\My Documents 
            {
                var tempStr = DirectoryHelper.GetSpecialDir("mydocuments");
                filePath = Regex.Replace(filePath, "%mydocuments%", tempStr, RegexOptions.IgnoreCase);
            }
            if (filePath.ToLower().Contains("%personal%")) //win7 C:\Users\chris.li\Documents; xp C:\Documents and Settings\Administrator\My Documents 
            {
                var tempStr = DirectoryHelper.GetSpecialDir("personal");
                filePath = Regex.Replace(filePath, "%personal%", tempStr, RegexOptions.IgnoreCase);
            }

            if (filePath.ToLower().Contains("%commonapplicationdata%")) //win7 C:\ProgramData; xp C:\Documents and Settings\All Users\Application Data
            {
                var tempStr = DirectoryHelper.GetSpecialDir("commonapplicationdata");
                filePath = Regex.Replace(filePath, "%commonapplicationdata%", tempStr, RegexOptions.IgnoreCase);
            }

            if (filePath.ToLower().Contains("%Localapplicationdata%")) //win7 C:\Users\chris.li\AppData\Local; xp C:\Documents and Settings\Administrator\Local Settings\Application Data	
            {
                var tempStr = DirectoryHelper.GetSpecialDir("Localapplicationdata");
                filePath = Regex.Replace(filePath, "%Localapplicationdata%", tempStr, RegexOptions.IgnoreCase);
            }

            if (filePath.ToLower().Contains("%applicationdata%")) //win7 C:\Users\chris.li\AppData\Roaming; xp C:\Documents and Settings\Administrator\Application Data 
            {
                var tempStr = DirectoryHelper.GetSpecialDir("applicationdata");
                filePath = Regex.Replace(filePath, "%applicationdata%", tempStr, RegexOptions.IgnoreCase);
            }
            if (filePath.ToLower().Contains("%roamingapplicationdata%")) //win7 C:\Users\chris.li\AppData\Roaming; xp C:\Documents and Settings\Administrator\Application Data 
            {
                var tempStr = DirectoryHelper.GetSpecialDir("applicationdata");
                filePath = Regex.Replace(filePath, "%roamingapplicationdata%", tempStr, RegexOptions.IgnoreCase);
            }

            if (filePath.ToLower().Contains("%programfiles%"))
            {
                var tempStr = DirectoryHelper.GetSpecialDir("programfiles");
                filePath = Regex.Replace(filePath, "%programfiles%", tempStr, RegexOptions.IgnoreCase);
            }
            return filePath;
        }


        public static string FormatRichText(string text)
        {

            text = text.Replace("\\r\\t", "\t");
            text = text.Replace("\\r\\n", "\n");
            return text;

        }

        public static bool Judge(string judgeFlag, string judgedText, string val)
        {
            var exInfo = _typeFullName + ".Judge Error: ";

            if (judgeFlag.ToLower().Trim() == "Equal".ToLower())
            {
                if (judgedText == val)
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "NotEqual".ToLower())
            {
                if (judgedText.Trim() != val.Trim())
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "VEqual".ToLower())
            {
                if (judgedText == val)
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "NotVEqual".ToLower())
            {
                if (judgedText.Trim() != val.Trim())
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "VGreater".ToLower())
            {
                if (Convert.ToDouble(judgedText) > Convert.ToDouble(val))
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "VGreaterEqual".ToLower())
            {
                if (Convert.ToDouble(judgedText) >= Convert.ToDouble(val))
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "VLess".ToLower())
            {
                if (Convert.ToDouble(judgedText) < Convert.ToDouble(val))
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "VLessEqual".ToLower())
            {
                if (Convert.ToDouble(judgedText) <= Convert.ToDouble(val))
                {
                    return true;
                }
                return false;
            }

            //else if (judgeFlag.ToLower() == "IsEmpty".ToLower())
            //{
            //    if (judgedText.IsNullOrEmpty())
            //    {
            //        return true;
            //    }
            //    return false;
            //}
            //else if (judgeFlag.ToLower() == "IsNotEmpty".ToLower())
            //{
            //    if (!judgedText.IsNullOrEmpty())
            //    {
            //        return true;
            //    }
            //    return false;
            //}
            else if (judgeFlag.ToLower().Trim() == "Contains".ToLower().Trim())
            {
                if (!val.IsNullOrEmpty())
                {
                    if (judgedText.Contains(val))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "StartsWith".ToLower())
            {
                if (!val.IsNullOrEmpty())
                {
                    if (judgedText.StartsWith(val))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "EndsWith".ToLower())
            {
                if (!val.IsNullOrEmpty())
                {
                    if (judgedText.EndsWith(val))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "In".ToLower())
            {
                if (!val.IsNullOrEmpty())
                {
                    if (judgedText.In(val))
                    {
                        return true;
                    }
                }
                return false;

            }
            else if (judgeFlag.ToLower() == "NotIn".ToLower())
            {
                if (!val.IsNullOrEmpty())
                {
                    if (judgedText.NotIn(val))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                throw new ArgumentException(exInfo + "judgeFlag: " + judgeFlag + " does not exist!");
            }
        }


        public static bool VerifyPassword(string input, string verificationTypeFlag, string verificationParams)
        {
            var passwordVerificationType = EnumHelper.GetByName<PasswordEncryptionType>(verificationTypeFlag, PasswordEncryptionType.None);

            var rst = false;
            if (passwordVerificationType == PasswordEncryptionType.None)
            {
                var clearCode = verificationParams;
                if (clearCode == input) rst = true;
                return rst;
            }
            return false;
        }

        internal static string Run(string funcName, string[] funcParamArray)
        {
            var returnStr = "";
            var inputStr = funcParamArray[0];
            var args = funcParamArray.Length > 1 ? funcParamArray[1] : "";
            var execCmdWindowOptionStr = funcParamArray.Length > 2 ? funcParamArray[2] : "ShowWindow";
            var execCmdWindowOption = EnumHelper.GetByName<ExecCmdWindowOption>(execCmdWindowOptionStr, ExecCmdWindowOption.ShowWindow);
            var interceptOutput = funcParamArray.Length > 3 ? funcParamArray[3].JudgeJudgementFlag() : false;
            var raiseUacLevel = funcParamArray.Length > 4 ? funcParamArray[4].JudgeJudgementFlag() : false;
            var runAsAdmin = funcParamArray.Length > 5 ? funcParamArray[5].JudgeJudgementFlag() : false;
            if (funcName.StartsWith("ExecCmds"))
            {
                if (runAsAdmin)
                {
                    returnStr = RunAsAdminHelper.ExecCmds(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput);
                }
                else returnStr = SysProcessHelper.ExecCmds(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput, null);
            }
            else if (funcName.StartsWith("ExecCmd"))
            {
                if (runAsAdmin)
                {
                    returnStr = RunAsAdminHelper.ExecCmd(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput);
                }
                else returnStr = SysProcessHelper.ExecCmd(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput, null);
            }
            else if (funcName.StartsWith("RunBat"))
            {
                if (runAsAdmin)
                {
                    returnStr = RunAsAdminHelper.RunBat(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput);
                }
                else returnStr = SysProcessHelper.RunBat(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput, null);
            }
            else /*if (funcName.StartsWith("RunPython"))*/
            {
                if (runAsAdmin)
                    returnStr = RunAsAdminHelper.RunPython(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput);
                else returnStr = SysProcessHelper.RunPython(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptOutput, null);
            }

            return returnStr;
        }


        //*common
        private static bool In(this string target, string str)
        {
            if (str.IsNullOrEmpty()) return false;
            var strArray = str.GetLarrayArray(true, true);
            return strArray.Any(x => x.Equals(target));

        }

        private static bool NotIn(this string target, string str)
        {
            if (str.IsNullOrEmpty()) return false;
            var strArray = str.GetLarrayArray(true, true);
            return strArray.All(x => !x.Equals(target));
        }




    }
}
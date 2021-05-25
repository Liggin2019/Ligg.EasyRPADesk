using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.Extension;
using Ligg.EasyWinApp.Parser.DataModel.Enums;

namespace Ligg.EasyWinApp.Parser.Helpers
{
    public static class GetHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;


        //*resolve
        public static string ResolveConstants(string text)
        {
            if (!text.Contains("%")) return text;
            var toBeRplStr = "";
            toBeRplStr = "%Now%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%UtcNow%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = SystemTimeHelper.UtcNow().ToString("yyyy-MM-dd HH:mm:ss");
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%r%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = "\r";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%n%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = "\n";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%t%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = "\t";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            return ResolveSpecialFolder(text);
        }

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
        //##FormatRichText
        public static bool Judge(string judgeFlag, string judgedText, string val)
        {
            var exInfo = "\n>> " + TypeName + ".Judge Error: ";

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

            else if (judgeFlag.ToLower() == "IsEmpty".ToLower())
            {
                if (judgedText.IsNullOrEmpty())
                {
                    return true;
                }
                return false;
            }
            else if (judgeFlag.ToLower() == "IsNotEmpty".ToLower())
            {
                if (!judgedText.IsNullOrEmpty())
                {
                    return true;
                }
                return false;
            }
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
            var passwordVerificationType =EnumHelper.GetByName<PasswordEncryptionType>(verificationTypeFlag, PasswordEncryptionType.None);

            var isOk = false;
            if (passwordVerificationType == PasswordEncryptionType.None)
            {
                var clearCode = verificationParams;
                if (clearCode == input) isOk = true;
                return isOk;
            }
            else if (passwordVerificationType == PasswordEncryptionType.Md5)
            {
                var encryptedCode = verificationParams;
                var clearCode = input;

                isOk = EncryptionHelper.Md5Encrypt(clearCode) == encryptedCode;
                return isOk;
            }

            return false;
        }


        //*common
        private static bool In(this string target, string str)
        {
            if (str.IsNullOrEmpty()) return false;
            var strArray = str.GetSubParamArray(true, true);
            return strArray.Any(x => x.Equals(target));

        }

        private static bool NotIn(this string target, string str)
        {
            if (str.IsNullOrEmpty()) return false;
            var strArray = str.GetSubParamArray(true, true);
            return strArray.All(x => !x.Equals(target));
        }




    }
}
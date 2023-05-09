using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using System;
using System.IO;
using Ligg.RpaDesk.Parser.Helpers;

namespace Ligg.RpaDesk.Parser
{
    public static class Doer
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string Do(string funcName, string[] funcParamArray)
        {
            var returnStr = "";

            //*dir
            if (funcName == "CreateDirectory")
            {
                if (!Directory.Exists(funcParamArray[0]))
                {
                    DirectoryHelper.CreateDirectory(funcParamArray[0]);
                }
                if (funcParamArray.Length > 1 & funcParamArray[1].ToLower() == "true")
                {
                    var di = new DirectoryInfo(funcParamArray[0]);
                    if (!((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    {
                        di.Attributes = FileAttributes.Hidden;
                    }
                }
            }
            else if (funcName == "OpenDirectory")
            {
                if (!Directory.Exists(funcParamArray[0]))
                {
                    throw new ArgumentException("Directory does not exist! "); ;
                }
                else
                {
                    System.Diagnostics.Process.Start(funcParamArray[0]);
                }
            }

            //*file
            else if (funcName == "SaveContentToTextFile")
            {
                var path = funcParamArray[0];
                var ctt = funcParamArray[1];
                var append = false;
                if (funcParamArray.Length > 2)
                {
                    if (funcParamArray[2].ToLower() == "true") append = true;
                }

                FileHelper.SaveContentToTextFile(path, ctt, append);
            }
            else if (funcName == "DeleteFile")
            {
                var path = funcParamArray[0];
                if (File.Exists(path))
                    File.Delete(path);
            }
            else if (funcName == "CopyFile")
            {
                var path = funcParamArray[0];
                var path1 = funcParamArray[1];

                var overWrite = false;
                if (funcParamArray.Length > 2)
                {
                    overWrite = funcParamArray[2].ToLower() == "true";
                }

                File.Copy(path, path1, true);
            }
            else if (funcName.ToLower() == "SendLocalEmail".ToLower())
            {

                var mailTo = funcParamArray[0];
                var subject = funcParamArray.Length > 1 ? funcParamArray[1] : "";
                var body = funcParamArray.Length > 2 ? funcParamArray[2] : "";
                if (body.Contains("\r\n"))
                    body = body.Replace("\r\n", "%0D%0A");
                if (body.Contains("\n\r"))
                    body = body.Replace("\n\r", "%0D%0A");
                if (body.Contains("\r"))
                    body = body.Replace("\r", "%0D%0A");
                if (body.Contains("\n"))
                    body = body.Replace("\n", "%0D%0A");
                LocalEmailHelper.Send(mailTo, subject, body);
            }

            //*process
            else if (funcName == "OpenFile")
            {
                var actArgsStr = "";
                SysProcessHelper.OpenFile(funcParamArray[0], actArgsStr);
            }
            else if (funcName == "OpenFolder")
            {
                SysProcessHelper.OpenFolder(funcParamArray[0]);
            }
            else if (funcName == "OpenUrl")
            {
                SysProcessHelper.OpenUrl(funcParamArray[0]);
            }
            else if (funcName == ("ExecCmds") | funcName == ("ExecCmd") | funcName == ("RunBat") | funcName == ("RunPython"))
            {
                return GetHelper.Run(funcName, funcParamArray);
            }

            else
            {
                return "LRDUNDEFINED";
            }

            return returnStr;
        }


    }
}
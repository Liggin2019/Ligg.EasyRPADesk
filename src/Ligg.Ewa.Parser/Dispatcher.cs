using System;
using System.IO;
using System.Text;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;

namespace Ligg.EasyWinApp.Parser
{
    public static class Dispatcher
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string Act(string funcName, string[] funcParamArray)
        {
            var returnStr = "";

            //##dir
            if (funcName.ToLower() == "CreateDirectory".ToLower())
            {
                if (!Directory.Exists(funcParamArray[0]))
                {
                    Directory.CreateDirectory(funcParamArray[0]);
                }
                if (funcParamArray[1] == "true")
                {
                    var di = new DirectoryInfo(funcParamArray[0]);
                    if (!((di.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                    {
                        di.Attributes = FileAttributes.Hidden;
                    }
                }
            }
            else if (funcName.ToLower() == "OpenDirectory".ToLower())
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

            //##file
            else if (funcName.ToLower() == "SaveContentToTextFile".ToLower())
            {
                var path = funcParamArray[0];
                var ctt = funcParamArray[1];
                var append = false;
                if (funcParamArray.Length > 2)
                {
                    if (funcParamArray[2].ToLower() == "true") append = true;
                }

                FileHelper.SaveContentToTextFile(ctt, path, append);
            }
            else if (funcName.ToLower() == "DeleteFile".ToLower())
            {
                var path = funcParamArray[0];
                if (File.Exists(path))
                    File.Delete(path);
            }
            else if (funcName.ToLower() == "CopyFile".ToLower())
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

            //##process

            else if (funcName.ToLower() == "OpenFile".ToLower())
            {
                var actArgsStr = "";
                SysProcessHelper.OpenFile(funcParamArray[0], actArgsStr);
            }
            else if (funcName.ToLower() == "OpenFolder".ToLower())
            {
                SysProcessHelper.OpenFolder(funcParamArray[0]);
            }
            else if (funcName.ToLower() == ("OpenUrl").ToLower())
            {
                SysProcessHelper.OpenUrl(funcParamArray[0]);
            }
            else if (funcName.ToLower() == "ExecCmd".ToLower())
            {
                var inputStr = funcParamArray[0];
                var execCmdWindowOptionStr = "";
                if (funcParamArray.Length > 1) execCmdWindowOptionStr = funcParamArray[1];
                var execCmdWindowOption = ExecCmdWindowOption.ShowWindow;
                execCmdWindowOption = EnumHelper.GetByName<ExecCmdWindowOption>(execCmdWindowOptionStr, execCmdWindowOption);

                var execCmdOutputOptionStr = "";
                if (funcParamArray.Length > 2) execCmdOutputOptionStr = funcParamArray[2];
                var execCmdOutputOption = ExecCmdOutputOption.None;
                execCmdOutputOption = EnumHelper.GetByName<ExecCmdOutputOption>(execCmdOutputOptionStr, execCmdOutputOption);
                returnStr = SysProcessHelper.ExecCmd(inputStr, execCmdWindowOption, execCmdOutputOption, null);
            }
            else if (funcName.ToLower() == "ExecCmdA".ToLower())
            {
                var inputStr = funcParamArray[0];
                var execCmdWindowOptionStr = "";
                if (funcParamArray.Length > 1) execCmdWindowOptionStr = funcParamArray[1];
                var execCmdWindowOption = ExecCmdWindowOption.ShowWindow;
                execCmdWindowOption = EnumHelper.GetByName<ExecCmdWindowOption>(execCmdWindowOptionStr, execCmdWindowOption);
                var popupOutput = false;
                if (funcParamArray.Length > 2) popupOutput = funcParamArray[2] == "true";

                SysProcessHelper.ExecCmdA(inputStr, execCmdWindowOption, popupOutput, null);
            }
            else if (funcName.ToLower() == "ExecBat".ToLower() | funcName.ToLower() == "ExecBatA".ToLower())
            {
                var path = funcParamArray[0];
                var args = "";
                if (funcParamArray.Length > 1) args = funcParamArray[1];
                var execCmdWindowOptionStr = "";
                if (funcParamArray.Length > 2) execCmdWindowOptionStr = funcParamArray[2];
                var execCmdWindowOption = ExecCmdWindowOption.ShowWindow;
                execCmdWindowOption = EnumHelper.GetByName<ExecCmdWindowOption>(execCmdWindowOptionStr, execCmdWindowOption);
                var popupOutput = false;
                if (funcParamArray.Length > 3) popupOutput = funcParamArray[3] == "true";
                var raiseUacLevel = funcName.ToLower() == "ExecBat".ToLower() ? false : true;

                SysProcessHelper.ExecBat(raiseUacLevel, path, args, execCmdWindowOption, popupOutput, null);
            }
            else if (funcName.ToLower() == "Run".ToLower() | funcName.ToLower() == "RunA".ToLower())
            {
                var path = funcParamArray[0];
                var args = "";
                if (funcParamArray.Length > 1) args = funcParamArray[1];
                var isSync = false;
                if (funcParamArray.Length > 2) isSync = funcParamArray[2] == "true";
                var raiseUacLevel = funcName.ToLower() == "Run".ToLower() ? false : true;

                SysProcessHelper.Run(raiseUacLevel, path, args, isSync, null);
            }
            else if (funcName.ToLower() == "ExecPython".ToLower()| funcName.ToLower() == "ExecPythonA".ToLower())
            {
                var path = funcParamArray[0];
                var args = "";
                if (funcParamArray.Length > 1) args = funcParamArray[1];
                var execCmdWindowOptionStr = "";
                if (funcParamArray.Length > 2) execCmdWindowOptionStr = funcParamArray[2];
                var execCmdWindowOption = ExecCmdWindowOption.ShowWindow;
                execCmdWindowOption = EnumHelper.GetByName<ExecCmdWindowOption>(execCmdWindowOptionStr, execCmdWindowOption);

                var execCmdOutputOptionStr = "";
                if (funcName.ToLower() == "ExecPython".ToLower())
                {
                    if (funcParamArray.Length > 3) execCmdOutputOptionStr = funcParamArray[3];
                    var execCmdOutputOption = ExecCmdOutputOption.None;
                    execCmdOutputOption = EnumHelper.GetByName<ExecCmdOutputOption>(execCmdOutputOptionStr, execCmdOutputOption);
                    returnStr =  SysProcessHelper.ExecPython(path, args, execCmdWindowOption, execCmdOutputOption, null);
                }
                else
                {
                    var popupOutput = false;
                    if (funcParamArray.Length > 3 )popupOutput = funcParamArray[3] == "true";
                    SysProcessHelper.ExecPythonA(path, args, execCmdWindowOption, popupOutput, null);
                }

            }

            else
            {
                return "OutOfScope";
            }

            return returnStr;
        }


    }
}
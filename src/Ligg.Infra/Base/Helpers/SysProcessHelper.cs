using System;
using System.Diagnostics;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;

namespace Ligg.Infrastructure.Base.Helpers
{
    public static class SysProcessHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string PythonExecPath = "";
        private static string _execCmdAFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgCmdA.exe";
        private static string _execCmdBFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgCmdB.exe";
        private static string _execXcmdFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgXcmd.exe";
        private static string _execXcmdAFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgXcmdA.exe";
        private static string _execXcmdBFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgXcmdB.exe";
        private static string _startBatPath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\Start.bat";
        public static string ExecCmd(string inputStr, ExecCmdWindowOption execCmdOption, ExecCmdOutputOption outputOption, ProcessAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(inputStr)) return "";

            var process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            //process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            //process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//does not work, the windows shown or hidden depends on  "process.StartInfo.CreateNoWindow"

            var execCmdMode = ExecCmdMode.AsyncWindow;
            if (execCmdOption == ExecCmdWindowOption.ShowWindow) execCmdMode = ExecCmdMode.AsyncWindow;
            else if (execCmdOption == ExecCmdWindowOption.NoWindow & outputOption == ExecCmdOutputOption.None) execCmdMode = ExecCmdMode.NoWindow;
            else if (execCmdOption == ExecCmdWindowOption.NoWindow & outputOption != ExecCmdOutputOption.None) execCmdMode = ExecCmdMode.NoWindowWithOutput;
            else if (execCmdOption == ExecCmdWindowOption.BlankWindow & outputOption == ExecCmdOutputOption.None) execCmdMode = ExecCmdMode.BlankWindow;
            else if (execCmdOption == ExecCmdWindowOption.BlankWindow & outputOption != ExecCmdOutputOption.None) execCmdMode = ExecCmdMode.BlankWindowWithOutput;

            var returnOutput = false;
            if (execCmdMode == ExecCmdMode.AsyncWindow)
            {
                inputStr = "start " + _startBatPath + " " + inputStr;
                process.StartInfo.CreateNoWindow = true;
                returnOutput = false;

            }
            else if (execCmdMode == ExecCmdMode.SyncWindow)
            {
                inputStr = "start " + _startBatPath + " " + inputStr;
                process.StartInfo.CreateNoWindow = true;
                returnOutput = true;
            }
            else if (execCmdMode == ExecCmdMode.NoWindow)
            {
                process.StartInfo.CreateNoWindow = true;
                returnOutput = false;
            }
            else if (execCmdMode == ExecCmdMode.NoWindowWithOutput)
            {
                process.StartInfo.CreateNoWindow = true;
                returnOutput = true;
            }
            else if (execCmdMode == ExecCmdMode.BlankWindow)
            {
                process.StartInfo.CreateNoWindow = false;
                //when CreateNoWindow = false, popup a empty cmd window, no any exec result appears. 
                returnOutput = false;
            }
            else if (execCmdMode == ExecCmdMode.BlankWindowWithOutput)
            {
                process.StartInfo.CreateNoWindow = false;
                returnOutput = true;
            }
            if (processAccountInfo != null)
            {
                process.StartInfo.Domain = processAccountInfo.Domain;
                process.StartInfo.UserName = processAccountInfo.UserName;
                var pw = new System.Security.SecureString();
                foreach (var v in processAccountInfo.Password.ToCharArray())
                {
                    pw.AppendChar(v);
                }
                process.StartInfo.Password = pw;
            }
            try
            {
                process.Start();
                //process.StandardInput.WriteLine("cd " + @"C:\windows\system32");
                process.StandardInput.WriteLine(inputStr);
                process.StandardInput.WriteLine("exit");
                var outputStr = "";
                if (returnOutput)
                {
                    outputStr = process.StandardOutput.ReadToEnd();//When use 'start' to start a real new window, the  Output is on the new window, not this empty window. so if you want a return output , don't use 'start'
                }
                process.WaitForExit();
                process.Close();
                return outputStr;
            }

            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".ExecCmd Error: " + ex.Message);
            }
        }
        public static void ExecCmdA(string inputStr, ExecCmdWindowOption execCmdWindowOption, bool popupOutput, ProcessAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(inputStr)) return;
            inputStr = popupOutput ? _execCmdAFilePath + " /k " + inputStr : _execCmdAFilePath + " " + inputStr;
            ExecCmd(inputStr, execCmdWindowOption, ExecCmdOutputOption.None, processAccountInfo);

        }

        public static void ExecBat(bool raiseUacLevel, string path, string args, ExecCmdWindowOption execCmdWindowOption, bool popupOutput, ProcessAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("File: '" + path + "' does not exist!");
            var inputStr = path + " " + args;
            var execFilePath = raiseUacLevel ? _execXcmdAFilePath : _execXcmdFilePath;
            inputStr = popupOutput ? execFilePath + " /k " + inputStr : execFilePath + " " + inputStr;
            ExecCmd(inputStr, execCmdWindowOption, ExecCmdOutputOption.None, processAccountInfo);
        }

        public static void Run(bool raiseUacLevel, string path, string args, bool isSync, ProcessAccountInfo processAccountInfo)
        {

            var process = new Process();
            if (!System.IO.File.Exists(path)) throw new ArgumentException("File: '" + path + "' does not exist!");

            var path1 = raiseUacLevel ? _execCmdAFilePath : path;
            process.StartInfo.FileName = path1;
            var args1 = raiseUacLevel ? path + " " + args : args;
            if (!string.IsNullOrEmpty(args1))
            {
                process.StartInfo.Arguments = args1;//to "explorer.exe", args="D:\\Readme.txt"; to "iexplore.exe",args=http://www.baidu.com
            }
            if (raiseUacLevel) process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            if (processAccountInfo != null)
            {
                process.StartInfo.Domain = processAccountInfo.Domain;
                process.StartInfo.UserName = processAccountInfo.UserName;
                var pw = new System.Security.SecureString();
                foreach (var v in processAccountInfo.Password.ToCharArray())
                {
                    pw.AppendChar(v);
                }
                process.StartInfo.Password = pw;
            }
            try
            {
                process.Start();
                if (isSync)
                {
                    process.WaitForExit();
                    process.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".Run Error: " + ex.Message);
            }
        }
        public static string ExecPython(string path, string args, ExecCmdWindowOption execCmdOption, ExecCmdOutputOption outputOption, ProcessAccountInfo processAccountInfo)
        {
            try
            {
                if (!FileHelper.IsFileExisting(PythonExecPath)) throw new ArgumentException("Python exec file does not exist!");
                if (!System.IO.File.Exists(path)) throw new ArgumentException("File: '" + path + "' does not exist!");
                var inputStr = PythonExecPath + " " + path + " " + args;
                return ExecCmd(inputStr, execCmdOption, outputOption, processAccountInfo);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".RunPython Error: " + ex.Message);
            }
        }
        public static void ExecPythonA(string path, string args, ExecCmdWindowOption execCmdOption, bool popupOutput, ProcessAccountInfo processAccountInfo)
        {
            try
            {
                if (!FileHelper.IsFileExisting(PythonExecPath)) throw new ArgumentException("Python exec file does not exist!");
                if (!System.IO.File.Exists(path)) throw new ArgumentException("File: '" + path + "' does not exist!");
                var inputStr = PythonExecPath + " " + path + " " + args;
                ExecCmdA(inputStr, execCmdOption, popupOutput, processAccountInfo);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".RunPython Error: " + ex.Message);
            }
        }

        public static void OpenFolder(string dir)
        {
            try
            {
                var process = new Process();
                if (System.IO.Directory.Exists(dir))
                {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = "Explorer.exe";
                    process.StartInfo.Arguments = dir;
                    process.Start();
                }
                else
                {
                    throw new ArgumentException("File: '" + dir + "' does not exist!");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".OpenFolder Error: " + ex.Message);
            }
        }

        public static void OpenFile(string path, string args)
        {
            try
            {
                var process = new Process();
                if (System.IO.File.Exists(path))
                {

                    process.StartInfo.FileName = path;
                    if (!string.IsNullOrEmpty(args))
                    {
                        process.StartInfo.Arguments = args;
                    }
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
                else
                {
                    throw new ArgumentException("File: '" + path + "' does not exist!");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".OpenFile Error: " + ex.Message);
            }
        }

        public static void OpenUrl(string url)
        {
            try
            {
                var process = new Process();
                process.SynchronizingObject = null;
                process.StartInfo.LoadUserProfile = false;
                process.StartInfo.StandardErrorEncoding = null;
                process.StartInfo.StandardOutputEncoding = null;
                process.StartInfo.FileName = url;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }

            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".OpenUrl Error: " + ex.Message);
            }
        }

        private enum ExecCmdMode
        {
            //to 0, 1:  popup a window, can see exec result(after one cmd finishs exce) till closing the window manually(pause);
            //          can not get exec result at calling location 
            AsyncWindow = 10,  //async;
            SyncWindow = 11,//sync;

            //to 2, 3:  popup no window;
            //          not fit for endless cmd, e.g., ping x.x.x.x -t
            NoWindow = 20,//sync; 
            NoWindowWithOutput = 21,//sync; can get exec result at calling location except for cmda

            //to 4, 5:  popup a empty window, can not see exec result;
            BlankWindow = 30,//sync; 
            BlankWindowWithOutput = 31,//syn; can get exec result at calling location
        }
    }



    public enum ExecCmdWindowOption
    {
        ShowWindow = 10,
        NoWindow = 20,
        BlankWindow = 30,
    }

    public enum ExecCmdOutputOption
    {
        None = 0,
        Pop = 10,
        Save = 11,
    }


    public class ProcessAccountInfo
    {
        public string Domain;
        public string UserName;
        public string Password;
    }
}

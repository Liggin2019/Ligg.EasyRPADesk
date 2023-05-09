using System;
using System.Diagnostics;
using  Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using System.IO;

namespace Ligg.Infrastructure.Helpers
{
    public static class SysProcessHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string PythonExecPath = "";
        public static string TmpPath = "";
        private static string _execCmdUFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgCmdU.exe";
        private static string _execXcmdFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgXcmd.exe";
        private static string _execXcmdUFilePath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\LgXcmdU.exe";
        private static string _startBatPath = FileHelper.GetFileDetailByOption(Process.GetCurrentProcess().MainModule.FileName, FilePathComposition.Directory) + "\\Resources\\Start.bat";

        public static string ExecCmd(bool raiseUacLevel, string inputStr, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptOutput, WinAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(inputStr)) return "";
            var inputStr1 = inputStr.Trim();
            inputStr1 = args.IsNullOrEmpty() ? inputStr1 : inputStr1 + " " + args;
            if (raiseUacLevel)
            {
                inputStr1 = execCmdWindowOption == ExecCmdWindowOption.ShowWindow ? _execCmdUFilePath + " /k " + inputStr1 : _execCmdUFilePath + " " + inputStr1;
                return ExecCommand(inputStr1, execCmdWindowOption, interceptOutput, processAccountInfo);
            }
            else return ExecCommand(inputStr1, execCmdWindowOption, interceptOutput, processAccountInfo);
        }

        public static string ExecCmds(bool raiseUacLevel, string inputStr, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptOutput, WinAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(inputStr)) return "";
            var path = CreateBatFile(inputStr);
            return RunBat(raiseUacLevel, path, args, execCmdWindowOption, interceptOutput, processAccountInfo);
        }


        public static string RunBat(bool raiseUacLevel, string path, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptOutput, WinAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("File path can't be empty!");
            if (!File.Exists(path)) throw new ArgumentException("File: '" + path + "' does not exist!");
            var inputStr = args.IsNullOrEmpty() ? path : path + " " + args;
            var execFilePath = raiseUacLevel ? _execXcmdUFilePath : _execXcmdFilePath;
            inputStr = execCmdWindowOption == ExecCmdWindowOption.ShowWindow ? execFilePath + " /k " + inputStr : execFilePath + " " + inputStr;
            return ExecCommand(inputStr, execCmdWindowOption, interceptOutput, processAccountInfo);
        }

        public static string RunPython(bool raiseUacLevel, string path, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptOutput, WinAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(PythonExecPath)) throw new ArgumentException("Python exec file path can't be empty!");
            if (!File.Exists(PythonExecPath)) throw new ArgumentException("Python exec file does not exist!");
            FileHelper.CheckPathExistence(path);
            var inputStr = args.IsNullOrEmpty() ? PythonExecPath + " " + path : PythonExecPath + " " + path + " " + args;
            if (raiseUacLevel)
            {
                inputStr = execCmdWindowOption == ExecCmdWindowOption.ShowWindow ? _execCmdUFilePath + " /k " + inputStr : _execCmdUFilePath + " " + inputStr;
            }
            return ExecCommand(inputStr, execCmdWindowOption, interceptOutput, processAccountInfo);
        }


        public static void OpenFolder(string dir)
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
                throw new ArgumentException("Directory: '" + dir + "' does not exist!");
            }
        }

        public static void OpenFile(string path, string args)
        {
            var process = new Process();
            if (System.IO.File.Exists(path))
            {

                process.StartInfo.FileName = path; /// path="D:\\Readme.txt" to explorer.exe; path="http://www.baidu.com" to iexplore.exe, 找文件缺省执行器
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

        public static void OpenUrl(string url)
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

        //*private
        private static string ExecCommand(string inputStr, ExecCmdWindowOption execCmdWindowOption, bool interceptOutput, WinAccountInfo processAccountInfo)
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

            if (execCmdWindowOption == ExecCmdWindowOption.ShowWindow)
            {
                inputStr = "start " + _startBatPath + " " + inputStr;
                process.StartInfo.CreateNoWindow = true;
            }
            else if (execCmdWindowOption == ExecCmdWindowOption.NoWindow)
            {
                process.StartInfo.CreateNoWindow = true;
            }
            else if (execCmdWindowOption == ExecCmdWindowOption.BlankWindow)
            {
                process.StartInfo.CreateNoWindow = false;
                //when CreateNoWindow = false, popup a empty cmd window, no any exec result appears. 
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
                if (interceptOutput)
                {
                    outputStr = process.StandardOutput.ReadToEnd();//When use 'start' to start a real new window, the  Output is on the old window, not this new window. so if you want a return output , don't use 'start'
                }
                process.WaitForExit();
                process.Close();
                return outputStr;
            }

            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".ExecCmd Error: " + ex.Message);
            }
        }

        private static string ExecCommandA(string inputStr, ExecCmdWindowOption execCmdWindowOption, bool interceptOutput, WinAccountInfo processAccountInfo)
        {
            if (string.IsNullOrEmpty(inputStr)) return "";
            inputStr = execCmdWindowOption == ExecCmdWindowOption.ShowWindow ? _execCmdUFilePath + " /k " + inputStr : _execCmdUFilePath + " " + inputStr;
            return ExecCommand(inputStr, execCmdWindowOption, interceptOutput, processAccountInfo);

        }

        private static string CreateBatFile(string inputStr)
        {

            var inputArry = inputStr.SplitByChar('\n', true, true);
            var content = StringArrayExtension.Unwrap(inputArry, "\n");
            var title = "ExecCmd".ToUniqueStringByNow("-");

            var outputDir = TmpPath + "\\ProcessLog\\Bats";
            DirectoryHelper.CreateDirectory(outputDir);
            var savePath = outputDir + "\\" + title + ".bat";
            File.WriteAllText(savePath, content);
            return savePath;
        }

        //for exe 带界面程序, seems no use
        private static void Run(bool raiseUacLevel, string path, string args, bool interceptOutput, WinAccountInfo processAccountInfo)
        {

            var process = new Process();
            if (!System.IO.File.Exists(path)) throw new ArgumentException("File: '" + path + "' does not exist!");

            var path1 = raiseUacLevel ? _execCmdUFilePath : path;
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
                if (interceptOutput)
                {
                    process.WaitForExit();
                    process.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".Run Error: " + ex.Message);
            }
        }


    }

    public enum ExecCmdWindowOption
    {
        ShowWindow = 10,
        NoWindow = 20,
        BlankWindow = 30,
    }

    /*
    private enum ExecCmdMode //old time
    {
        //to 10, 11:  popup a window, can see exec result(after one cmd finishs exce) till closing the window manually(pause);
        //            can not get exec result at calling location 
        AsyncWindow = 10,  //async; ShowWindow+no interceptOutput
        SyncWindow = 11,//sync; ShowWindow+interceptOutput; output is only the output of start.exe, useless

        //to 20, 21:  popup no window;
        //            not fit for endless cmd, e.g., ping x.x.x.x -t
        NoWindow = 20,//sync; 
        NoWindowWithOutput = 21,//sync; NoWindow+interceptOutput, can get exec result at calling location except for cmda

        //to 30, 31:  popup a empty window, can not see exec result;  fit for endless cmd, e.g., ping x.x.x.x -t
        BlankWindow = 30,//sync; 
        BlankWindowWithOutput = 31,//syn; BlankWindow+interceptOutput, can get exec result at calling location
    }*/


}

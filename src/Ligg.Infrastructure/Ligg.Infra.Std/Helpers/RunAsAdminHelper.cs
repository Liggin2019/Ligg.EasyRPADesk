using System;
using Ligg.Infrastructure.Extensions;
using  Ligg.Infrastructure.DataModels;

namespace Ligg.Infrastructure.Helpers
{
    public static class RunAsAdminHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static bool IsCurrentAccountAdmin = true;

        public static WinAccountInfo ProcessAccountInfo
        {
            get;
            set;
        }
        public static string ExecCmds(bool raiseUacLevel, string inputStr, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptoutput)
        {

            if (IsCurrentAccountAdmin) return SysProcessHelper.ExecCmds(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptoutput, null);
            else
            {
                CheckProcessAccountInfo();
                return SysProcessHelper.ExecCmds(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptoutput, ProcessAccountInfo);
            }
        }
        public static string ExecCmd(bool raiseUacLevel, string inputStr, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptoutput)
        {

            if (IsCurrentAccountAdmin) return SysProcessHelper.ExecCmd(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptoutput, null);
            else
            {
                CheckProcessAccountInfo();
                return SysProcessHelper.ExecCmd(raiseUacLevel, inputStr, args, execCmdWindowOption, interceptoutput, ProcessAccountInfo);
            }
        }

        public static string RunBat(bool raiseUacLevel, string path, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptoutput)
        {
            if (IsCurrentAccountAdmin) return SysProcessHelper.RunBat(raiseUacLevel, path, args, execCmdWindowOption, interceptoutput, null);
            else
            {
                CheckProcessAccountInfo();
                return SysProcessHelper.RunBat(raiseUacLevel, path, args, execCmdWindowOption, interceptoutput, ProcessAccountInfo);
            }
        }


        public static string RunPython(bool raiseUacLevel, string path, string args, ExecCmdWindowOption execCmdWindowOption, bool interceptoutput)
        {
            if (IsCurrentAccountAdmin) return SysProcessHelper.RunPython(raiseUacLevel, path, args, execCmdWindowOption, interceptoutput, null);
            else
            {
                CheckProcessAccountInfo();
                return SysProcessHelper.RunPython(raiseUacLevel, path, args, execCmdWindowOption, interceptoutput, ProcessAccountInfo);
            }
        }


        private static void CheckProcessAccountInfo()
        {
            if (ProcessAccountInfo == null) throw new ArgumentException(_typeFullName + ".CheckProcessAccountInfo Error: "+ "ProcessAccountInfo can't be null!");
            if (ProcessAccountInfo.UserName.IsNullOrEmpty()) throw new ArgumentException(_typeFullName + ".CheckProcessAccountInfo Error: " +  "RunAsAdminAccount can't be empty!");
        }

    }
}

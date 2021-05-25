using Ligg.EasyWinApp.Interface;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Utility.Ioc;

using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;
using Ligg.EasyWinApp.Parser.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;


using Ligg.Infrastructure.Base.Extension;

namespace Ligg.EasyWinApp
{
    public static class BootStrapper
    {
        private static List<string> _bootStrapperTaskClassFullNames = new List<string>();
        internal static List<BootStrapperTask> BootStrapperTaskList = new List<BootStrapperTask>();

        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public static ICblpAdapter Adapter = null;
        public static void Init(ApplicationSetting applicationStartParamSet)
        {
            try
            {
                if (!applicationStartParamSet.RunBootStrapperTasksAtStart & !applicationStartParamSet.HasCblpComponent)
                {
                    return;
                }

                var bootStrapperTasksDllPath = "";
                var cblpDllPath = "";
                var adapterClassFullName = "";
                Ioc.InitializeWith(new DependencyResolverFactory("Ligg.Infrastructure.Utility.Ioc.AutofacContainer"));
                if (applicationStartParamSet.RunBootStrapperTasksAtStart)
                {
                    var bootStrapperTasksNames = applicationStartParamSet.BootStrapperTasks;
                    bootStrapperTasksDllPath = applicationStartParamSet.BootStrapperTasksDllPath;
                    if (!string.IsNullOrEmpty(bootStrapperTasksNames))
                    {
                        var tmpArry = bootStrapperTasksNames.GetSubParamArray(true, true);
                        foreach (var tmpStr in tmpArry)
                        {
                            var tmpStr1 = tmpStr.Trim();
                            var bootStrapperClassFullName = applicationStartParamSet.BootStrapperTaskNameSpace + "." + tmpStr1;
                            Ioc.Register(bootStrapperTasksDllPath, bootStrapperClassFullName);
                            _bootStrapperTaskClassFullNames.Add(bootStrapperClassFullName);
                        }
                    }
                }
                if (applicationStartParamSet.HasCblpComponent)
                {
                    cblpDllPath = applicationStartParamSet.CblpDllPath;
                    adapterClassFullName = applicationStartParamSet.CblpAdapterClassFullName;
                    if (!cblpDllPath.IsNullOrEmpty())
                    {
                        Ioc.Register(cblpDllPath, adapterClassFullName);
                    }
                }

                Ioc.SetContainer();

                if (applicationStartParamSet.RunBootStrapperTasksAtStart)
                {
                    if (_bootStrapperTaskClassFullNames.Count > 0)
                    {

                        foreach (var bootStrapperTaskClassFullName in _bootStrapperTaskClassFullNames)
                        {
                            var obj = (IBootStrapperTask)Ioc.Resolve(bootStrapperTasksDllPath, bootStrapperTaskClassFullName);
                            var bootStrapperTask = new BootStrapperTask(obj, bootStrapperTaskClassFullName.GetLastSeparatedString('.'));
                            BootStrapperTaskList.Add(bootStrapperTask);

                        }
                    }
                }

                if (applicationStartParamSet.HasCblpComponent)
                {
                    if (File.Exists(cblpDllPath))
                    {
                        if (!FileHelper.IsFileExisting(cblpDllPath)) return;
                        Adapter = CreateAdapter(cblpDllPath, adapterClassFullName);
                        Adapter?.Initialize();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".Init Error: " + ex.Message);
            }
        }



        public static bool ExecTask(BootStrapperTask bootStrapperTask)
        {
            try
            {

                if (!bootStrapperTask.Task.ExecuteThenJudge())
                {
                    var msg = "Execute " + bootStrapperTask.Name + " Failed" + "!";
                    Console.WriteLine(msg);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                var msg = "Execute " + bootStrapperTask.Name + " Failed" + "!";
                Console.WriteLine(msg);
                return false;
            }

        }

        private static ICblpAdapter CreateAdapter(string cblpDllPath, string adapterClassFullName)
        {
            try
            {
                var adapterObj = Ioc.Resolve(cblpDllPath, adapterClassFullName);

                string key = adapterClassFullName;//namespaceDotClassName;
                var objType = AssemblyHelper.GetCache(key) as ICblpAdapter;
                if (objType == null)
                {
                    objType = (ICblpAdapter)adapterObj;
                    AssemblyHelper.SetCache(key, objType);
                }
                return objType;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".CreateAdapter Error: " + ex.Message);
            }
        }

        // also you can use CreateAdapter1 to realize IOC by reflection --by Liggin2019 at 200319
        private static ICblpAdapter CreateAdapter1(string cblpDllPath, string adapterClassFullName)
        {
            try
            {

                string key = adapterClassFullName;//namespaceDotClassName;
                var objType = AssemblyHelper.GetCache(key) as ICblpAdapter;
                if (objType == null)
                {
                    objType = (ICblpAdapter)AssemblyHelper.CreateObject(cblpDllPath, adapterClassFullName);
                    AssemblyHelper.SetCache(key, objType);
                }
                return objType;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".CreateAdapter Error: " + ex.Message);
            }
        }


    }


}
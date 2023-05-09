using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.Infrastructure.Utilities.Ioc;
using Ligg.RpaDesk.DataModels;
using Ligg.RpaDesk.Interface;
using System;
using System.IO;
using System.Diagnostics;


namespace Ligg.RpaDesk
{
    internal static class ComponentHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        internal static IStdServiceComponentAdapter CblpComponentAdapter = null;
        internal static void Initialize(ApplicationSettingEx applicationSetting, string appCode)
        {
            var startupDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var dir = DirectoryHelper.GetParent(startupDir, 2);
            dir = dir + "\\Components";
            var namespace1 = "Ligg.RpaDesk.StandardServiceComponent";
            if (applicationSetting.HasCblpComponent)
            {
                var dllPath = dir + "\\" + appCode + "\\Adapter.dll";
                var classFullName = namespace1 + "." + "Adapter";

                CblpComponentAdapter = ComponentHelper.CreateCblpcAdapter(dllPath, classFullName);
            }

        }


        private static IStdServiceComponentAdapter CreateCblpcAdapter(string cblpDllPath, string adapterClassFullName)
        {
            try
            {

                string key = adapterClassFullName;//namespaceDotClassName;
                var objType = AssemblyHelper.GetCache(key) as IStdServiceComponentAdapter;
                if (objType == null)
                {
                    objType = (IStdServiceComponentAdapter)AssemblyHelper.CreateObject(cblpDllPath, adapterClassFullName);
                    AssemblyHelper.SetCache(key, objType);
                }
                if (objType == null) throw new ArgumentException("CblpcAdapter can't be null, cblpDllPath= " + cblpDllPath + ", adapterClassFullName= " + adapterClassFullName);
                return objType;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + _typeFullName + ".CreateCblpcAdapter Error: " + ex.Message);
            }
        }

    }

}
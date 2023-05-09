using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Resources;
using Ligg.RpaDesk.WinCnsl.Helpers;
using System;
using System.Reflection;

namespace Ligg.RpaDesk
{
    internal class AdapterHandler
    {
        private static readonly string _typeFullName = MethodBase.GetCurrentMethod().ReflectedType.FullName;
        private IStdServiceComponentAdapter _adaper;
        internal AdapterHandler()
        {
            _adaper = ComponentHelper.CblpComponentAdapter;
        }
        internal AdapterHandler(string code)
        {
        }
        internal  string Dispatch(string component, string service, string method, string function, string[] paramArr)
        {
            var adapter = _adaper;
            if (component=="Cblpc")
            {
                if (_adaper == null)
                {
                    var msg = "\n>> "+ _typeFullName + ".Dispatch Error: component= " + component + " does not exist! ";
                    throw new ArgumentException(msg);  
                }
            }

            try
            {
                return adapter.Dispatch(service, method, function, paramArr);
            }
            catch (Exception ex)
            {
                var msg = "\n>> " + _typeFullName + ".Dispatch Error: " + component + "." + service + "." + "." + method + "." + function + " " + TextRes.Failed + "!" + ex.Message;
                throw new ArgumentException(msg);
            }
        }

        public  void Initialize(string component, string[] paramArr)
        {
            if (_adaper == null)
            {
                var msg = "\n>> " + _typeFullName + ".Initialize Error: component= " + component + " does not exist! ";
                throw new ArgumentException(msg);
            }
            try
            {
                _adaper.Initialize(paramArr);
            }
            catch (Exception ex)
            {
                var msg = "\n>> " + _typeFullName + ".Initialize Error: " + component + ".Initialize " + TextRes.Failed + "; " +ex.Message;
                throw new ArgumentException(msg);
            }
        }
        public  string ResolveConstants(string text)
        {
            if (_adaper == null)
            {
                var msg = "\n>> " + _typeFullName + ".ResolveConstants Error: CblpComponent does not exist! ";
                throw new ArgumentException(msg);
            }
            try
            {
                return _adaper.ResolveConstants(text);
            }
            catch (Exception ex)
            {
                var msg = "\n>> " + GetType().FullName + ".ResolveConstants Error: text= " +text +" "+ ex.Message;
                throw new ArgumentException(msg);
            }
        }
        public UniversalResult Validate(string code,string rule)
        {
            if (_adaper == null)
            {
                var msg = "\n>> " + _typeFullName + ".ResolveConstants Error: CblpComponent does not exist! ";
                throw new ArgumentException(msg);
            }
            try
            {
                return _adaper.Validate(code, rule);
            }
            catch (Exception ex)
            {
                var msg = "\n>> " + GetType().FullName + ".Validate error: rule= "+rule+" " + ex.Message;
                throw new ArgumentException(msg);
            }
        }


    }

}
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;


namespace Ligg.Infrastructure.Helpers
{
    public static class EnumHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static string[] GetNames<T>()
        {
            var exInfo = _typeFullName + ".GetNames Error: ";
            Type enumType = typeof(T);

            // can't use type constraints on Value types, so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException(exInfo + "EnumHelper.T must be of type System.Enum");

            var names = new List<string>();
            foreach (var v in Enum.GetNames(enumType))
            {
                names.Add((string)v);
            }
            return names.ToArray();
        }


        public static T GetById<T>(int id, T defaultValue)
        {
            Type enumType = typeof(T);
            var vals = Enum.GetValues(enumType);
            var name = "";
            foreach (var v in vals)
            {
                if ((int)v == id)
                    name = v.ToString();
                continue;
            }

            return GetByName<T>(name, defaultValue);
        }

        public static T GetByName<T>(string name, T defaultValue)
        {
            Type enumType = typeof(T);
            var vals = Enum.GetValues(enumType) as IEnumerable<T>;
            foreach (var v in vals)
            {
                if (name == v.ToString())
                    return v;
            }
            return defaultValue;
        }

        public static int GetIdByName<T>(string name)
        {
            var exInfo = _typeFullName + ".GetIdByName Error: ";
            if (name.IsNullOrEmpty()) return 0;
            Type enumType = typeof(T);
            int returnVal = 0;
            var isOk = false;
            foreach (var v in Enum.GetValues(enumType))
            {
                var id = (int)v;
                var name1 = v.ToString();
                if (name1 == name)
                {
                    returnVal = id;
                    isOk = true;
                    break;
                }
            }
            if (!isOk)
            {
                var nameArry = GetNames<T>();
                var names = StringArrayExtension.Unwrap(nameArry, ",");
                throw new ArgumentException(exInfo + "Enum Name: \"" + name + "\" mismatches enum type " + enumType + "'s all names: \"" + names + "\" !");
            }
            return returnVal;
        }



        //*judge
        public static bool IsNameValid<T>(string name)
        {
            Type enumType = typeof(T);
            foreach (var v in Enum.GetValues(enumType))
            {
                var name1 = v.ToString();
                if (name1 == name)
                {
                    return true;
                }
            }
            return false;

        }

    }
}
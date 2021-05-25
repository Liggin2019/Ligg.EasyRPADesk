using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Diagnostics;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.Extension;


namespace Ligg.Infrastructure.Base.Helpers
{
    public static class EnumHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static List<IntIdName> EnumToIdNames<T>()
        {
            var exInfo = "\n>> " + TypeName + ".EnumToIdNames Error: ";
            Type enumType = typeof(T);
            // Can't use type constraints on Value types, so have to do check like this

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException(exInfo + "EnumHelper.T must be of type System.Enum");
            var idNames = new List<IntIdName>();
            foreach (var v in Enum.GetValues(enumType))
            {
                var idName = new IntIdName();
                idName.Id = (int)v;
                idName.Name = v.ToString();
                idNames.Add(idName);
            }
            return idNames;
        }

        public static string GetNameById<T>(int id)
        {
            Type enumType = typeof(T);
            string name = "";
            name = Enum.GetName(enumType, id);
            return name;
        }

        public static int GetIdByName<T>(string name)
        {
            var exInfo = "\n>> " + TypeName + ".GetIdByName Error: ";
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
                var names = StringHelper.UnwrapStringArrayBySeparator(nameArry, ',');
                throw new ArgumentException(exInfo + "Enum Name: \"" + name + "\" mismatches enum type " + enumType + "'s all names: \"" + names + "\" !");
            }
            return returnVal;
        }

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

        private static string[] GetNames<T>()
        {
            var exInfo = "\n>> " + TypeName + ".GetNames Error: ";
            Type enumType = typeof(T);
            // Can't use type constraints on Value types, so have to do check like this

            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException(exInfo + "EnumHelper.T must be of type System.Enum");
            var idNames = new List<IntIdName>();
            foreach (var v in Enum.GetValues(enumType))
            {
                var idName = new IntIdName();
                idName.Id = (int)v;
                idName.Name = v.ToString();
                idNames.Add(idName);
            }
            var names = new string[idNames.Count];
            var i = 0;
            foreach (var idName in idNames)
            {
                names[i] = idName.Name;
                i++;
            }

            return names;
        }

        private static List<T> EnumToList<T>()
        {
            var exInfo = "\n>> " + TypeName + ".EnumToIdNames Error: ";
            Type enumType = typeof(T);

            // Can't use type constraints on Value types, so have to do check like this

            if (enumType.BaseType != typeof(Enum))

                throw new ArgumentException(exInfo + "EnumHelper.T must be of type System.Enum");

            return new List<T>(Enum.GetValues(enumType) as IEnumerable<T>);

        }



        private static List<ValueText> EnumToValueTexts<T>()
        {
            var exInfo = "\n>> " + TypeName + ".EnumToValueTexts Error: ";

            Type enumType = typeof(T);
            // Can't use type constraints on Value types, so have to do check like this

            if (enumType.BaseType != typeof(Enum))

                throw new ArgumentException(exInfo + "EnumHelper.T must be of type System.Enum");
            var valueTexts = new List<ValueText>();
            foreach (var v in Enum.GetValues(enumType))
            {
                var valueText = new ValueText();
                valueText.Value = Convert.ToString((int)v);
                valueText.Text = v.ToString();
                valueTexts.Add(valueText);
            }
            return valueTexts;
        }

        private static DataTable EnumToDataTable<T>()
        {
            var exInfo = "\n>> " + TypeName + ".EnumToDataTable Error: ";
            Type enumType = typeof(T);
            if (enumType.BaseType != typeof(Enum))

                throw new ArgumentException(exInfo + "EnumHelper.T must be of type System.Enum");
            DataTable dt = new DataTable();
            dt.Columns.Add("Value");
            dt.Columns.Add("Text");
            foreach (var v in Enum.GetValues(enumType))
            {
                dt.Rows.Add(Convert.ToString((int)v), v.ToString());
            }
            return dt;
        }



    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Ligg.Infrastructure.Base.DataModel;

namespace Ligg.Infrastructure.Base.Helpers
{
    public class Culture
    {
        public string Name
        {
            get;
            set;
        }
        public string LanguageCode
        {
            get;
            set;
        }

        public string LanguageName
        {
            get;
            set;
        }

        public string ImageUrl
        {
            get;
            set;
        }

        //*no use yet
        public int TimeZone
        {
            get;
            set;
        }

        public bool IsDefault
        {
            get;
            set;
        }

        public bool IsCurrent
        {
            get;
            set;
        }
    }

    public static class CultureHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public static List<Culture> Cultures = new List<Culture>();

        //#property
        public static CultureInfo CurrentCulture
        {
            get
            {
                return CultureInfo.CurrentCulture;
            }
        }


        public static string CurrentCultureName
        {
            get
            {
                return Cultures.Find(x => x.IsCurrent) != null ? Cultures.Find(x => x.IsCurrent).Name : "en-US";
            }
        }

        public static string CurrentLanguageName
        {
            get
            {
                return Cultures.Find(x => x.IsCurrent) != null ? Cultures.Find(x => x.IsCurrent).LanguageName : "English";
            }
        }

        public static string DefaultLanguageCode
        {
            get
            {
                return Cultures.Find(x => x.IsDefault) != null ? Cultures.Find(x => x.IsDefault).LanguageCode : "eng";
            }
        }

        public static string CurrentLanguageCode
        {
            get
            {
                return Cultures.Find(x => x.IsCurrent) != null ? Cultures.Find(x => x.IsCurrent).LanguageCode : "eng";
            }
        }

        public static string FirstLanguageCode
        {
            get
            {
                return Cultures.First().LanguageCode;
            }
        }


        //#set
        public static void SetCurrentCulture(string cultureName)
        {

            foreach (var cul in Cultures)
            {
                cul.IsCurrent = false;
            }
            var curCul = Cultures.Find(x => x.Name == cultureName);
            if (curCul == null) throw new ArgumentException("\n>> " + TypeName + ".SetCurrentCulture Error: " + "Culture:" + cultureName + " does not exsit!");
            else
            {
                curCul.IsCurrent = true;
                var culture = new CultureInfo(cultureName);
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        //#get
        public static string GetLanguageCodeByCultureName(string cultureName)
        {

            var cul = Cultures.Find(x => x.Name == cultureName);
            if (cul != null)
            {
                return cul.LanguageCode;
            }
            else return string.Empty;
        }

        //check
        public static bool IsCultureNameValid(string cultureName)
        {

            var cul = Cultures.Find(x => x.Name == cultureName);
            if (cul != null)
            {
                return true;
            }
            return false;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ligg.Infrastructure.Helpers
{
    public class Language
    {
        public string Id
        {
            get;
            set;
        }
        public string CultureName
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

        public bool IsCurrent
        {
            get;
            set;
        }

        public bool IsDefault
        {
            get;
            set;
        }
    }

    public static class LanguageHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public static List<Language> Languages = new List<Language>();

        //*prop
        public static CultureInfo CurrentCulture
        {
            get
            {
                return CultureInfo.CurrentCulture;
            }
        }

        public static string CurrentId
        {
            get
            {
                var lang = Languages.Find(x => x.IsCurrent);
                return lang != null ? lang.Id : "";
            }
        }

        public static string CurrentCultureName
        {
            get
            {
                var lang = Languages.Find(x => x.IsCurrent);
                return lang != null ? lang.CultureName : "";
            }
        }

        public static string CurrentImageUrl
        {
            get
            {
                var lang = Languages.Find(x => x.IsCurrent);
                return lang != null ? lang.ImageUrl : "";

                var inputStr = "";
                float float1 = 3.02f;
                DateTime now = DateTime.Now;
                var singleOutputStr = 3.14f.ToString();
                var timeOutputStr = new DateTime(2021, 10, 27).ToShortDateString();
                var timeOutputStr2 = new DateTime(2021, 10, 27).ToString(CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern, CultureInfo.InvariantCulture);

            }
        }

        public static string CurrentLanguageName
        {
            get
            {
                var lang = Languages.Find(x => x.IsCurrent);
                return lang != null ? lang.LanguageName : "";
            }
        }

        public static string CurrentLanguageCode
        {
            get
            {
                var lang = Languages.Find(x => x.IsCurrent);
                return lang != null ? lang.LanguageCode : "";
            }
        }

        public static string DefaultLanguageCode
        {
            get
            {
                var lang = Languages.Find(x => x.IsDefault);
                return lang != null ? lang.LanguageCode : "";
            }
        }

        //*set
        public static void SetCulture(string cultureName)
        {
            try
            {
                var cultureInfo = new CultureInfo(cultureName);
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(_typeFullName + ".SetCulture Error: " + "Culture:" + cultureName + ", " + ex.Message);
            }

        }

        public static void SetCurrent(string id)
        {
            var lang = Languages.Find(x => x.Id == id);
            if (lang != null)
            {
                var oldCurLang = Languages.Find(x => x.IsCurrent);
                oldCurLang.IsCurrent = false;
                lang.IsCurrent = true;
            }
            else throw new ArgumentException(_typeFullName + ".SetCurrent Error: " + "Language:" + id + " does not exsit!");

        }

        //*get
        public static Language GetLanguageById(string id)
        {
            var lang = Languages.Find(x => x.Id == id);
            return lang;
        }



        public static string GetLanguageCodeByLanguageId(string id)
        {
            var lang = Languages.Find(x => x.Id == id);
            if (lang != null)
            {
                return lang.LanguageCode;
            }
            else return string.Empty;
        }

        public static string GetCultureNameByLanguageId(string id)
        {
            var lang = Languages.Find(x => x.Id == id);
            if (lang != null)
            {
                return lang.CultureName;
            }
            else return string.Empty;
        }

        public static string GetOsCultureName()
        {
            return CultureInfo.CurrentCulture.Name;
            //CultureInfo.CurrentUICulture
        }

        //*check
        public static void CheckLanguageIdValidity(string id)
        {
            var langs = Languages.FindAll(x => x.Id == id);
            if (langs.Count == 0)
            {
                throw new ArgumentException(_typeFullName + ".CheckLanguageIdValidity Error: " + "Language: " + id + " does not exsit!");
            }
        }

        public static void CheckCultureNameValidity(string cultureName)
        {
            var langs = Languages.FindAll(x => x.CultureName == cultureName);
            if (langs.Count == 0)
            {
                throw new ArgumentException(_typeFullName + ".CheckCultureNameValidity Error: " + "Culture: " + cultureName + " does not exsit!");
            }
        }

        public static void CheckCultureNameLegality(string cultureName)
        {
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var cultures = allCultures.Where(x => x.Name == cultureName);
            if (cultures.Count() == 0)
            {
                throw new ArgumentException(_typeFullName + ".CheckCultureNameLegality Error: " + "Culture:" + cultureName + " does not exsit!");
            }
        }

    }
}
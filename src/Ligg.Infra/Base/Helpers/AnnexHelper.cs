using System;
using System.Collections.Generic;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;


namespace Ligg.Infrastructure.Base.Helpers
{

    public static class AnnexHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        private static string _defaultLanguageCode;
        public static string DefaultLanguageCode
        {
            private get
            {
                return _defaultLanguageCode.IsNullOrEmpty() ? CultureHelper.DefaultLanguageCode : _defaultLanguageCode;
            }
            set
            {
                _defaultLanguageCode = value;
            }
        }

        //#gettext
        //##GetText by masterId, className can be empty
        public static string GetText(string className, long masterId, List<Annex> annexes, AnnexTextType textType, string curLangCode, GetAnnexMode getAnnexMode)
        {

            var annex = Get(className, masterId, annexes, curLangCode, getAnnexMode);
            if (annex == null) return string.Empty;
            else
            {
                return GetTextByAnnex(annex, textType);
            }

        }

        //#GetText by masterName, className can be empty
        public static string GetText(string className, string masterName, List<Annex> annexes, AnnexTextType textType, string curLangCode, GetAnnexMode getAnnexMode)
        {

            var annex = Get(className, masterName, annexes, curLangCode, getAnnexMode);
            if (annex == null) return string.Empty;
            else
            {
                return GetTextByAnnex(annex, textType);
            }

        }

        public static AnnexTextType GetTextType(string annexTypeStr)
        {

            var annexType = AnnexTextType.DisplayName;
            if (annexTypeStr.ToLower() == "displayName")
            {
                annexType = AnnexTextType.DisplayName;
            }
            else if (annexTypeStr.ToLower() == "description")
            {
                annexType = AnnexTextType.Description;
            }
            else if (annexTypeStr.ToLower() == "remark")
            {
                annexType = AnnexTextType.Remark;
            }
            else if (annexTypeStr.ToLower() == "remark1")
            {
                annexType = AnnexTextType.Remark1;
            }
            else if (annexTypeStr.ToLower() == "remark2")
            {
                annexType = AnnexTextType.Remark2;
            }
            else if (annexTypeStr.ToLower() == "body")
            {
                annexType = AnnexTextType.Body;
            }
            else if (annexTypeStr.ToLower() == "other")
            {
                annexType = AnnexTextType.Other;
            }
            return annexType;

        }

        //#common
        //##get
        //##Get by masterId
        private static Annex Get(string className, long masterId, List<Annex> annexes, string langCode, GetAnnexMode getAnnexMode)
        {
            if (annexes == null) return null;

            var annex = new Annex();

            if (getAnnexMode == GetAnnexMode.OnlyByCurLang)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == langCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == langCode);
            }

            else if (getAnnexMode == GetAnnexMode.FirstAnnex)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == langCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == langCode);

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId);
                }
            }

            else if (getAnnexMode == GetAnnexMode.StepByStep)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == langCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == langCode);

                //if (annex == null)
                //{
                //    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == CultureHelper.CurrentLanguageCode)
                //        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == CultureHelper.CurrentLanguageCode);
                //}

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == DefaultLanguageCode)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == DefaultLanguageCode);
                }

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId);
                }
            }
            return annex ?? null;

        }

        //#Get by masterName
        private static Annex Get(string className, string masterName, List<Annex> annexes, string langCode, GetAnnexMode getAnnexMode)
        {
            if (annexes == null) return null;

            var annex = new Annex();
            if (getAnnexMode == GetAnnexMode.FirstAnnex)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName);
            }
            else if (getAnnexMode == GetAnnexMode.OnlyByCurLang)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == langCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == langCode);
            }
            else if (getAnnexMode == GetAnnexMode.StepByStep)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == langCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == langCode);

                //if (annex == null)
                //{
                //    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == CultureHelper.CurrentLanguageCode)
                //        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == CultureHelper.CurrentLanguageCode);
                //}

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == DefaultLanguageCode)
                        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == DefaultLanguageCode);
                }

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName)
                        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName);
                }
            }
            return annex ?? null;

        }


        private static string GetTextByAnnex(Annex annex, AnnexTextType textType)
        {

            if (annex != null)
            {
                if (textType == AnnexTextType.DisplayName) return annex.DisplayName;
                else if (textType == AnnexTextType.Description) return annex.Description;
                else if (textType == AnnexTextType.Body) return annex.Body;
                else if (textType == AnnexTextType.Remark) return annex.Remark;
                else if (textType == AnnexTextType.Remark1) return annex.Remark1;
                else if (textType == AnnexTextType.Remark2) return annex.Remark2;
                else if (textType == AnnexTextType.Other) return annex.Other;
            }
            return string.Empty;

        }

    }

    public enum GetAnnexMode
    {
        FirstAnnex = 0,
        OnlyByCurLang = 1,
        StepByStep = 2,
    }
}

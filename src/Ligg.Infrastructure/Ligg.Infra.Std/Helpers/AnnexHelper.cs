using System;
using System.Collections.Generic;
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;


namespace Ligg.Infrastructure.Helpers
{

    public static class AnnexHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        private static string _defaultLanguageCode;
        public static string DefaultLanguageCode
        {
            private get
            {
                //return _defaultLanguageCode.IsNullOrEmpty() ? LanguageHelper.DefaultLanguageCode : _defaultLanguageCode;
                return _defaultLanguageCode;
            }
            set
            {
                _defaultLanguageCode = value;
            }
        }

        private static string _currentLanguageCode;
        public static string CurrentLanguageCode
        {
            private get
            {
                //return _currentLanguageCode.IsNullOrEmpty() ? LanguageHelper.CurrentLanguageCode : _currentLanguageCode;
                return _currentLanguageCode;
            }
            set
            {
                _currentLanguageCode = value;
            }
        }


        //*GetText
        //GetText by masterName, className can be empty
        public static string GetText(string className, string masterName, List<Annex> annexes, AnnexTextType textType, GetAnnexMode getAnnexMode, string defText)
        {

            var annex = Get(className, masterName, annexes, _currentLanguageCode, getAnnexMode);
            //if (annex == null) return defTex.IsNullOrEmpty() ? masterName : defTex;
            if (annex == null) return defText.IsNullOrEmpty() ? "" : defText;
            else
            {
                var rst = GetTextByAnnex(annex, textType);
                return rst;
            }
        }
        //GetText by masterId, className can be empty
        public static string GetText(string className, long masterId, List<Annex> annexes, AnnexTextType textType, GetAnnexMode getAnnexMode, string defText)
        {

            var annex = Get(className, masterId, annexes, _currentLanguageCode, getAnnexMode);
            if (annex == null) return defText.IsNullOrEmpty() ? "" : defText;
            else
            {
                return GetTextByAnnex(annex, textType);
            }

        }

        //*for web server end
        public static string GetText(string className, string masterName, List<Annex> annexes, AnnexTextType textType, string curLangCode, GetAnnexMode getAnnexMode, string defText)
        {

            var annex = Get(className, masterName, annexes, curLangCode, getAnnexMode);
            //if (annex == null) return defTex.IsNullOrEmpty() ? masterName : defTex;
            if (annex == null) return defText.IsNullOrEmpty() ? "" : defText;
            else
            {
                var rst = GetTextByAnnex(annex, textType);
                return rst;
            }
        }
        //*for web server end
        public static string GetText(string className, long masterId, List<Annex> annexes, AnnexTextType textType, string curLangCode, GetAnnexMode getAnnexMode, string defText)
        {

            var annex = Get(className, masterId, annexes, curLangCode, getAnnexMode);
            if (annex == null) return defText;
            else
            {
                return GetTextByAnnex(annex, textType);
            }

        }

        //*GetTextType
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

        //*private
        //*Get
        //*for web server end
        //Get by masterId
        private static Annex Get(string className, long masterId, List<Annex> annexes, string curLangCode, GetAnnexMode getAnnexMode)
        {
            if (annexes == null) return null;

            var annex = new Annex();

            if (getAnnexMode == GetAnnexMode.First)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId);
            }

            if (getAnnexMode == GetAnnexMode.DefaultOrFirst)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == _defaultLanguageCode)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == _defaultLanguageCode);

                if (annex == null)
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId)
                            : annexes.Find(x => x.ClassName == className && x.MasterId == masterId);
            }

            else if (getAnnexMode == GetAnnexMode.Current)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == curLangCode);

            }

            else if (getAnnexMode == GetAnnexMode.CurrentOrFirst)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == curLangCode);

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId);
                }
            }

            else if (getAnnexMode == GetAnnexMode.CurrentOrDefault)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == curLangCode);

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == _defaultLanguageCode)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == _defaultLanguageCode);
                }
            }

            else if (getAnnexMode == GetAnnexMode.StepByStep)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == curLangCode);

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId && x.LanguageCode == _defaultLanguageCode)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId && x.LanguageCode == _defaultLanguageCode);
                }

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterId == masterId)
                        : annexes.Find(x => x.ClassName == className && x.MasterId == masterId);
                }
            }

            return annex ?? null;

        }

        //Get by masterName
        private static Annex Get(string className, string masterName, List<Annex> annexes, string curLangCode, GetAnnexMode getAnnexMode)
        {
            if (annexes == null) return null;

            var annex = new Annex();
            if (getAnnexMode == GetAnnexMode.First)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName);
            }

            if (getAnnexMode == GetAnnexMode.DefaultOrFirst)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == _defaultLanguageCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == _defaultLanguageCode);

                if (annex == null)
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName)
                        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName);

            }
            else if (getAnnexMode == GetAnnexMode.Current)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == curLangCode);

            }
            else if (getAnnexMode == GetAnnexMode.CurrentOrFirst)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == curLangCode);

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName)
                        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName);
                }
            }
            else if (getAnnexMode == GetAnnexMode.CurrentOrDefault)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == curLangCode);

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == _defaultLanguageCode)
                        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == _defaultLanguageCode);
                }
            }
            else if (getAnnexMode == GetAnnexMode.StepByStep)
            {
                annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == curLangCode)
                    : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == curLangCode);

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName && x.LanguageCode == _defaultLanguageCode)
                        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName && x.LanguageCode == _defaultLanguageCode);
                }

                if (annex == null)
                {
                    annex = className.IsNullOrEmpty() ? annexes.Find(x => x.MasterName == masterName)
                        : annexes.Find(x => x.ClassName == className && x.MasterName == masterName);
                }
            }

            return annex ?? null;

        }

        //*GetTextByAnnex
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
        First = 0,
        DefaultOrFirst = 1,
        Current = 2,
        CurrentOrFirst = 20,
        CurrentOrDefault = 21,
        StepByStep = 3,
    }
}

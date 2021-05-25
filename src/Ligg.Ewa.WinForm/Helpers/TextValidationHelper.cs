using System;
using Ligg.Infrastructure.Base.Extension;
using Ligg.EasyWinApp.WinForm.Resources;
using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;
using Ligg.EasyWinApp.Parser;
using Ligg.EasyWinApp.Parser.Helpers;

namespace Ligg.EasyWinApp.WinForm.Helpers
{
    public static class TextValidationHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;
        public static string Validate(string text, string validationRule)
        {
            try
            {
                var ruleArray = validationRule.GetSubParamArray(true, true);
                if (ruleArray[0] == "Mandatory")
                {
                    if (!text.IsNullOrEmpty())
                    {
                        return "true";
                    }
                    else
                    {
                        return ValidationRes.Mandatory;
                    }
                }
                if (ruleArray[0] == "StringMinLength")
                {
                    var minLen = Convert.ToInt16(ruleArray[1]);
                    if (text.Length > minLen - 1)
                    {
                        return "true";
                    }
                    else
                    {
                        return string.Format(ValidationRes.StringMinLength, minLen);
                    }
                }
                if (ruleArray[0] == "StringMaxLength")
                {
                    var maxLen = Convert.ToInt16(ruleArray[1]);
                    if (text.Length < maxLen - 1)
                    {
                        return "true";
                    }
                    else
                    {
                        return string.Format(ValidationRes.StringMaxLength, maxLen);
                    }
                }
                if (ruleArray[0] == "WebUrlFormat")
                {
                    if (text.IsWebUrl())
                    {
                        return "true";
                    }
                    else
                    {
                        return string.Format(ValidationRes.WebUrlFormat);
                    }
                }
                if (ruleArray[0] == "EmailFormat")
                {
                    if (text.IsEmailAddress())
                    {
                        return "true";
                    }
                    else
                    {
                        return string.Format(ValidationRes.EmailFormat);
                    }
                }
                if (ruleArray[0] == "PasswordFormat")
                {
                    if (text.IsPassword())
                    {
                        return "true";
                    }
                    else
                    {
                        return string.Format(ValidationRes.PasswordFormat);
                    }
                }


                return "OutOfScopeOfTextValidationHelper";
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + TypeName + ".Validate error: " + ex.Message);
            }
        }



    }
}
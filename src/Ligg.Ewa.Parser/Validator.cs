using System;
using Ligg.Infrastructure.Base.Extension;
using Ligg.EasyWinApp.Parser.Resources;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.EasyWinApp.Parser.Helpers;

namespace Ligg.EasyWinApp.Parser
{
    public static class Validator
    {
        public static string Validate(string text, string validationRule)
        {

            var ruleArray = validationRule.GetParamArray(true, true);
            if (ruleArray[0].ToLower() == "Mandatory".ToLower())
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
            else if (ruleArray[0].ToLower() == "Equal".ToLower())
            {

                if (text == ruleArray[1])
                    return "true";
                return string.Format(ValidationRes.IncorrectInput);
            }
            else if (ruleArray[0].ToLower() == "NotEqual".ToLower())
            {

                if (text != ruleArray[1])
                    return "true";
                return string.Format(ValidationRes.IncorrectInput);
            }

            else if (ruleArray[0].ToLower() == "StringLengthRange".ToLower())
            {
                var minLen = Convert.ToInt16(ruleArray[1]);
                var maxLen = Convert.ToInt16(ruleArray[2]);
                if (text.Length > minLen - 1 & text.Length < maxLen - 1)
                {
                    return "true";
                }
                else
                {
                    return string.Format(ValidationRes.StringLengthRange, minLen, maxLen);
                }
            }

            else if (ruleArray[0].ToLower() == "StringMinLength".ToLower())
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

            else if (ruleArray[0].ToLower() == "StringMaxLength".ToLower())
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

            else if (ruleArray[0].ToLower() == "PlusInteger".ToLower())
            {
                if (text.IsPlusInteger())
                    return "true";
                return string.Format(ValidationRes.PlusInteger);
            }

            else if (ruleArray[0].ToLower() == "PlusIntegerOrZero".ToLower())
            {
                if (text.IsPlusIntegerOrZero())
                    return "true";
                return string.Format(ValidationRes.IncorrectInput);
            }

            else if (ruleArray[0].ToLower() == "Numeral".ToLower())
            {
                if (text.IsNumeral())
                    return "true";
                return string.Format(ValidationRes.Numeral);
            }
            else if (ruleArray[0].ToLower() == "NumeralRange".ToLower())
            {
                var max = "";
                if (ruleArray.Length > 2) max = ruleArray[2];
                if (text.JudgeNumeralRange(ruleArray[1], max))
                    return "true";
                var min1 = ruleArray[1].IsNullOrEmpty() ? "-∞" : ruleArray[1];
                var max1 = max.IsNullOrEmpty() ? "+∞" : ruleArray[1];
                return string.Format(ValidationRes.NumeralRange, min1, max1);
            }

            else if (ruleArray[0].ToLower() == "WebUrl".ToLower())
            {
                if (text.IsWebUrl())
                {
                    return "true";
                }
                else
                {
                    return string.Format(ValidationRes.WebUrl);
                }
            }
            else if (ruleArray[0].ToLower() == "EmailAddress".ToLower())
            {
                if (text.IsEmailAddress())
                {
                    return "true";
                }
                else
                {
                    return string.Format(ValidationRes.EmailAddress);
                }
            }
            else if (ruleArray[0].ToLower() == "UserName".ToLower())
            {
                if (text.IsAlphaNumeralAndHyphen())
                    return "true";
                return string.Format(ValidationRes.UserName);
            }
            else if (ruleArray[0].ToLower() == "Password".ToLower())
            {
                if (text.IsPassword())
                {
                    return "true";
                }
                else
                {
                    return string.Format(ValidationRes.Password);
                }
            }
            else if (ruleArray[0].ToLower() == "Password1".ToLower())
            {
                if (text.IsPassword())
                {
                    return "true";
                }
                else
                {
                    return string.Format(ValidationRes.Password1);
                }
            }
            else if (ruleArray[0].ToLower() == "FilePath".ToLower())
            {
                if (FileHelper.IsFileExisting(text))
                    return "true";
                return string.Format(ValidationRes.FilePath);
            }
            else if (ruleArray[0].ToLower() == "Directory".ToLower())
            {
                if (DirectoryHelper.IsDirectoryExisting(text))
                    return "true";
                return string.Format(ValidationRes.Directory);
            }

            else if (ruleArray[0].ToLower() == "Ip".ToLower())
            {
                if (text.IsIp())
                    return "true";
                return string.Format(ValidationRes.Ip);
            }
            else if (ruleArray[0].ToLower() == "IpOrMask".ToLower())
            {
                if (text.IsIpOrMask())
                    return "true";
                return string.Format(ValidationRes.IpOrMask);
            }
            else if (ruleArray[0].ToLower() == "Mask".ToLower())
            {
                if (text.IsMask())
                    return "true";
                return string.Format(ValidationRes.Mask);
            }
            else if (ruleArray[0].ToLower() == "Regex".ToLower())
            {
                if (text.MatchesRegex(ruleArray[1]))
                    return "true";
                return string.Format(ValidationRes.UserName);
            }

            return "OutOfScopeOfValidator";
        }



    }
}
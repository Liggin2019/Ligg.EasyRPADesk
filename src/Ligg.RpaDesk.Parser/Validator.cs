using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using  Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Parser.Resources;
using System;
using System.IO;

namespace Ligg.RpaDesk.Parser
{
    public static class Validator
    {
        public static UniversalResult Validate(string text, string validationRule)
        {
            var success = false;
            var msg ="";
            var ruleArray = validationRule.GetLarrayArray(true, true);
            var rst = new UniversalResult() { Success = success, Message = msg };
            if (ruleArray[0].ToLower() == "Mandatory".ToLower())
            {
                if (!text.IsNullOrEmpty()) success = true;
                else msg =ValidationRes.Mandatory;

            }
            else if (ruleArray[0].ToLower() == "Equal".ToLower())
            {
                if (text == ruleArray[1]) success = true;
                else msg = string.Format(ValidationRes.IncorrectInput);
            }
            else if (ruleArray[0].ToLower() == "NotEqual".ToLower())
            {
                if (text != ruleArray[1]) success = true;
                else msg = string.Format(ValidationRes.IncorrectInput);
            }

            else if (ruleArray[0].ToLower() == "StringLengthRange".ToLower())
            {
                var minLen = Convert.ToInt16(ruleArray[1]);
                var maxLen = Convert.ToInt16(ruleArray[2]);

                if (text.Length > minLen - 1 & text.Length < maxLen - 1) success = true;
                else msg = string.Format(ValidationRes.StringLengthRange, minLen, maxLen);

            }

            else if (ruleArray[0].ToLower() == "StringMinLength".ToLower())
            {
                var minLen = Convert.ToInt16(ruleArray[1]);

                if (text.Length > minLen - 1) success = true;
                else msg = string.Format(ValidationRes.StringMinLength, minLen);
            }

            else if (ruleArray[0].ToLower() == "StringMaxLength".ToLower())
            {
                var maxLen = Convert.ToInt16(ruleArray[1]);

                if (text.Length < maxLen - 1) success = true;
                else msg = string.Format(ValidationRes.StringMaxLength, maxLen);
            }

            else if (ruleArray[0].ToLower() == "PlusInteger".ToLower())
            {
                if (text.IsPlusInteger()) success = true;
                else msg = string.Format(ValidationRes.PlusInteger);

            }

            else if (ruleArray[0].ToLower() == "PlusIntegerOrZero".ToLower())
            {
                if (text.IsPlusIntegerOrZero()) success = true;
                else msg = string.Format(ValidationRes.IncorrectInput);
            }

            else if (ruleArray[0].ToLower() == "Numeral".ToLower())
            {
                if (text.IsNumeral()) success = true;
                else msg = string.Format(ValidationRes.Numeral);
            }
            else if (ruleArray[0].ToLower() == "NumeralRange".ToLower())
            {
                if (!text.IsNumeral()) msg =string.Format(ValidationRes.Numeral);
                else
                {

                    var max = Convert.ToSingle(ruleArray[2]);
                    var min = Convert.ToSingle(ruleArray[1]);
                    var num = Convert.ToSingle(text);
                    if(num>min&(num<max|num==max)) success = true;
                    else
                    {
                        msg =string.Format(ValidationRes.NumeralRange, ruleArray[1], ruleArray[2]);
                    }
                }
            }

            else if (ruleArray[0].ToLower() == "WebUrl".ToLower())
            {
                if (text.IsWebUrl()) success = true;
                else msg = string.Format(ValidationRes.WebUrl);
            }
            else if (ruleArray[0].ToLower() == "EmailAddress".ToLower())
            {
                if (text.IsLegalEmailAddress()) success = true;
                else msg = string.Format(ValidationRes.EmailAddress);
            }
            else if (ruleArray[0].ToLower() == "UserName".ToLower())
            {
                if (!StringExtension.AlphaNumeralAndHyphenExpression.IsMatch(text)) success = true;
                else msg = string.Format(ValidationRes.UserName);
            }
            else if (ruleArray[0].ToLower() == "Password".ToLower())
            {
                if (text.IsPassword()) success = true;
                else msg = string.Format(ValidationRes.Password);

            }
            else if (ruleArray[0].ToLower() == "FilePath".ToLower())
            {
                if (File.Exists(text)) success = true;
                else msg = string.Format(ValidationRes.FilePath);
            }
            else if (ruleArray[0].ToLower() == "Directory".ToLower())
            {
                if (DirectoryHelper.IsDirectoryExisting(text)) success = true;
                else msg = string.Format(ValidationRes.Directory);
            }

            else if (ruleArray[0].ToLower() == "Ip".ToLower())
            {
                if (text.IsIp()) success = true;
                else msg =string.Format(ValidationRes.Ip);
            }
            else if (ruleArray[0].ToLower() == "IpOrMask".ToLower())
            {
                if (text.IsIpOrMask()) success = true;
                else msg =string.Format(ValidationRes.IpOrMask);
            }
            else if (ruleArray[0].ToLower() == "Mask".ToLower())
            {
                if (text.IsMask()) success = true;
                else msg =string.Format(ValidationRes.Mask);
            }

            else if (ruleArray[0].ToLower() == "Regex".ToLower())
            {
                if (text.MatchesRegex(ruleArray[1])) success = true;
                else msg = string.Format(ValidationRes.UserName);
            }
            else
            {
                msg = "LRDUNDEFINED";
            }

            rst.Success = success; rst.Message = msg;
            return rst ;

        }



    }
}
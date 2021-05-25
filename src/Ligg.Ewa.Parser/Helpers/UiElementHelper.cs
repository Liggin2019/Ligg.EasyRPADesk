using System;
using System.Collections.Generic;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;

namespace Ligg.EasyWinApp.Parser.Helpers
{
    public static class UiElementHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        public static void CheckName(string text)
        {
            var exInfo = "\n>> " + TypeName + ".CheckName Error: ";
            if (text.IsNullOrEmpty())
            {
                throw new ArgumentException(exInfo+"UiElememt Name \" + text + \" can not be empty! ");
            }
            if (!text.IsAlphaNumeralAndHyphen())
            {
                throw new ArgumentException(exInfo+"UiElememt Name \" + text + \" is not in valid format, UiElememt Name can only includes alpha,numeral and hyphen! ");
            }
        }






    }

}
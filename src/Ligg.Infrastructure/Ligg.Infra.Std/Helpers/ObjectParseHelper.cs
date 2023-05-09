using System;
using System.Collections;
using System.Collections.Generic;
using Ligg.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace Ligg.Infrastructure.Helpers
{

    public static partial class ObjectHelper
    {

        public static DateTime ParseToDateTime(this string str, string format = null)
        {
            var defDateTime = new DateTime();
            if (string.IsNullOrWhiteSpace(str))
            {
                return defDateTime;
                //return DateTime.MinValue;
            }
            try
            {
                if (format.IsNullOrEmptyOrWhiteSpace())
                {

                    return JsonConvert.DeserializeObject<DateTime>(str);

                }
                else
                {
                    return DateTime.ParseExact(str, format, System.Globalization.CultureInfo.CurrentCulture);
                }
            }
            catch (Exception ex)
            {
                return defDateTime;
            }
        }

    }
}




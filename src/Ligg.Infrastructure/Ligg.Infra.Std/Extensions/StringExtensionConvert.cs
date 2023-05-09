using Ligg.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Ligg.Infrastructure.Extensions
{
    public static partial class StringExtension
    {

        public static List<T> ConvertIdsStringToIntegerList<T>(this string target, char separator)
        {
            var IntegerList = new List<T>();
            var idStrArray = target.Split(separator);
            idStrArray = idStrArray.Wash();

            if (idStrArray.Length > 0)
            {
                for (var i = 0; i < idStrArray.Length; i++)
                {
                    if (!string.IsNullOrEmpty(idStrArray[i]))
                    {
                        if (idStrArray[i].IsPlusIntegerOrZero())
                        {
                            var integer = (T)Convert.ChangeType(idStrArray[i], typeof(T));
                            IntegerList.Add(integer);
                        }
                    }
                }
            }
            return IntegerList;
        }

  
    }
}
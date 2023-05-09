using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace Ligg.Infrastructure.Helpers
{
    internal static class DataTableHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*convert
        internal static string ConvertToJson(this DataTable dt)
        {
            if (dt == null) return "";
            if (dt.Rows.Count == 0) return "";

            return JsonHelper.Serialize(dt);
        }


    }
}

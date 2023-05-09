using System;
using System.Collections.Generic;
using System.Linq;
using  Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.RpaDesk.Parser.DataModels;
using  Ligg.Infrastructure.Utilities.DataParserUtil;

namespace Ligg.RpaDesk.Parser.Helpers
{
    public static class ShellHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;


        public static void CheckProcedures(List<Procedure> procedures, string shellName)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckProcedures Error: ";
            var shellTypeNames = EnumHelper.GetNames<ProcedureType>();
            foreach (var procedureItem in procedures)
            {
                if (procedureItem.Name.IsNullOrEmpty()) procedureItem.Name = "";
                var exInfo1 = "shellName= " + shellName + ", Procedure.Name= " + procedureItem.Name;
                CheckProcedureName(procedureItem.Name);

                var sameNameVars = procedures.FindAll(x => x.Name == procedureItem.Name);
                if (sameNameVars.Count > 1)
                {
                    throw new ArgumentException(exInfo + "Procedure can't have duplicated name!" + exInfo1);
                }

                var isLegalShellTypeName = shellTypeNames.Contains(procedureItem.TypeName);
                if (!isLegalShellTypeName)
                    throw new ArgumentException(exInfo + "Procedure's Type is not valid ! TypeName= " + procedureItem.TypeName + ", item.Name=" + procedureItem.Name + ", TypeName should be in " + shellTypeNames.Unwrap(", "));

            }

        }

        public static void SetProcedureType(Procedure procedure)
        {
            try
            {
                procedure.Type = EnumHelper.GetIdByName<ProcedureType>(procedure.TypeName);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + _typeFullName + ".SetProcedureType Error: " + ex.Message);
            }
        }

        public static void SetVariableValue(string varName, string val, List<Procedure> procedures)
        {
            var exInfo = "\n>> " + _typeFullName + ".SetVariableValue Error: ";

            var var = procedures.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
            if (var != null)
            {
                var.Value = val;
            }
            else
            {
                throw new ArgumentException(exInfo + "Variable: " + varName + " does not exist!");
            }
        }
        public static string GetVariableValue(string varName, List<Procedure> procedures)
        {
            var exInfo = "\n>> " + _typeFullName + ".SetsVariableValue Error: ";

            var var = procedures.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
            if (var != null)
            {
                return var.Value;
            }
            else
            {
                throw new ArgumentException(exInfo + "Shell Variable: " + varName + " does not exist!");
            }
        }



        public static string AddShellIdToRefsForProcedureElement(string str, string ShellId, bool isForTransaction)
        {
            if (str.IsNullOrEmpty()) return string.Empty;

            if (!isForTransaction &
                !(str.StartsWith(IdentifierHelper.ShellIdentifer) & str.EndsWith(IdentifierHelper.ShellIdentifer)) & !str.StartsWith("="))
                return str;

            var str1 = str;
            if (str.Contains("#"))
            {
                var strArray = str.Split('#');
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(" '#' no. in " + str + " is not a even! ");//*old
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (i % 2 == 1)
                        {
                            if (strArray[i].IsNullOrEmpty() | strArray[i] == ".")
                            {
                                strArray[i] = ShellId;
                            }
                            else
                                strArray[i] = ShellId + "_" + strArray[i];
                        }
                    }
                    str1 = string.Join("#", strArray);
                }
            }

            return str1;
        }

        public static string ResolveStringByRefVariables(string str, bool supportMultiLangs,List<Procedure> procedures, List<Annex> annexes)
        {
            var exInfo = "\n>> " + _typeFullName + ".ResolveStringByRefVariables Error: ";
            str.CheckProcessElementDataFormat();
            var vars = procedures.Where(x => x.Type == (int)ProcedureType.Variable | x.Type == (int)ProcedureType.Args).ToList();
            if (str.IsNullOrEmpty()) return "";
            if (!IdentifierHelper.ContainsShellIdentifer(str)) return str;
            var strArray = str.Split(IdentifierHelper.ShellIdentifer.ToChar());
            int n = strArray.Count();

            var i = 1;
            if (strArray[i].GetLastSeparatedString('_').IsPlusIntegerOrZero()) //args
            {
                var arry = strArray[i].Split('_');
                if (arry.Length > 1)
                {
                    var lastStr = arry[arry.Length - 1];
                    if (lastStr.IsPlusIntegerOrZero())
                    {
                        var shellId = "";
                        for (int ii = 0; ii < arry.Length - 1; ii++)
                        {
                            shellId = ii == 0 ? arry[ii] : shellId + "_" + arry[ii];
                        }

                        var zoneArgsVar = vars.Find(x => x.Name == shellId + "_args" & x.Type == (int)ProcedureType.Args);
                        if (zoneArgsVar != null)
                        {
                            var zoneParamArray = zoneArgsVar.Value.GetLarrayArray(false, false);
                            var index = Convert.ToInt16(lastStr);
                            if (index > zoneParamArray.Length - 1) throw new ArgumentException(exInfo + "Shell Arg: #" + strArray[i] + "# doesn't exsit! ");
                            strArray[i] = zoneParamArray[index];
                        }
                    }

                }
            }
            else if (strArray[i].ToLower().EndsWith(".v"))
            {
                var varName = strArray[i].Split('.')[0].Trim();
                var variable1 = vars.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
                if (variable1 == null) throw new ArgumentException(exInfo + "Variable: " + varName + " does not exist!");
                strArray[i] = variable1.Value;
            }
            else if (strArray[i].ToLower().EndsWith(".t"))
            {
                var varName = strArray[i].Split('.')[0].Trim();
                var variable1 = vars.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
                if (variable1 == null) throw new ArgumentException(exInfo + "Variable: " + varName + " does not exist!");
                var disPlayName = CommonHelper.GetDisplayName(supportMultiLangs, "Procedure", varName, annexes, variable1.DisplayName);
                strArray[i] = disPlayName;
            }
            else
            {
                strArray[i] = IdentifierHelper.ShellIdentifer + strArray[i] + IdentifierHelper.ShellIdentifer;
            }
            return strArray[i];

        }


        //*private
        private static void CheckProcedureName(string text)
        {
            var exInfo = "\n>> " + _typeFullName + ".CheckProcedureName Error: ";

            if (text.IsNullOrEmpty())
            {
                throw new ArgumentException(exInfo + "Procedure Name \" + text + \" can not be empty! ");
            }
            if (!StringExtension.AlphaAndNumeralExpression.IsMatch(text))
            {
                throw new ArgumentException(exInfo + "Procedure Name \" + text + \" is not in valid format, ProcessItem Name can only includes alpha,numeral and hyphen! ");
            }
        }


    }

}
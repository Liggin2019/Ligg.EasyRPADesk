using System;
using System.Collections.Generic;
using System.Linq;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;

namespace Ligg.EasyWinApp.Parser.Helpers
{
    public static class ProcessHelper
    {
        private static readonly string TypeName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;


        public static void CheckProcedures(List<Procedure> procedures, string processName)
        {
            var exInfo = "\n>> " + TypeName + ".CheckProcedures Error: ";
            var exInfo1 = "";

            foreach (var procedureItem in procedures)
            {
                if (procedureItem.Name.IsNullOrEmpty()) procedureItem.Name = "";
                exInfo1 = "processName= " + processName + ", Procedure.Name= " + procedureItem.Name;
                CheckProcedureName(procedureItem.Name);
                if (procedureItem.Name.EndsWith("_VALUES"))
                {
                    throw new ArgumentException(exInfo + "Procedure can't have name ends with VALUES! " + exInfo1);
                }


                var sameNameVars = procedures.FindAll(x => x.Name == procedureItem.Name);
                if (sameNameVars.Count > 1)
                {
                    throw new ArgumentException(exInfo + "Procedure can't have duplicated name!" + exInfo1);
                }

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
                throw new ArgumentException("\n>> " + TypeName + ".SetProcedureType Error: " + ex.Message);
            }
        }

        public static void ClearProcessVariablesByGroup(int groupId, List<Procedure> procedures)
        {
            var procList = new List<Procedure>();
            procList = procedures.FindAll(x => x.Type == (int)ProcedureType.Variable & x.GroupId == groupId);

            if (procList.Count == 0)
            {
                return;

            }

            foreach (var var in procList)
            {
                var.Value = string.Empty;
            }
        }

        public static void SetProcessVariableValue(string varName, string val, List<Procedure> procedures)
        {
            var exInfo = "\n>> " + TypeName + ".SetProcessVariableValue Error: ";

            var var = procedures.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
            if (var != null)
            {
                var.Value = val;
            }
            else
            {
                throw new ArgumentException(exInfo + "Process Variable: " + varName + " does not exist!");
            }
        }
        public static string GetProcessVariableValue(string varName, List<Procedure> procedures)
        {
            var exInfo = "\n>> " + TypeName + ".SetProcessVariableValue Error: ";

            var var = procedures.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
            if (var != null)
            {
                return var.Value;
            }
            else
            {
                throw new ArgumentException(exInfo + "Process Variable: " + varName + " does not exist!");
            }
        }

        public static string ResolveStringByRefProcessVariables(string str, List<Procedure> procedures)
        {
            var exInfo = "\n>> " + TypeName + ".ResolveStringByRefProcessVariables Error: ";

            var vars = procedures.Where(x => x.Type == (int)ProcedureType.Variable | x.Type == (int)ProcedureType.Params).ToList();
            if (str.IsNullOrEmpty()) return "";
            if (!IdentifierHelper.ContainsProcessIdentifer(str)) return str;
            var strArray = str.Split(IdentifierHelper.ProcessIdentifer.ToChar());
            int n = strArray.Count();
            if (n % 2 == 0)
            {
                throw new ArgumentException(IdentifierHelper.ProcessIdentifer + " no. in " + str + " is not a even! ");
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    if (i % 2 == 1)
                    {
                        if (strArray[i].IsPlusIntegerOrZero()) //func start input var, for consoleApp
                        {
                            var tempStr = strArray[i];
                            if (tempStr.IsPlusIntegerOrZero())
                            {
                                var inputParamVar = vars.Find(x => x.Name == "finput" & x.Type == (int)ProcedureType.Params);
                                if (inputParamVar != null)
                                {
                                    var funcParamArry = inputParamVar.Value.GetSubParamArray(true, false);
                                    var index = Convert.ToInt16(tempStr);
                                    if (index > funcParamArry.Length - 1) throw new ArgumentException("Process variable value: #" + strArray[i] + "# doesn't exsit! ");
                                    strArray[i] = funcParamArry[index];
                                }
                            }
                        }
                        else if (strArray[i].GetLastSeparatedString('_').IsPlusIntegerOrZero()) //zone input var, for winformApp
                        {
                            var arry = strArray[i].Split('_');
                            if (arry.Length > 1)
                            {
                                var lastStr = arry[arry.Length - 1];
                                if (lastStr.IsPlusIntegerOrZero())
                                {
                                    var zoneName = "";
                                    for (int ii = 0; ii < arry.Length - 1; ii++)
                                    {
                                        zoneName = ii == 0 ? arry[ii] : zoneName + "_" + arry[ii];
                                    }

                                    var zoneParamVar = vars.Find(x => x.Name == zoneName + "_zinput" & x.Type == (int)ProcedureType.Params);
                                    if (zoneParamVar != null)
                                    {
                                        var zoneParamArray = zoneParamVar.Value.GetSubParamArray(false,false);
                                        var index = Convert.ToInt16(lastStr);
                                        if(index> zoneParamArray.Length-1) throw new ArgumentException("Process variable value: #" + strArray[i] + "# doesn't exsit! ");
                                        strArray[i] = zoneParamArray[index];
                                    }
                                }
                                
                            }
                        }
                        else if (strArray[i].ToLower().EndsWith(".v"))
                        {
                            var varName = strArray[i].Split('.')[0].Trim();
                            var variable1 = vars.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
                            if (variable1 == null) throw new ArgumentException(exInfo + "Process Variable: " + varName + " does not exist!");
                            strArray[i] = variable1.Value;
                        }
                        else
                        {
                            strArray[i] = IdentifierHelper.ProcessIdentifer + strArray[i] + IdentifierHelper.ProcessIdentifer;
                        }
                    }

                }
                return string.Join("", strArray);
            }
        }


        //*private
        private static void CheckProcedureName(string text)
        {
            var exInfo = "\n>> " + TypeName + ".CheckProcedureName Error: ";

            if (text.IsNullOrEmpty())
            {
                throw new ArgumentException(exInfo + "Procedure Name \" + text + \" can not be empty! ");
            }
            if (!text.IsAlphaNumeralAndHyphen())
            {
                throw new ArgumentException(exInfo + "Procedure Name \" + text + \" is not in valid format, ProcessItem Name can only includes alpha,numeral and hyphen! ");
            }
        }


    }

}
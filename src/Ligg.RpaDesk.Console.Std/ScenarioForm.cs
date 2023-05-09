
using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
//using Ligg.Infrastructure.Handlers;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Parser;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.WinCnsl.DataModels;
using Ligg.RpaDesk.WinCnsl.DataModels.Enums;
using Ligg.RpaDesk.WinCnsl.Helpers;
using Ligg.RpaDesk.WinCnsl.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//*start
namespace Ligg.RpaDesk.WinCnsl
{
    public class ScenarioForm
    {
        public bool BoolOutput = true;
        private FormInitParamSet _formInitParamSet;
        public FormInitParamSet FormInitParamSet
        {
            protected get => _formInitParamSet;
            set => _formInitParamSet = value;
        }

        private Dictionary<String, string> _exchangeData = new Dictionary<String, string>();

        private string _basicInfoForException;
        private string _additionalExceptionInfo;
        private string _formLocation;

        private string _startupDir;
        private string _cfgDir;
        private string _glbCfgDir;
        private string _glbSharedDir;
        private string _glbUiDir;

        private string _appCfgDir;
        private string _appSharedDir;
        private string _appUiDir;

        private string _snrsDir;
        private string _formCfgDir;
        private string _functionCode;
        private string _snrDir;

        private string _dataDir;
        private string _glbDataDir;
        private string _appDataDir;
        private string _libDir;
        private string _glbLibDir;
        private string _appLibDir;
        private string _appOutputDir;

        private string _sysDataDir;
        private string _sysAppDataDir;
        private string _myDataDir;
        private string _myAppDataDir;

        private List<Procedure> _procedures = new List<Procedure>();
        private List<Annex> _annexes = new List<Annex>();
        private List<UiItem> _uiItems = new List<UiItem>();

        protected ScenarioForm()
        {
        }

        //*proc
        //*load
        //#load
        protected void Load()
        {
            _startupDir = Directory.GetCurrentDirectory();

            _cfgDir = _startupDir + "\\Conf";
            _glbCfgDir = _cfgDir + "\\Global";
            _glbSharedDir = _glbCfgDir + "\\Shared";
            _glbUiDir = _glbCfgDir + "\\Ui\\Console";

            _appCfgDir = _cfgDir + "\\Apps\\" + _formInitParamSet.ApplicationCode;
            _appSharedDir = _appCfgDir + "\\Shared";
            _appUiDir = _appCfgDir + "\\Ui\\Console";
            _snrsDir = _appUiDir + "\\Scenarios";

            _formCfgDir = FileHelper.GetPath(_formInitParamSet.FormRelativeLocation, _snrsDir, true);
            _functionCode = FileHelper.GetFileDetailByOption(_formCfgDir, FilePathComposition.FileTitle);
            _snrDir = _formCfgDir;

            _dataDir = _startupDir + "\\Data";
            _glbDataDir = _dataDir + "\\Global";
            _appDataDir = _formInitParamSet.ApplicationDataDir;
            _libDir = _startupDir + "\\Lib";
            _glbLibDir = _libDir + "\\Global";
            _appLibDir = _formInitParamSet.ApplicationLibDir;
            _appOutputDir = _formInitParamSet.ApplicationOutputDir;

            _sysDataDir = DirectoryHelper.GetSpecialDir("commonapplicationdata") + "\\" + _formInitParamSet.ArchitectureCode;
            _sysAppDataDir = _sysDataDir + "\\Apps\\" + _formInitParamSet.ApplicationCode;

            _myDataDir = DirectoryHelper.GetSpecialDir("mydocuments") + "\\" + _formInitParamSet.ArchitectureCode;
            _myAppDataDir = _myDataDir + "\\Apps\\" + _formInitParamSet.ApplicationCode;

            _basicInfoForException = _formInitParamSet.ApplicationCode + "." + _functionCode;
            //var userName = GetCentralData("UserName");
            var userName = ResolveConstantsEx("CurrentUserName");
            _additionalExceptionInfo = "--Architecture: " + _formInitParamSet.ArchitectureCode + "-" + _formInitParamSet.ArchitectureVersion + ", " +
                "Application: " + _formInitParamSet.ApplicationCode + "-" + _formInitParamSet.ApplicationVersion + ", " +
                (userName.IsNullOrEmpty() ? "" : ", UserName: " + userName) + "\n--" +
           "Please send this error information to HelpdeskEmail: " + _formInitParamSet.HelpdeskEmail + "";
            _formLocation = "Scenarios" + "\\" + _formInitParamSet.FormRelativeLocation;

            LoadForm();
        }

        //*func *form *loadform
        //##loadform
        private void LoadForm()
        {
            try
            {
                _annexes.AddRange(CommonHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\SharedAnnexes", "Shared", false));

                var annexes = CommonHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\ApplicationAnnexes", "", false);
                CommonHelper.SetApplicationAnnexes(annexes, _formInitParamSet.ApplicationCode);
                _annexes.AddRange(annexes);

                var annexes1 = CommonHelper.GetAnnexesFromCfgFile(_formCfgDir + "\\FormTitleAnnexes", "", false);
                CommonHelper.SetFormTitleAnnexes(annexes1, _functionCode);
                _annexes.AddRange(annexes1);

                //--initLayout starts
                var formStyle = DataParserHelper.ConvertToGeneric<FormStyle>(_snrDir + "\\FormStyle", false, TxtDataType.Undefined) ?? new FormStyle();
                formStyle.IconUrl = formStyle.IconUrl.IsNullOrEmpty() ? "" : ResolveConstants(formStyle.IconUrl);

                //Console.Title = _functionCode.ToUniqueStringByNow("-");

                if (formStyle.DrawIcon)
                {
                    var iconUrl = FileHelper.GetPath(formStyle.IconUrl, _snrDir);
                    if (!iconUrl.IsNullOrEmpty() && System.IO.File.Exists(iconUrl))
                    {
                        //by Liggin2019 on 210301
                        //ok for Ligg.Ewa.ConsoleApp(netFx 4.72)  ok for Ligg.Ewa.ConsoleApp.Core(netCore 3.1) 
                        //if call  Ligg.Infrastructure.Helpers.ConsoleHelper.SetConsoleIcon(iconUrl); //not ok for Ligg.Ewa.ConsoleApp(netFx 4.72)  not ok for Ligg.Ewa.ConsoleApp.Core(netCore 3.1) 
                        UiHelper.SetIcon(iconUrl);
                    }

                }

                if (_formInitParamSet.FormTitle.IsNullOrEmpty())
                {
                    var text = "";
                    if (_formInitParamSet.SupportMultiLanguages)
                    {
                        text = AnnexHelper.GetText("FormTitle", _functionCode, _annexes, AnnexTextType.DisplayName, GetAnnexMode.StepByStep, _formInitParamSet.ApplicationCode + "-" + _functionCode);
                    }
                    else
                    {
                        text = AnnexHelper.GetText("FormTitle", _functionCode, _annexes, AnnexTextType.DisplayName, GetAnnexMode.CurrentOrFirst, _formInitParamSet.ApplicationCode + "-" + _functionCode);
                    }
                    Console.Title = text;

                }
                else
                {
                    Console.Title = _formInitParamSet.FormTitle;
                }

                //--initLayout ends

                var uiShellProcedures = GetScenarioShell();
                if (uiShellProcedures != null)
                {
                    if (uiShellProcedures.Count > 0)
                        _procedures.AddRange(uiShellProcedures);
                }

                InitScenarioShell(uiShellProcedures);

                MergeUiItems();
                Execute();
            }
            catch (Exception ex)
            {
                var methodName = "LoadForm";
                throw new ArgumentException(_basicInfoForException + "." + methodName + " Error: _formLocation=" + _formLocation + ex.Message + "\n\n" + _additionalExceptionInfo);
            }
        }

        //*shell
        //##GetAndMergeScenarioShell
        private List<Procedure> GetScenarioShell()
        {
            var exInfo = "\n>> " + GetType().FullName + ".GetScenarioShell Error: ";

            var uiShellProcedures = new List<Procedure>();

            var uiShellProceduresTmp = new List<Procedure>();
            var location = _snrDir;
            var cfgFile = location + "\\Shell";
            try
            {
                uiShellProceduresTmp = DataParserHelper.ConvertToGeneric<List<Procedure>>(cfgFile, false, TxtDataType.Undefined) ?? new List<Procedure>();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }

            var uiShellProceduresTmp1 = new List<Procedure>();
            foreach (var proc in uiShellProceduresTmp)
            {
                proc.Invalid = proc.Invalid.GetJudgementFlag();
                if (proc.Invalid.ToLower() == "true")
                {
                    continue;
                }
                else
                {
                    uiShellProceduresTmp1.Add(proc);
                }
            }
            uiShellProceduresTmp1 = uiShellProceduresTmp1.FindAll(x => x.TypeName == ProcedureType.Variable.ToString()
                                              | x.TypeName == ProcedureType.Transaction.ToString() | x.TypeName == ProcedureType.SubTransaction.ToString()
                                              );
            if (uiShellProceduresTmp1.Count == 0) return null;

            var annexList = new List<Annex>();
            try
            {
                ShellHelper.CheckProcedures(uiShellProceduresTmp1, _functionCode);
                annexList = CommonHelper.GetAnnexesFromCfgFile(cfgFile + "Annexes", "Procedure", false);

                foreach (var proc in uiShellProceduresTmp1)
                {
                    ShellHelper.SetProcedureType(proc);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }

            foreach (var proc in uiShellProceduresTmp1)
            {
                proc.ShowRunningStatus = proc.ShowRunningStatus.GetJudgementFlag();
                proc.SkipOnInit = proc.SkipOnInit.GetJudgementFlag();
                proc.StartValue = string.IsNullOrEmpty(proc.StartValue) ? "" : proc.StartValue;
                proc.Value = proc.StartValue;
                proc.Expression = string.IsNullOrEmpty(proc.Expression) ? "" : proc.Expression;
                proc.Expression = ResolveConstants(proc.Expression);


                if (proc.GroupId < 0) proc.GroupId = 0;

                uiShellProcedures.Add(proc);


                var tempAnnexes = annexList.FindAll(x => x.MasterName == proc.Name);
                proc.Name = _functionCode + "_" + proc.Name;
                proc.ShellId = _functionCode;
                proc.Expression = ShellHelper.AddShellIdToRefsForProcedureElement(proc.Expression, _functionCode, (proc.Type == (int)ProcedureType.Transaction | proc.Type == (int)ProcedureType.SubTransaction) ? true : false);
                if (tempAnnexes.Count > 0)
                {
                    foreach (var annex in tempAnnexes)
                    {
                        annex.MasterName = proc.Name;
                        _annexes.Add(annex);
                    }
                }
            }
            return uiShellProcedures;
        }

        //##InitScenarioShell
        private void InitScenarioShell(List<Procedure> procedures)
        {
            var exInfo = "\n>> " + GetType().FullName + ".InitScenarioShell Error: ";

            if (procedures == null) return;
            if (procedures.Count == 0) return;

            try
            {
                var procListByGrp = procedures.FindAll(x => x.GroupId == 0 & x.SkipOnInit.ToLower() != "true" &
                x.Type != (int)ProcedureType.SubTransaction && x.Type != (int)ProcedureType.Args);
                RefreshProcedures(procListByGrp);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }
        }

        //*uiitems
        //##MergeUiItems
        private void MergeUiItems()
        {
            var exInfo = "\n>> " + GetType().FullName + ".MergeUiItems Error: ";

            var location = _snrDir;
            var tmpUiItems = new List<UiItem>();
            try
            {
                tmpUiItems = DataParserHelper.ConvertToGeneric<List<UiItem>>(location + "\\ui", true, TxtDataType.Undefined) ?? new List<UiItem>();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }

            if (tmpUiItems.Count == 0) return;

            var uiItems = new List<UiItem>();
            foreach (var item in tmpUiItems)
            {
                item.Invalid = ResolveConstants(item.Invalid);
                if (item.Invalid.StartsWith("=")) item.Invalid = GetText(item.Invalid, GetTextType.OnlyResolveCed);
                item.Invalid = item.Invalid.GetJudgementFlag();
                if (item.Invalid.ToLower() != "true")
                {
                    uiItems.Add(item);
                }
            }
            if (uiItems.Count == 0) return;

            foreach (var item in uiItems)
            {
                UiHelper.SetUiItemType(item);
            }
            UiHelper.CheckUiItems(_snrDir + "//Ui", uiItems);

            var annexList = CommonHelper.GetAnnexesFromCfgFile(location + "\\Annexes", "UiItem", false);
            foreach (var item in uiItems)
            {

                item.ShowRunningStatus = item.ShowRunningStatus.GetJudgementFlag();
                item.ExecMode = item.ExecMode.IsNullOrEmpty() ? "" : item.ExecMode;
                item.ShowRunningStatus = item.ShowRunningStatus.GetJudgementFlag();
                item.WriteIntoLog = item.WriteIntoLog.GetJudgementFlag();

                item.DisplayName = string.IsNullOrEmpty(item.DisplayName) ? "" : item.DisplayName;
                item.DisplayName = ResolveConstants(item.DisplayName);

                item.Condition = string.IsNullOrEmpty(item.Condition) ? "" : item.Condition;
                item.Condition = ResolveConstants(item.Condition);
                //item.Condition = ShellHelper.AddShellIdToRefsForProcedureElement(item.Condition, _functionCode, false);
                item.Condition = ShellHelper.AddShellIdToRefsForProcedureElement(item.Condition, _functionCode, (item.Type == (int)ProcedureType.Transaction | item.Type == (int)ProcedureType.SubTransaction) ? true : false);


                item.Value = string.IsNullOrEmpty(item.Value) ? "" : item.Value;
                item.Value = ResolveConstants(item.Value);
                //item.Value = ShellHelper.AddShellIdToRefsForProcedureElement(item.Value, _functionCode, false);
                item.Value = ShellHelper.AddShellIdToRefsForProcedureElement(item.Value, _functionCode, (item.Type == (int)ProcedureType.Transaction | item.Type == (int)ProcedureType.SubTransaction) ? true : false);
                _uiItems.Add(item);
                var tempAnnexes = annexList.FindAll(x => x.MasterName == item.Name);
                if (tempAnnexes.Count > 0)
                {
                    foreach (var annex in tempAnnexes)
                    {
                        annex.ClassName = "UiItem";
                        annex.MasterName = item.Name;
                        _annexes.Add(annex);
                    }
                }
            }
        }

        //**exec
        //##Execute
        private void Execute()
        {
            var uiItems = _uiItems.FindAll(x => x.Type > -1);
            var itemNameEx = "";
            try
            {
                UiHelper.SetUiItemsIds(uiItems);
                var len = uiItems.Count;
                var id = 0;
                do
                {
                    var item = _uiItems.Find(x => x.Id == id);
                    itemNameEx = item.Name;
                    var displayName = "";
                    if (item.Type == (int)UiItemType.Write | item.Type == (int)UiItemType.WriteLine |
                        item.Type == (int)UiItemType.ReadKey | item.Type == (int)UiItemType.ReadLine)
                    {
                        if (!String.IsNullOrEmpty(item.DisplayName))
                        {

                            if (item.DisplayName.StartsWith("="))
                            {
                                displayName = GetText(item.DisplayName, GetTextType.UiItem);
                            }
                            else
                            {
                                displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "UiItem", item.Name, _annexes, item.DisplayName);
                            }
                        }
                        else
                        {
                            //displayName = GetDisplayName(_formInitParamSet.SupportMultiLanguages, "UiItem", item.Name, _annexes, "");
                        }
                    }


                    var value = "";
                    if (item.Type == (int)UiItemType.Write | item.Type == (int)UiItemType.WriteLine | item.Type == (int)UiItemType.WriteSpace | item.Type == (int)UiItemType.NewLine)
                    {
                        value = GetText(item.Value, GetTextType.UiItem);
                    }
                    else if (item.Type == (int)UiItemType.GoTo)
                    {
                        value = GetText(item.Value, GetTextType.UiItem);
                    }
                    else if (item.Type == (int)UiItemType.Transaction)
                    {
                        //do nothing
                    }

                    //-read
                    if (item.Type == (int)UiItemType.ReadKey)
                    {
                        var exit = true;
                        do
                        {
                            Console.Write(TextRes.PressKey + " " + displayName + ": ");
                            var input = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            item.Value = input.ToString();
                            var validationRules = item.ValidationRules;

                            if (!validationRules.IsNullOrEmpty())
                            {
                                var rstTxt = GetText("=ValidateInput^" + item.Name, GetTextType.UiItem);
                                var rst = rstTxt.ConvertToGeneric<UniversalResult>(true, TxtDataType.Json);
                                if (rst.Success) exit = true;
                                else
                                {
                                    exit = false;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(rst.Message);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }

                            }
                            Console.WriteLine();
                        }
                        while (!exit);
                    }
                    else if (item.Type == (int)UiItemType.ReadLine)
                    {
                        var exit = true;
                        do
                        {
                            Console.Write(TextRes.Input + " " + displayName + ": ");

                            var input = Console.ReadLine();
                            item.Value = input.ToString();
                            var validationRules = item.ValidationRules;

                            if (!validationRules.IsNullOrEmpty())
                            {
                                var rstTxt = GetText("=ValidateInput^" + item.Name, GetTextType.UiItem);
                                var rst = rstTxt.ConvertToGeneric<UniversalResult>(true, TxtDataType.Json);
                                if (rst.Success) exit = true;
                                else
                                {
                                    exit = false;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(rst.Message);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                            }
                        }
                        while (!exit);
                    }
                    //-write
                    else if (item.Type == (int)UiItemType.NewLine)
                    {
                        var value1 = 1;
                        if (value.IsPlusInteger())
                        {
                            value1 = Convert.ToInt32(value);
                        }
                        var times = value1;
                        for (var i = 0; i < times; i++)
                        {
                            Console.WriteLine();
                        }
                    }
                    else if (item.Type == (int)UiItemType.WriteSpace)
                    {
                        var value1 = 1;
                        if (value.IsPlusInteger())
                        {
                            value1 = Convert.ToInt32(value);
                        }
                        var times = value1;
                        for (var i = 0; i < times; i++)
                        {
                            Console.Write(" ");
                        }
                    }

                    else if (item.Type == (int)UiItemType.Write)
                    {
                        if (!displayName.IsNullOrEmpty())
                        {
                            Console.Write(displayName + " ");
                        }

                        if (value != null) Console.Write(value);
                    }
                    else if (item.Type == (int)UiItemType.WriteLine)
                    {
                        if (!displayName.IsNullOrEmpty()) Console.Write(displayName + " ");

                        if (value != null) Console.WriteLine(value);
                    }
                    else if (item.Type == (int)UiItemType.Transaction)
                    {
                        if (value != null)
                            TransactByElementName("$" + item.Name + "$", TransactFrom.FromScenarioUi);
                    }
                    else if (item.Type == (int)UiItemType.Break)
                    {
                        var con = item.Condition;
                        var isOk = true;
                        if (con.IsNullOrEmpty()) isOk = true;
                        else
                        {
                            con = GetText(con, GetTextType.UiItem);
                            isOk = con.ToLower() == "true" ? true : false;
                        }
                        if (isOk) return;
                    }
                    else if (item.Type == (int)UiItemType.Exit)
                    {
                        var con = item.Condition;
                        var isOk = true;
                        if (con.IsNullOrEmpty()) isOk = true;
                        else
                        {
                            con = GetText(con, GetTextType.UiItem);
                            isOk = con.ToLower() == "true" ? true : false;
                        }
                        if (isOk) ExitApplication();
                    }
                    else if (item.Type == (int)UiItemType.GoTo)
                    {
                        if (!value.IsNullOrEmpty())
                        {
                            var con = item.Condition;
                            var isOk = true;
                            if (con.IsNullOrEmpty()) isOk = true;
                            else
                            {
                                con = GetText(con, GetTextType.UiItem);
                                isOk = con.ToLower() == "true" ? true : false;
                            }

                            var goToStr = value;
                            var gotoName = goToStr;
                            //var gotoName = goToStr.ToLower() == "exit" ? "Exit" : goToStr;
                            //if (gotoName == "Exit") return;
                            var item1 = uiItems.Find(x => x.Name == gotoName);
                            if (item1 != null) id = item1.Id - 1;
                        }


                    }


                    id++;

                } while (id < len);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".Execute Error: UiItem.Name=" + itemNameEx + "; " + ex.Message);
            }
        }

        //*subfunc
        //*gettext
        //##GetText
        protected string GetText(string str, GetTextType getTextTp)
        {

            if (str.IsNullOrEmpty())
            {
                return string.Empty;
            }
            try
            {
                if (!str.StartsWith("="))
                {
                    if (getTextTp == GetTextType.ShellItem)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                        else if (str.IsShellElementDataFormat())
                            str = ResolveStringByRefShellVariables(str);
                    }
                    else if (getTextTp == GetTextType.UiItem)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                        else if (str.IsShellElementDataFormat())
                            str = ResolveStringByRefShellVariables(str);
                        else if (str.IsUiElementDataFormat())
                            str = ResolveStringByRefUiItems(str);

                    }
                    else if (getTextTp == GetTextType.OnlyResolveCed)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                    }
                    else { }

                    return str;
                }

                var getNameAndParamsStr = str.Substring(1, str.Length - 1).Trim();
                var getNameAndParamsArray = getNameAndParamsStr.GetFormularParamArray(false, false);
                var funcName = getNameAndParamsArray[0];

                var funcParamArray = new string[getNameAndParamsArray.Length - 1];
                for (int i = 0; i < getNameAndParamsArray.Length - 1; i++)
                {
                    funcParamArray[i] = getNameAndParamsArray[i + 1];
                }

                for (int i = 0; i < funcParamArray.Length; i++)
                {
                    var str1 = funcParamArray[i];
                    if (getTextTp == GetTextType.ShellItem)
                    {
                        if (str1.IsCenterExchangeDataFormat())
                            funcParamArray[i] = ResolveCentralExchangeData(str1);
                        else if (str1.IsShellElementDataFormat())
                            funcParamArray[i] = ResolveStringByRefShellVariables(str1);
                    }
                    else if (getTextTp == GetTextType.UiItem)
                    {
                        if (str1.IsCenterExchangeDataFormat())
                            funcParamArray[i] = ResolveCentralExchangeData(str1);
                        else if (str1.IsShellElementDataFormat())
                            funcParamArray[i] = ResolveStringByRefShellVariables(str1);
                        else if (str1.IsUiElementDataFormat())
                            funcParamArray[i] = ResolveStringByRefUiItems(str1);
                    }
                    else if (getTextTp == GetTextType.OnlyResolveCed)
                    {
                        if (str.IsCenterExchangeDataFormat())
                            str = ResolveCentralExchangeData(str);
                    }
                    else { }

                    var str2 = funcParamArray[i];
                    if (str2.StartsWith("="))
                    {
                        throw new ArgumentException("Nesting of formulas is not supported");
                    };
                }



                var returnText = Get(funcName, funcParamArray);
                return returnText;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".GetText Error: params=" + str + "; " + ex.Message);
            }

        }


        //*Transact
        private void TransactByElementName(string elementName, TransactFrom transFr)
        {
            var action = "";
            var writeIntoLog = false;
            var showRunningStatus = false;
            var displayName = "";
            ExecMode execMode = ExecMode.Sync;


            if (transFr == TransactFrom.FromShell)
            {
                var elementName1 = elementName.DeleteShellIdentifer();
                var item = _procedures.Find(x => x.Name == elementName1);
                if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".TransactByElementName Error: ProcessElementName={0} des not exist!", elementName));
                if (item.Expression.IsNullOrEmpty()) return;
                showRunningStatus = item.ShowRunningStatus.GetJudgementFlag() == "true";

                displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "Procedure", item.Name, _annexes, item.DisplayName);
                if (displayName.IsNullOrEmpty()) displayName = "#" + item.Name + "#";
                if (item.Expression.IsNullOrEmpty()) return;
                action = item.Expression;
                action = (action.IsCenterExchangeDataFormat() | action.IsShellElementDataFormat()) ? GetText(action, GetTextType.ShellItem) : action;
            }

            else if (transFr == TransactFrom.FromScenarioUi)
            {
                //if (IdentifierHelper.IsUiElementDataFormat(elementName))
                var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);
                var item = _uiItems.Find(x => x.Name == elementName1);
                if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".TransactByElementName Error: UiElementName={0} des not exist!", elementName));
                if (item.Value.IsNullOrEmpty()) return;
                writeIntoLog = item.WriteIntoLog.GetJudgementFlag() == "true";
                showRunningStatus = item.ShowRunningStatus.GetJudgementFlag() == "true";
                execMode = EnumHelper.GetByName<ExecMode>(item.ExecMode, execMode);

                displayName = CommonHelper.GetDisplayName(_formInitParamSet.SupportMultiLanguages, "UiItem", item.Name, _annexes, item.DisplayName);
                if (displayName.IsNullOrEmpty()) displayName = "$" + item.Name + "$";

                action = item.Value;
                action = (action.IsCenterExchangeDataFormat() | action.IsShellElementDataFormat() | action.IsUiElementDataFormat()) ? GetText(action, GetTextType.UiItem) : action;

            }
            else return;

            if (action.IsNullOrEmpty()) return;
            if (action == "DoNothing") return;

            var transParams = ParseAction(action, transFr);
            var transactionDetail = new TransactionDetail() { Name = transParams.Name, Params = transParams.Params, DisplayName = displayName, ExecMode = execMode, ShowRunningStatus = showRunningStatus, WriteIntoLog = writeIntoLog };
            try
            {
                Transact(transactionDetail);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".TransactByElementName Error: elementName={0}; action={1} " + ex.Message, elementName, action));
            }

        }

        private TransactionParams ParseAction(string action, TransactFrom transFr)
        {
            var actionNameAndParamsArray = action.GetFormularParamArray(false, false);
            var actionName = actionNameAndParamsArray[0];
            var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
            for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
            {
                actionParamArray[i] = actionNameAndParamsArray[i + 1];
            }
            var transName = ParseActionName(actionName, transFr);
            if (transName == "Xrun")
            {
                var transParams = new TransactionParams();
                transParams.Name = transName;
                transParams.Params = actionParamArray;
                return transParams;
            }

            if (transName == "Crun")
            {
                var con = actionParamArray[0];
                con = ParseActionName(con, transFr);
                var action1 = "";
                if (con.JudgeJudgementFlag())
                {
                    action1 = actionParamArray[1];
                }
                else
                {
                    if (actionParamArray.Length > 2)
                        action1 = actionParamArray[2];
                    else action1 = "DoNothing";
                }
                return ParseAction(action1, transFr);
            }
            else
            {
                for (int i = 0; i < actionParamArray.Length; i++)
                {
                    actionParamArray[i] = actionNameAndParamsArray[i + 1];
                }
                var actionParamArray1 = ParseActionParams(actionParamArray, transFr);
                var transParams = new TransactionParams();
                transParams.Name = transName;
                transParams.Params = actionParamArray1;
                return transParams;
            }
        }

        private string ParseActionName(string funcName, TransactFrom transFr)
        {
            if (transFr == TransactFrom.FromScenarioUi)
                funcName = GetText(funcName, GetTextType.UiItem);
            else if (transFr == TransactFrom.FromShell)
                funcName = GetText(funcName, GetTextType.ShellItem);
            else funcName = GetText(funcName, GetTextType.OnlyResolveCed);
            return funcName;
        }

        private string[] ParseActionParams(string[] actionParamArray, TransactFrom transFr)
        {

            for (int i = 0; i < actionParamArray.Length; i++)
            {
                if (transFr == TransactFrom.FromScenarioUi)
                    actionParamArray[i] = GetText(actionParamArray[i], GetTextType.UiItem);
                else if (transFr == TransactFrom.FromShell)
                    actionParamArray[i] = GetText(actionParamArray[i].Trim(), GetTextType.ShellItem);
                else actionParamArray[i] = GetText(actionParamArray[i].Trim(), GetTextType.OnlyResolveCed);

            }
            return actionParamArray;
        }


        ///////////////////////////////By Liggin2019 on 20200308////////////////////////////////////////
        //--Please optimize all try/catch before Release in front of this point, after full testing!!!//
        ////////////////////////////////////////////////////////////////////////////////////////////////
        //*common
        //**transact
        //###Transact
        private void Transact(TransactionDetail transactionDetail)
        {
            var exInfo = "\n>> " + GetType().FullName + ".Tansact Error: ";

            var displayName = transactionDetail.DisplayName;
            var funcName = transactionDetail.Name;
            var funcParamArray = transactionDetail.Params;
            var showRunningStatus = transactionDetail.ShowRunningStatus;
            var writeIntoLog = transactionDetail.WriteIntoLog;
            var execMode = transactionDetail.ExecMode;
            if (showRunningStatus)
            {
                Console.WriteLine(TextRes.Dispensing + " " + displayName + " " + TextRes.PleaseWait + "...");
            }

            try
            {
                if (execMode == ExecMode.Sync)
                {
                    Do(funcName, funcParamArray);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".Transact Error: funcName={0}; " + ex.Message, funcName));
                //--write into error log
            }

            if (writeIntoLog)
            {
                //--write into transaction log
            }
        }

        //###DoInThreadPool
        protected object DoInThreadPool(Object objParams)
        {
            var transactionParams = objParams as TransactionParams;
            Do(transactionParams.Name, transactionParams.Params);
            return null;
        }

        //*subcommon
        //*get
        //####Get
        private string Get(string funcName, string[] funcParamArray)
        {

            var exInfo = GetType().FullName + ".Get Error: ";
            if (funcName == "Equal")
            {
                return funcParamArray[0];
            }

            else if (funcName == "GetInitParams")
            {
                return GetEx(funcName, funcParamArray);
            }
            else if (funcName == "GetAnnexText")
            {

                var className = funcParamArray[0];
                var masterName = funcParamArray[1];
                masterName = masterName.IsShellElementDataFormat() ? masterName.DeleteShellIdentifer() : masterName;
                masterName = masterName.IsUiElementDataFormat() ? masterName.DeleteUiIdentifer() : masterName;
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 2)
                {
                    var annexTypeStr = funcParamArray[2];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var deftext = funcParamArray.Length > 3 ? funcParamArray[3] : "";
                var tempStr = "";
                if (_formInitParamSet.SupportMultiLanguages)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.StepByStep, deftext);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.CurrentOrFirst, deftext);
                }
                return tempStr;
            }
            else if (funcName == "GetSharedAnnexText")
            {
                var className = "Shared";
                var masterName = funcParamArray[0];
                var annexType = AnnexTextType.DisplayName;

                var tempStr = "";
                if (_formInitParamSet.SupportMultiLanguages)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.Current, masterName);
                }
                else
                {
                    tempStr = masterName;
                }
                return tempStr;
            }

            else if (funcName == "GetApplicationAnnexText")
            {
                var className = "Application";
                var masterName = _formInitParamSet.ApplicationCode;
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 0)
                {
                    var annexTypeStr = funcParamArray[0];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var deftext = funcParamArray.Length > 1 ? funcParamArray[1] : "";

                var tempStr = "";
                if (_formInitParamSet.SupportMultiLanguages)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.StepByStep, deftext);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, GetAnnexMode.CurrentOrFirst, deftext);
                }
                return tempStr;
            }

            else if (funcName == "ValidateInput")
            {
                var uiItemName = funcParamArray[0];
                var uiItemValue = GetUiItemValue(uiItemName + ".v");
                var uiItemValidateRules = _uiItems.Find(x => x.Name == uiItemName).ValidationRules;

                if (uiItemValidateRules.IsNullOrEmpty())
                {
                    return GenericHelper.ConvertToJson(new UniversalResult(true, ""));
                }
                else
                {
                    var ruleArry = uiItemValidateRules.GetLarrayArray(true, true);
                    foreach (var rule in ruleArry)
                    {
                        var getParamArray1 = new string[] { uiItemValue, rule };
                        var rstTxt = Getter.Get("Validate", getParamArray1);

                        if (rstTxt == "LRDUNDEFINED")
                        {
                            rstTxt = ValidateEx(uiItemValue, rule);
                        }

                        var rst = rstTxt.ConvertToGeneric<UniversalResult>(true, TxtDataType.Json);
                        if (rst.Success) continue;
                        else
                        {
                            return GenericHelper.ConvertToJson(rst);
                        }

                    }
                }
                return GenericHelper.ConvertToJson(new UniversalResult(true, ""));
            }

            else
            {
                var returnText = Getter.Get(funcName, funcParamArray);
                if (returnText == "LRDUNDEFINED")
                {
                    returnText = GetEx(funcName, funcParamArray);
                }
                return returnText;
            }

        }

        //**do
        private void Do(string funcName, string[] funcParamArray)
        {

            var exInfo = GetType().FullName + ".Do Error: ";
            if (funcName.IsNullOrEmpty()) return;
            if (funcName == "DoNothing") return;
            if (funcName == "ThrowException") throw new ArgumentException(exInfo + ".Do Error: " + funcParamArray[0]);

            var returnStr = "";
            if (funcName == "Xrun")
            {
                var actionParams = IdentifierHelper.UnwrapFormularParamArray(funcParamArray);
                Xrun(actionParams);
            }
            else if (funcName == "Lrun")
            {
                var actionParams = IdentifierHelper.UnwrapFormularParamArray(funcParamArray);
                Lrun(actionParams);
            }

            //--form
            else if (funcName == "ReturnTrue")
            {
                BoolOutput = true;
            }
            else if (funcName == "ReturnFalse")
            {
                BoolOutput = false;
            }
            else if (funcName == "ReturnFalseAndExit")
            {
                BoolOutput = false;
                ExitApplication();
            }
            else if (funcName == "ReturnTrueAndExit")
            {
                BoolOutput = true;
                ExitApplication();
            }
            else if (funcName == "ExitApplication")
            {
                ExitApplication();
            }
            else if (funcName == "SetFormTitle")
            {
                Console.Title = funcParamArray[0];
            }
            else if (funcName == "Sleep")
            {
                var duration = 1000;
                if (funcParamArray.Length > 0) duration = Convert.ToInt16(funcParamArray[0]);
                System.Threading.Thread.Sleep(duration);
            }
            else if (funcName == "ClearConsole")
            {
                Console.Clear();
            }
            else if (funcName == "NewLine")
            {
                var value = funcParamArray[0];
                if (value.IsNullOrEmpty())
                {
                    Console.WriteLine();
                }
                else
                {
                    if (value.IsPlusInteger())
                    {
                        var times = Convert.ToInt32(value);
                        for (var i = 0; i < times; i++)
                        {
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }
            else if (funcName == "Write")
            {
                var colorStr = "";
                if (funcParamArray.Length > 1) colorStr = funcParamArray[1];
                if (!colorStr.IsNullOrEmpty()) SetConsoleForegroundColor(colorStr);
                Console.Write(funcParamArray[0]);
                if (!colorStr.IsNullOrEmpty()) Console.ForegroundColor = ConsoleColor.White;
            }
            else if (funcName == "WriteLine")
            {
                var colorStr = "";
                if (funcParamArray.Length > 1) colorStr = funcParamArray[1];
                if (!colorStr.IsNullOrEmpty()) SetConsoleForegroundColor(colorStr);
                Console.WriteLine(funcParamArray[0]);
                if (!colorStr.IsNullOrEmpty()) Console.ForegroundColor = ConsoleColor.White;
            }
            else if (funcName == "WriteSpace")
            {
                var value = funcParamArray[0];
                if (value.IsNullOrEmpty())
                {
                    Console.Write(" ");
                }
                else
                {
                    if (value.IsPlusInteger())
                    {
                        var times = Convert.ToInt32(value);
                        for (var i = 0; i < times; i++)
                        {
                            Console.Write(" ");
                        }
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
            }
            else if (funcName == "Beep")
            {
                var value = funcParamArray[0];
                Beep(value);
            }

            else if (funcName == "PressAnyKeyToExit")
            {
                Console.Write(TextRes.PressAnyKeyToExit);
                Console.ReadKey();
                ExitApplication();
            }
            //--shell
            else if (funcName == "RefreshProcedures")
            {
                var procs = funcParamArray[0];
                var procArr = procs.GetLarrayArray(true, true);
                foreach (var proc in procArr)
                {
                    var procName = proc.DeleteShellIdentifer();
                    RefreshProcedure(procName);
                }
            }

            else if (funcName == "SetVariableValue")
            {
                var varName = funcParamArray[0].DeleteShellIdentifer();
                var varValue = funcParamArray[1].Trim();
                var procList = _procedures.FindAll(x => x.Type == (int)ProcedureType.Variable);
                ShellHelper.SetVariableValue(varName, varValue, _procedures);
            }


            else if (funcName == "UpdateCed")
            {
                DoEx(funcName, funcParamArray);
            }
            else//*to do by dispacher
            {

                returnStr = Doer.Do(funcName, funcParamArray);
                if (returnStr == "LRDUNDEFINED")
                {
                    DoEx(funcName, funcParamArray);
                }
            }
            if (funcName == "ExecCmds" | funcName == "ExecCmd" | funcName == "RunBat" | funcName == "RunPython")
            {
                var interceptOutput = funcParamArray.Length > 3 ? funcParamArray[3].JudgeJudgementFlag() : false;
                var saveOutput = interceptOutput;
                if (saveOutput)
                {
                    var title = funcName;
                    for (int i = 0; i < funcParamArray.Length; i++)
                    {
                        title = title + "-" + funcParamArray[i];
                    }
                    title = title + "-";
                    if (title.Length > 64) title = title.Substring(0, 64);
                    title = title.ToLegalFileName();
                    var content = GetHelper.FormatRichText(returnStr);
                    var outputDir = _appOutputDir + "\\ProcessLog\\Console";

                    DirectoryHelper.CreateDirectory(outputDir);
                    var outputPath = outputDir + "\\" + title.ToUniqueStringByNow("-") + ".txt";
                    File.WriteAllText(outputPath, content);
                }
            }
        }

        //*xrun
        //###xrun
        private void Xrun(string actParams)
        {
            var exInfo = "\n>> " + GetType().FullName + ".Xrun Error: ";
            var elementNameArr = actParams.GetLarrayArray(true, true);
            foreach (var elementName in elementNameArr)
            {

                var action = "";
                if (IdentifierHelper.IsShellElementDataFormat(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteShellIdentifer(elementName);
                    var item = _procedures.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "ProcessElementName={0} des not exist!", elementName));
                    if (item.Expression.IsNullOrEmpty()) continue;
                    action = item.Expression;
                }
                else if (IdentifierHelper.IsUiElementDataFormat(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);
                    var item = _uiItems.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "UiElementName={0} des not exist!", elementName));
                    if (item.Value.IsNullOrEmpty()) continue;
                    action = item.Value;
                }

                var actionNameAndParamsArray = action.GetFormularParamArray(true, false);
                var funcName = actionNameAndParamsArray[0].Trim();
                var funcParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    funcParamArray[i] = GetText(actionNameAndParamsArray[i + 1].Trim(), GetTextType.UiItem); //max possible
                }

                if (funcName == "Crun")
                {
                    var con = funcParamArray[0];
                    con = GetText(con, GetTextType.UiItem); //max possible
                    var action1 = "";
                    if (con.JudgeJudgementFlag())
                    {
                        action1 = funcParamArray[1];
                    }
                    else
                    {
                        if (funcParamArray.Length > 2)
                            action1 = funcParamArray[2];
                        else action1 = "DoNothing";
                    }
                    var transParams = ParseAction(action1, TransactFrom.FromScenarioUi); ////max possible
                    funcName = transParams.Name;
                    funcParamArray = transParams.Params;
                }

                try
                {
                    Do(funcName, funcParamArray);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(exInfo + "elementName=" + elementName + "; " + " action= " + action + "; " + ex.Message);
                }

            }
        }

        private void Lrun(string funcParams)
        {
            if (!IdentifierHelper.ContainsLrunIdentifer(funcParams)) return;
            var strArray = funcParams.GetFormularParamArray(true, false);
            var funcName = strArray[0];
            var funcParamsAndReplaceVals = strArray[1];
            var funcParamsAndReplaceValsArry = funcParamsAndReplaceVals.GetFormularParamArray(true, false);
            var transParams = funcParamsAndReplaceValsArry[0];
            var transParamsArry = transParams.GetFormularParamArray(true, false);
            var ReplaceVals = funcParamsAndReplaceValsArry[1];
            var ReplaceValsArry = ReplaceVals.GetLarrayArray(true, false);

            foreach (var lrunParam in ReplaceValsArry)
            {
                foreach (var param in transParamsArry)
                {
                    if (IdentifierHelper.ContainsLrunIdentifer(param))
                        param.Replace(IdentifierHelper.LrunIdentifer, lrunParam);
                }
                Do(funcName, transParamsArry);
            }
        }

        //*shell

        private void RefreshProcedures(List<Procedure> procList)
        {
            foreach (var proc in procList)
            {
                var toDo = true;
                if (toDo)
                {
                    if (proc.Type == (int)ProcedureType.Variable)
                    {
                        if (!proc.Expression.IsNullOrEmpty())
                            if (!string.IsNullOrEmpty(proc.Expression))
                            {
                                proc.Value = GetText(proc.Expression, GetTextType.ShellItem);
                            }
                    }
                    else if (proc.Type == (int)ProcedureType.Transaction)
                    {
                        if (!string.IsNullOrEmpty(proc.Expression))
                        {
                            TransactByElementName("#" + proc.Name + "#", TransactFrom.FromShell);
                        }
                    }
                }
            }
        }
        protected virtual void ExitShell() { }

        private void RefreshProcedure(string procName)
        {

            var proc = _procedures.Find(x => x.Name == procName);
            if (proc == null)
            {
                return;
            }
            var toDo = true;
            if (toDo)
            {
                if (proc.Type == (int)ProcedureType.Variable)
                {
                    if (!proc.Expression.IsNullOrEmpty())
                        if (!string.IsNullOrEmpty(proc.Expression))
                        {
                            proc.Value = GetText(proc.Expression, GetTextType.ShellItem);
                        }
                }
                else if (proc.Type == (int)ProcedureType.Transaction)
                {
                    if (!string.IsNullOrEmpty(proc.Expression))
                    {
                        TransactByElementName("#" + proc.Name + "#", TransactFrom.FromShell);
                    }
                }
            }
        }

        //###GetUiItemValue
        private string GetUiItemValue(string txt)
        {
            var name = txt;
            if (!txt.ToLower().EndsWith(".v")) return string.Empty;
            name = txt.Split('.')[0];
            var uiItem = _uiItems.Find(x => x.Name == name);
            if (uiItem != null) return uiItem.Value;
            return string.Empty;
        }

        private void Beep(string value)
        {
            if (value.IsNullOrEmpty())
            {
                Console.Beep();
            }
            else
            {
                var paramArry = value.GetLarrayArray(true, true);
                var frequency = 37;//32768
                var duration = 1000;
                if (paramArry[0].IsPlusInteger())
                {
                    frequency = Convert.ToInt32(paramArry[0]);
                    if (frequency < 37) frequency = 37;
                    if (frequency > 32767) frequency = 32767;
                }

                if (paramArry.Length > 1)
                {
                    if (paramArry[0].IsPlusInteger())
                        duration = Convert.ToInt32(paramArry[1]);
                }
                Console.Beep(frequency, duration);
            }
        }

        private void SetConsoleForegroundColor(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                Console.ForegroundColor = EnumHelper.GetByName<ConsoleColor>(value, ConsoleColor.White);
            }
        }

        protected void ExitApplication()
        {
            System.Environment.Exit(-1);
        }


        //*resolve
        //###ResolveConstants
        protected string ResolveConstants(string text)
        {
            if (text.IsNullOrEmpty()) return string.Empty;
            if (!text.Contains("%")) return text;

            var toBeRplStr = "";
            toBeRplStr = "%ArchCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ArchitectureCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ArchitectureName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ArchitectureVersion;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%OrgCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.OrganizationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgShortName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.OrganizationShortName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.OrganizationName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ApplicationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formInitParamSet.ApplicationVersion;
                return Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%FunctionCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%FormCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _formCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //-dir
            toBeRplStr = "%RootDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _startupDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%CfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _cfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //--golbal dir
            toBeRplStr = "%GlbCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbSharedDir%".ToLower(); //tobecancelled
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbSharedDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbUiDir%".ToLower(); //tobecancelled
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbUiDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            //--app dir
            toBeRplStr = "%AppCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppSharedDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appSharedDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppUiDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appUiDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //toBeRplStr = "%ScenariosDir%".ToLower();
            //if (text.ToLower().Contains(toBeRplStr))
            //{
            //    var rplStr = _snrsDir;
            //    text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            //}
            toBeRplStr = "%SnrsDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _snrsDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            //toBeRplStr = "%ScenarioDir%".ToLower();
            //if (text.ToLower().Contains(toBeRplStr))
            //{
            //    var rplStr = _snrDir;
            //    text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            //}
            toBeRplStr = "%SnrDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _snrDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //--data dir
            toBeRplStr = "%DataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _dataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%sysAppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _sysAppDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            //--lib dir
            toBeRplStr = "%LibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _libDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%GlbLibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _glbLibDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            toBeRplStr = "%AppLibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appLibDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            //--tmp dir
            toBeRplStr = "%AppOutputDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appOutputDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }
            if (!text.Contains("%"))
            {
                return text;
            }
            else
            {
                var retStr = Resolver.ResolveConstants(text);
                if (!retStr.Contains("%"))
                {
                    return retStr;
                }
                else
                {
                    var retStr1 = ResolveConstantsEx(text);
                    if (!retStr1.Contains("%")) return retStr1;
                    else
                        throw new ArgumentException("\n>> " + string.Format(".ResolveConstants Error: {0}'" + retStr1 + "'; ", "There are constants not defined!, str="));
                }
            }
        }

        private string ResolveStringByRefShellVariables(string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return "";
                if (str.IsShellElementDataFormat())
                {
                    //str = ShellHelper.ResolveStringByRefVariables(str, _procedures.Where(x => x.Type == (int)ProcedureType.Args | x.Type == (int)ProcedureType.Variable).ToList());
                    var procedures = _procedures.Where(x => x.Type == (int)ProcedureType.Args | x.Type == (int)ProcedureType.Variable).ToList();
                    var annexes = _annexes.Where(x => x.ClassName == "Procedure").ToList();
                    str = ShellHelper.ResolveStringByRefVariables(str, _formInitParamSet.SupportMultiLanguages, procedures, annexes);

                }
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefShellVariables Error: " + ex.Message);
            }
        }
        private string ResolveStringByRefUiItems(string str)
        {
            try
            {
                str.CheckUiElementDataFormat();
                var strArray = str.Split(IdentifierHelper.UiIdentifer.ToChar());
                var i = 1;
                var txt = strArray[1];
                if (strArray[i].ToLower().EndsWith(".v"))
                {
                    strArray[i] = GetUiItemValue(txt);
                }
                else
                {
                    strArray[i] = IdentifierHelper.UiIdentifer + strArray[i] + IdentifierHelper.UiIdentifer;
                }

                return strArray[i];
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefUiItems Error: str='" + str + "'; " + ex.Message);
            }
        }

        private string ResolveCentralExchangeData(string str)
        {
            try
            {
                //str.CheckCenterExchangeDataFormat();
                //var strArray = str.ExtractSubStringsByTwoDifferentItentifiers(IdentifierHelper.CentralDataExchangeDataSeparators[0], IdentifierHelper.CentralDataExchangeDataSeparators[1], false);
                //var val = GetCentralExchangeData(strArray[0]);
                //return val;
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveCenteExchangeData Error: str='" + str + "'; " + ex.Message);
            }
        }


        //*virtual
        //####ResolveConstantsEx
        protected virtual string ResolveConstantsEx(string text)
        {
            return text;
        }
        protected virtual string ValidateEx(string code, string rule)
        {
            return string.Empty;
        }


        protected virtual string GetEx(string funName, string[] paramArray)
        {
            return string.Empty;
        }

        //####DoEx
        protected virtual void DoEx(string funcName, string[] funcParamArray)
        {
        }


        //*end
    }
}

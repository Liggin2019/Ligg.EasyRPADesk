
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ligg.EasyWinApp.WinCnsl.DataModel;
using Ligg.EasyWinApp.WinCnsl.Helpers;
using Ligg.EasyWinApp.WinCnsl.DataModel.Enums;

using Ligg.EasyWinApp.Interface;
using Ligg.Infrastructure.Base.DataModel;
using Ligg.Infrastructure.Base.DataModel.Enums;
using Ligg.Infrastructure.Base.Extension;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.Handlers;
using Ligg.EasyWinApp.Resources;

using Ligg.EasyWinApp.Parser.DataModel;
using Ligg.EasyWinApp.Parser.DataModel.Enums;
using Ligg.EasyWinApp.Parser;
using Ligg.EasyWinApp.Parser.Helpers;

//*start
namespace Ligg.EasyWinApp
{
    public class FunctionForm
    {
        public bool BoolValue = true;

        private ICblpAdapter _adapter = null;
        private FormInitParamSet _functionInitParamSet = null;


        private string _startupDir;
        private string _cfgDir;

        private string _appCfgDir;
        private string _appCfgUiDir;
        private string _scenariosDir;
        private string _scenarioDir;

        private string _appLibDir;
        private string _appDataDir;
        private string _appTempDir;

        private string _sysDataDir;
        private string _sysAppDataDir;
        private string _myDataDir;
        private string _myAppDataDir;
        private string _myAppScenarioDataDir;

        private List<Procedure> _procedures = new List<Procedure>();
        private List<Annex> _annexes = new List<Annex>();
        private List<UiItem> _uiItems = new List<UiItem>();
        private ManagedThreadPool _threadPool;

        internal FunctionForm(FormInitParamSet functionInitParamSet)
        {

            _functionInitParamSet = functionInitParamSet;
            _adapter = BootStrapper.Adapter;
            try
            {
                Load();
                Execute();
            }
            catch (Exception ex)
            {
                var basicInfoForException = "\n-Scenario: " + _functionInitParamSet.StartScenarioRelativeLocation;
                var additionalInfoForException = "Architecture: " + _functionInitParamSet.ArchitectureCode + "-" + _functionInitParamSet.ArchitectureVersion + "; " +
                    "Application: " + _functionInitParamSet.ApplicationCode + "-" + _functionInitParamSet.ApplicationVersion + "; " +
                    (CentralData.UserCode.IsNullOrEmpty() ? "" : "; UserCode: " + CentralData.UserCode) + "\n--" +
                    "Please send this error information to: " + _functionInitParamSet.HelpdeskEmail + "\n";

                throw new ArgumentException(basicInfoForException + " Error: " + ex.Message + "\n--" + additionalInfoForException);

            }
        }

        //*proc
        //**load
        private void Load()
        {
            _startupDir = Directory.GetCurrentDirectory();
            _cfgDir = _startupDir + "\\Conf";

            _appCfgDir = _cfgDir + "\\Apps\\" + _functionInitParamSet.ApplicationCode;
            _appCfgUiDir = _appCfgDir + "\\UIs\\Console";
            _scenariosDir = _appCfgUiDir + "\\Scenarios";
            _scenarioDir = _appCfgUiDir + "\\Zones";

            _scenarioDir = FileHelper.GetFilePath(_functionInitParamSet.StartScenarioRelativeLocation, _scenariosDir);
            _functionInitParamSet.ScenarioCode = FileHelper.GetFileDetailByOption(_scenarioDir, FilePathComposition.FileTitle);

            _appDataDir = _functionInitParamSet.ApplicationDataDir;
            _appLibDir = _functionInitParamSet.ApplicationLibDir;
            _appTempDir = _functionInitParamSet.ApplicationTempDir;
            _sysDataDir = DirectoryHelper.GetSpecialDir("commonapplicationdata") + "\\" + _functionInitParamSet.ArchitectureCode;
            _sysAppDataDir = _sysDataDir + "\\Apps\\" + _functionInitParamSet.ApplicationCode;

            _myDataDir = DirectoryHelper.GetSpecialDir("mydocuments") + "\\" + _functionInitParamSet.ArchitectureCode;
            _myAppDataDir = _myDataDir + "\\Apps\\" + _functionInitParamSet.ApplicationCode;
            _myAppScenarioDataDir = _myAppDataDir + "\\Scenarios";

            LoadForm();
        }

        //*func
        //**loadform
        private void LoadForm()
        {
            try
            {
                _annexes.AddRange(FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\AbbrevAnnexes", "Abbrev", false));
                _annexes.AddRange(FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\PhraseAnnexes", "Phrase", false));

                _annexes.AddRange(FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\AbbrevAnnexes", "Abbrev", false));
                _annexes.AddRange(FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\PhraseAnnexes", "Phrase", false));

                var annexes = FunctionHelper.GetAnnexesFromCfgFile(_appCfgDir + "\\ApplicationAnnexes", "", false);
                FunctionHelper.SetApplicationAnnexes(annexes, _functionInitParamSet.ApplicationCode);
                _annexes.AddRange(annexes);

                var annexes1 = FunctionHelper.GetAnnexesFromCfgFile(_scenarioDir + "\\FormTitleAnnexes", "", false);
                FunctionHelper.SetFormTitleAnnexes(annexes1, _functionInitParamSet.ScenarioCode);
                _annexes.AddRange(annexes1);

                //--initLayout starts
                var formStyle = FunctionHelper.GetGenericFromCfgFile<FormStyle>(_scenarioDir + "\\FormStyle", false) ?? new FormStyle();

                if (!_functionInitParamSet.InvisibleFlag.IsNullOrEmpty())
                {
                    if (_functionInitParamSet.InvisibleFlag.ToLower() == "false") formStyle.Invisible = false;
                    if (_functionInitParamSet.InvisibleFlag.ToLower() == "true") formStyle.Invisible = true;
                }

                Console.Title = _functionInitParamSet.ScenarioCode.ToUniqueStringByNow("-");
                if (formStyle.Invisible)
                    ConsoleHelper.HideWindow(Console.Title);
                if (formStyle.CannotBeClosed)
                    ConsoleHelper.DisbleClosebtn(Console.Title);

                if (formStyle.DrawIcon)
                {
                    var iconUrl = FileHelper.GetFilePath(formStyle.IconUrl, _scenarioDir);
                    if (!iconUrl.IsNullOrEmpty() && System.IO.File.Exists(iconUrl))
                    {
                        ConsoleHelper.SetIcon(iconUrl);
                    }
                }

                if (_functionInitParamSet.FormTitle.IsNullOrEmpty())
                {
                    var text = "";
                    if (_functionInitParamSet.SupportMultiCultures)
                    {
                        text = AnnexHelper.GetText("Function", _functionInitParamSet.ScenarioCode, _annexes, AnnexTextType.DisplayName, CultureHelper.CurrentLanguageCode, GetAnnexMode.OnlyByCurLang);
                    }
                    else
                    {
                        text = AnnexHelper.GetText("Function", _functionInitParamSet.ScenarioCode, _annexes, AnnexTextType.DisplayName, CultureHelper.CurrentLanguageCode, GetAnnexMode.FirstAnnex);
                    }

                    text = !text.IsNullOrEmpty() ? text : _functionInitParamSet.ApplicationCode + "-" + _functionInitParamSet.ScenarioCode;
                    Console.Title = text;
                }
                else
                {
                    Console.Title = _functionInitParamSet.FormTitle;
                }

                var supportsMultipleThreads = formStyle.SupportsMultipleThreads;
                var threadPoolMaxNum = formStyle.ThreadPoolMaxNum;
                if (supportsMultipleThreads)
                    _threadPool = new ManagedThreadPool(threadPoolMaxNum);
                //--initLayout ends

                //--merge
                MergeScenarioProcedures();
                MergeUiItems();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".LoadForm Error: " + ex.Message);
            }
        }

        //**merge
        //**process
        private List<Procedure> MergeScenarioProcedures()
        {
            var exInfo = "\n>> " + GetType().FullName + ".MergeScenarioProcedures Error: ";

            var scenarioProcedures = new List<Procedure>();
            var startParamsTxt = _functionInitParamSet.StartScenarioProcessParams;
            if (!startParamsTxt.IsNullOrEmpty())
            {
                var startParamsTxtArry = startParamsTxt.GetSubParamArray(true, false);
                var finputStr = "";
                for (var i = 0; i < startParamsTxtArry.Length; i++)
                {
                    var tmp = startParamsTxtArry[i];
                    if (!tmp.IsNullOrEmpty())
                    {
                        tmp = ResolveConstants(tmp);
                        tmp = GetText(tmp);
                    }
                    startParamsTxtArry[i] = tmp;
                }

                finputStr = IdentifierHelper.UnwrapSubParamArray(startParamsTxtArry);
                var variableItem = new Procedure();
                variableItem.Name = "finput";
                variableItem.Value = finputStr;
                variableItem.Type = (int)ProcedureType.Params;
                variableItem.DisplayName = "";
                variableItem.ExecModeFlag = "";
                variableItem.ExecOnInitFlag = "";
                variableItem.Formula = "";
                variableItem.ShowRunningStatusFlag = "";
                scenarioProcedures.Add(variableItem);
            }

            var scenarioProceduresTmp = new List<Procedure>();
            var cfgFile = _scenarioDir + "\\Process";
            try
            {
                scenarioProceduresTmp = FunctionHelper.GetGenericFromCfgFile<List<Procedure>>(_scenarioDir + "\\Process", false) ?? new List<Procedure>();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }

            var scenarioProceduresTmp1 = new List<Procedure>();
            foreach (var proc in scenarioProceduresTmp)
            {
                proc.InvalidFlag = ResolveConstants(proc.InvalidFlag);
                if (proc.InvalidFlag.StartsWith("=")) proc.InvalidFlag = GetText(proc.InvalidFlag);
                proc.InvalidFlag = proc.InvalidFlag.GetJudgementFlag();
                if (proc.InvalidFlag.ToLower() == "true")
                {
                    continue;
                }

                if (!proc.Formula.IsNullOrEmpty())
                    if (IdentifierHelper.ContainsUiIdentifer(proc.Formula))
                    {
                        throw new ArgumentException(exInfo + "Procedure can't contain " + IdentifierHelper.UiIdentifer);
                        //continue;
                    }
                scenarioProceduresTmp1.Add(proc);
            }

            scenarioProceduresTmp1 = scenarioProceduresTmp1.FindAll(x => x.TypeName == ProcedureType.Variable.ToString()
                                                              | x.TypeName == ProcedureType.Transaction.ToString() | x.TypeName == ProcedureType.SubTransaction.ToString()
                                                              | x.TypeName == ProcedureType.Break.ToString() | x.TypeName == ProcedureType.Exit.ToString());

            if (scenarioProceduresTmp1.Count > 0)
            {

                string annexesCfgFile = cfgFile + "Annexes";
                var annexList = new List<Annex>();
                try
                {
                    ProcessHelper.CheckProcedures(scenarioProceduresTmp1, _functionInitParamSet.ScenarioCode);
                    annexList = FunctionHelper.GetAnnexesFromCfgFile(annexesCfgFile, "Procedure", false);
                    foreach (var proc in scenarioProceduresTmp1)
                    {
                        ProcessHelper.SetProcedureType(proc);
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(exInfo + ex.Message);
                }

                foreach (var proc in scenarioProceduresTmp1)
                {
                    proc.WriteIntoLogFlag = string.IsNullOrEmpty(proc.WriteIntoLogFlag) ? "" : proc.WriteIntoLogFlag;
                    proc.WriteIntoLogFlag = ResolveConstants(proc.WriteIntoLogFlag);
                    proc.ExecOnInitFlag = string.IsNullOrEmpty(proc.ExecOnInitFlag) ? "" : proc.ExecOnInitFlag;
                    proc.ExecOnInitFlag = ResolveConstants(proc.ExecOnInitFlag);
                    proc.ExecModeFlag = string.IsNullOrEmpty(proc.ExecModeFlag) ? "" : proc.ExecModeFlag;
                    proc.ExecModeFlag = ResolveConstants(proc.ExecModeFlag);
                    proc.Value = string.IsNullOrEmpty(proc.Value) ? "" : proc.Value;
                    proc.Value = ResolveConstants(proc.Value);
                    proc.Formula = string.IsNullOrEmpty(proc.Formula) ? "" : proc.Formula;
                    proc.Formula = ResolveConstants(proc.Formula);
                    proc.Condition = string.IsNullOrEmpty(proc.Condition) ? "" : proc.Condition;
                    proc.Condition = ResolveConstants(proc.Condition);
                    if (proc.GroupId < 0) proc.GroupId = 0;
                    scenarioProcedures.Add(proc);

                    if (proc.Type != (int)ProcedureType.SubTransaction)
                    {
                        var tempAnnexes = annexList.FindAll(x => x.MasterName == proc.Name);
                        if (tempAnnexes.Count > 0)
                        {
                            foreach (var annex in tempAnnexes)
                            {
                                annex.ClassName = "Procedure";
                                annex.MasterName = proc.Name;
                                _annexes.Add(annex);
                            }
                        }
                    }
                }

            }

            if (scenarioProcedures.Count > 0)
            {
                _procedures.AddRange(scenarioProcedures);
            }
            return scenarioProcedures;

        }

        //**uiitems
        private void MergeUiItems()
        {
            var exInfo = "\n>> " + GetType().FullName + ".MergeUiItems Error: ";

            var location = FileHelper.GetFilePathByRelativeLocation(_functionInitParamSet.StartScenarioRelativeLocation, _scenariosDir);
            var tmpUiItems = new List<UiItem>();
            try
            {
                tmpUiItems = FunctionHelper.GetGenericFromCfgFile<List<UiItem>>(location + "\\ui", false) ?? new List<UiItem>();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }

            tmpUiItems = tmpUiItems.FindAll(x => x.TypeName == UiItemType.NewLine.ToString() |
                                                 x.TypeName == UiItemType.WriteSpace.ToString() | x.TypeName == UiItemType.GoTo.ToString() |
                                                 x.TypeName == UiItemType.ReadKey.ToString() | x.TypeName == UiItemType.ReadLine.ToString() |
                                                 x.TypeName == UiItemType.Write.ToString() | x.TypeName == UiItemType.WriteLine.ToString() |
                                                 x.TypeName == UiItemType.Transaction.ToString() | x.TypeName == UiItemType.SubTransaction.ToString());
            var uiItems = new List<UiItem>();

            foreach (var item in tmpUiItems)
            {
                item.InvalidFlag = item.InvalidFlag.GetJudgementFlag();
                if (item.InvalidFlag.ToLower() != "true")
                {
                    uiItems.Add(item);
                }
            }

            if (uiItems.Count > 0)
            {
                var annexList = new List<Annex>();
                try
                {
                    string cfgFile = location + "\\Annexes";
                    annexList = FunctionHelper.GetAnnexesFromCfgFile(cfgFile, "UiItem", false);
                    foreach (var item in uiItems)
                    {
                        UiHelper.SetUiItemType(item);
                        item.WriteIntoLogFlag = item.WriteIntoLogFlag.GetJudgementFlag();
                        item.ShowRunningStatusFlag = item.ShowRunningStatusFlag.GetJudgementFlag();
                        item.ExecModeFlag = string.IsNullOrEmpty(item.ExecModeFlag) ? "" : item.ExecModeFlag;

                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(exInfo + ex.Message);
                }


                foreach (var item in uiItems)
                {
                    var tempAnnexes = annexList.FindAll(x => x.MasterName == item.Name);
                    item.Value = string.IsNullOrEmpty(item.Value) ? "" : item.Value;
                    item.Value = ResolveConstants(item.Value);
                    _uiItems.Add(item);
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

            UiHelper.CheckUiItems(_uiItems);
        }

        //**exec
        private void Execute()
        {
            InitProcess(_procedures);

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
                        displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "UiItem", item.Name, _annexes, item.DisplayName);

                    var value = "";
                    if (item.Type == (int)UiItemType.Write | item.Type == (int)UiItemType.WriteLine | item.Type == (int)UiItemType.NewLine | item.Type == (int)UiItemType.WriteSpace)
                    {
                        value = ResolveStringByRefProcessVariablesAndUiItems(item.Value);
                        if (item.Value.StartsWith("="))
                            value = GetText(value);
                    }
                    if (item.Type == (int)UiItemType.GoTo)
                    {
                        value = ResolveStringByRefProcessVariablesAndUiItems(item.Value);
                    }
                    if (item.Type == (int)UiItemType.Transaction)
                    {
                        //do nothing
                    }

                    if (item.Type == (int)UiItemType.NewLine)
                    {
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
                    else if (item.Type == (int)UiItemType.WriteSpace)
                    {
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
                    else if (item.Type == (int)UiItemType.ReadKey)
                    {
                        var exit = true;
                        do
                        {
                            if (CultureHelper.CurrentLanguageCode == "szh" | CultureHelper.CurrentLanguageCode == "czh" |
                               CultureHelper.CurrentLanguageCode == "jpn" | CultureHelper.CurrentLanguageCode == "kr")
                                Console.Write(EasyWinAppRes.PressKey + "" + displayName + ": ");
                            else Console.Write(EasyWinAppRes.PressKey + " " + displayName + ": ");
                            var input = Console.ReadKey().KeyChar;
                            Console.WriteLine();
                            item.Value = input.ToString();
                            var validationRules = item.ValidationRules;

                            if (!validationRules.IsNullOrEmpty())
                            {
                                var rst = GetText("=ValidateInput^" + item.Name);
                                if (rst == "true") exit = true;
                                else
                                {
                                    exit = false;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(rst);
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
                            if (CultureHelper.CurrentLanguageCode == "szh" | CultureHelper.CurrentLanguageCode == "czh" |
                                CultureHelper.CurrentLanguageCode == "jpn" | CultureHelper.CurrentLanguageCode == "kr")
                                Console.Write(EasyWinAppRes.Input + "" + displayName + ": ");
                            else Console.Write(EasyWinAppRes.Input + " " + displayName + ": ");

                            var input = Console.ReadLine();
                            item.Value = input.ToString();
                            var validationRules = item.ValidationRules;

                            if (!validationRules.IsNullOrEmpty())
                            {
                                var rst = GetText("=ValidateInput^" + item.Name);
                                if (rst == "true") exit = true;
                                else
                                {
                                    exit = false;
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(rst);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                            }

                            //Console.WriteLine();
                        }
                        while (!exit);
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
                            ActByUiElementAndProcedure("$" + item.Name + "$");
                    }

                    else if (item.Type == (int)UiItemType.GoTo)
                    {
                        if (!value.IsNullOrEmpty())
                        {
                            var paramArry = value.GetParamArray(true, false);
                            var con = paramArry[0];
                            con = GetText(con);
                            var isOk = con.ToLower() == "true" ? true : false;

                            if (paramArry.Length > 1 & isOk)
                            {
                                var goToStr = paramArry[1];
                                var gotoName = goToStr.ToLower() == "exit" ? "Exit" : goToStr;
                                if (gotoName == "Exit") return;
                                var item1 = uiItems.Find(x => x.Name == gotoName);
                                if (item1 != null) id = item1.Id - 1;
                            }
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

        //##InitProcess
        private void InitProcess(List<Procedure> procedures)
        {
            var exInfo = "\n>> " + GetType().FullName + ".InitProcess Error: ";

            if (procedures.Count == 0) return;

            //var grpIds = procedures.Select(x => x.GroupId).Distinct();
            try
            {
                var procListByGrp = procedures.FindAll(x => x.GroupId == 0 & x.ExecOnInitFlag.ToLower() == "true" &
                x.Type != (int)ProcedureType.SubTransaction & x.Type != (int)ProcedureType.Params);
                RefreshProcedures(procListByGrp, procedures);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(exInfo + ex.Message);
            }
        }

        //*subfunc
        //**gettext
        //##GetText
        private string GetText(string str)
        {
            var funcName = "";
            var returnText = "";
            if (str.IsNullOrEmpty())
            {
                return string.Empty;
            }


            if (!str.StartsWith("="))
            {
                return str;
            }

            var repalceTabooIdentifers = false;
            var paramsStr = str.Substring(1, str.Length - 1).Trim();
            if (paramsStr.EndsWith("RTI") | paramsStr.ToLower().EndsWith("RepalceTabooIdentifers".ToLower()))
            {
                var tempParamArr = paramsStr.GetParamArray(true, false);
                paramsStr = tempParamArr[0];
                repalceTabooIdentifers = true;
            }

            var funNameAndParamsArray = paramsStr.GetParamArray(true, false);
            funcName = funNameAndParamsArray[0];
            var funcParamArray = new string[funNameAndParamsArray.Length - 1];
            for (int i = 0; i < funNameAndParamsArray.Length - 1; i++)
            {
                funcParamArray[i] = funNameAndParamsArray[i + 1];
            }
            try
            {
                Task task = new Task(() =>
            {

                returnText = GetText(funcName, funcParamArray);
                if (returnText == "OutOfScope")
                {
                    returnText = GetTextEx(funcName, funcParamArray);
                }

            });

                task.Start();
                task.Wait();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".GetText Error: params=" + str + "; " + ex.Message);
            }

            return repalceTabooIdentifers ? returnText.RepalceTabooIdentifers() : returnText;
        }

        //**act
        private void ActByUiElementAndProcedure(string elementName)
        {
            var action = "";
            var writeIntoLog = false;
            var showRunningStatus = false;
            var replacedIdentifiers = "";
            var displayName = "";
            TransactionExecMode execMode = TransactionExecMode.Sync;

            try
            {
                if (IdentifierHelper.IsProcessElement(elementName))
                {
                    var elementName1 = elementName.DeleteProcessIdentifer();
                    var item = _procedures.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".Act Error: ProcessElementName={0} does not exist!", elementName));
                    if (item.Formula.IsNullOrEmpty()) return;
                    action = item.Formula;
                    replacedIdentifiers = item.ReplacedTabooIdentifiers.IsNullOrEmpty() ? "" : item.ReplacedTabooIdentifiers;
                    if (!item.WriteIntoLogFlag.IsNullOrEmpty())
                    {
                        writeIntoLog = item.WriteIntoLogFlag.ToLower() == "true";
                    }
                    if (!item.ShowRunningStatusFlag.IsNullOrEmpty())
                    {
                        showRunningStatus = item.ShowRunningStatusFlag.ToLower() == "true";
                    }
                    if (!item.ExecModeFlag.IsNullOrEmpty())
                    {
                        execMode = EnumHelper.GetByName<TransactionExecMode>(item.ExecModeFlag, execMode);
                    }
                    displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "Procedure", item.Name, _annexes, item.DisplayName);
                    if (displayName.IsNullOrEmpty()) displayName = "#" + item.Name + "#";
                }
                else if (IdentifierHelper.IsUiElement(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);
                    var item = _uiItems.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".Act Error: UiElementName={0} des not exist!", elementName));
                    if (item.Value.IsNullOrEmpty()) return;
                    action = item.Value;
                    replacedIdentifiers = item.ReplacedTabooIdentifiers.IsNullOrEmpty() ? "" : item.ReplacedTabooIdentifiers;
                    if (!item.WriteIntoLogFlag.IsNullOrEmpty())
                    {
                        writeIntoLog = item.WriteIntoLogFlag.ToLower() == "true";
                    }
                    if (!item.ShowRunningStatusFlag.IsNullOrEmpty())
                    {
                        showRunningStatus = item.ShowRunningStatusFlag.ToLower() == "true";
                    }
                    if (!item.ExecModeFlag.IsNullOrEmpty())
                    {
                        execMode = EnumHelper.GetByName<TransactionExecMode>(item.ExecModeFlag, execMode);
                    }
                    displayName = FunctionHelper.GetDisplayName(_functionInitParamSet.SupportMultiCultures, "UiItem", item.Name, _annexes, item.DisplayName);
                    if (displayName.IsNullOrEmpty()) displayName = "$" + item.Name + "$";
                }
                else
                {
                    return;
                }

                if (action.IsNullOrEmpty()) return;
                if (action.ToLower() == "DoNothing".ToLower()) return;

                var actionNameAndParamsArray = action.GetParamArray(true, false);
                var actionName = actionNameAndParamsArray[0];
                actionName = ResolveStringByRefProcessVariablesAndUiItems(actionName);
                actionName = GetText(actionName);
                var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    actionParamArray[i] = actionNameAndParamsArray[i + 1];
                }
                actionParamArray = ParseActionParams(actionName, actionParamArray, replacedIdentifiers);

                var transactionDetail = new TransactionDetail() { ActionName = actionName, ActionParams = actionParamArray, DisplayName = displayName, ExecMode = execMode, ShowRunningStatus = showRunningStatus, WriteIntoLog = writeIntoLog };
                Transact(transactionDetail);

            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".ActByUiElementAndProcedure Error: elementName={0}; action={1} " + ex.Message, elementName, action));
            }

        }

        //--for alienProcess
        private void ActByTransaction(Transaction transaction)
        {
            var action = "";
            var writeIntoLog = false;
            var showRunningStatus = false;
            var displayName = "";
            TransactionExecMode execMode = TransactionExecMode.Sync;
            try
            {
                if (transaction != null)
                {
                    action = transaction.Action;
                    writeIntoLog = transaction.WriteIntoLogFlag.GetJudgementFlag() == "true";
                    showRunningStatus = transaction.ShowRunningStatusFlag.GetJudgementFlag() == "true";
                    if (!transaction.ExecModeFlag.IsNullOrEmpty())
                    {
                        execMode = EnumHelper.GetByName<TransactionExecMode>(transaction.ExecModeFlag, execMode);
                    }
                    displayName = transaction.DisplayName.IsNullOrEmpty() ? "" : transaction.DisplayName;
                }
                if (action.IsNullOrEmpty()) return;
                if (action.ToLower() == "DoNothing".ToLower()) return;

                var actionNameAndParamsArray = action.GetParamArray(true, false);
                var actionName = actionNameAndParamsArray[0];
                actionName = ResolveStringByRefProcessVariablesAndUiItems(actionName);
                actionName = GetText(actionName);
                var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    actionParamArray[i] = actionNameAndParamsArray[i + 1];
                }


                actionParamArray = ParseActionParams(actionName, actionParamArray, "");
                var transactionDetail = new TransactionDetail() { ActionName = actionName, ActionParams = actionParamArray, DisplayName = displayName, ExecMode = execMode, ShowRunningStatus = showRunningStatus, WriteIntoLog = writeIntoLog };
                Transact(transactionDetail);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".ActByTransaction Error: action={0} " + ex.Message, action));
            }
        }

        private string[] ParseActionParams(string actionName, string[] actionParamArray, string replacedIdentifiers)
        {
            try
            {
                if (actionName.ToLower() != "Xrun".ToLower())
                {
                    if (actionName.ToLower() == "Crun".ToLower())
                    {
                        actionParamArray[0] = GetText(ResolveStringByRefProcessVariablesAndUiItems(actionParamArray[0]));
                        actionParamArray[0] = actionParamArray[0].RecoverTabooIdentifers(replacedIdentifiers);
                    }
                    else
                    {
                        for (int i = 0; i < actionParamArray.Length; i++)
                        {
                            actionParamArray[i] = GetText(ResolveStringByRefProcessVariablesAndUiItems(actionParamArray[i]));
                            if (!replacedIdentifiers.IsNullOrEmpty())
                                actionParamArray[i] = actionParamArray[i].RecoverTabooIdentifers(replacedIdentifiers);
                        }
                    }

                }
                return actionParamArray;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("\n>> " + GetType().FullName + ".ParseActionParams Error: controlName={0}; " + ex.Message, actionName));
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////
        //--Please delete all try/catch before Release in front of this point, after full testing!!!//
        //////////////////////////////////////////////////////////////////////////////////////////////

        //**transact
        //##Transact
        private void Transact(TransactionDetail transactionDetail)
        {
            var exInfo = "\n>> " + GetType().FullName + ".Tansact Error: ";

            var displayName = transactionDetail.DisplayName;
            var actionName = transactionDetail.ActionName;
            var actionParamArray = transactionDetail.ActionParams;
            var showRunningStatus = transactionDetail.ShowRunningStatus;
            var writeIntoLog = transactionDetail.WriteIntoLog;
            var execMode = transactionDetail.ExecMode;
            if (showRunningStatus)
            {
                Console.WriteLine(EasyWinAppRes.Dispensing + " " + displayName + " " + EasyWinAppRes.PleaseWait + "...");
            }

            try
            {
                if (execMode == TransactionExecMode.Sync)
                {
                    Act(actionName, actionParamArray);
                }
                else if (execMode == TransactionExecMode.Async | execMode == TransactionExecMode.AsyncAwaited)
                {
                    Task task = new Task(() =>
                    {
                        Act(actionName, actionParamArray);
                    });
                    task.Start();
                    if (execMode == TransactionExecMode.AsyncAwaited) task.Wait();
                }
                else//managed thread
                {
                    if (_threadPool == null) throw new ArgumentException("Please set \"SupportsMultipleThreads and ThreadPoolMaxNum\" in form config file! ");
                    var transactionParams = new TransactionParams() { ActionName = transactionDetail.ActionName, ActionParams = transactionDetail.ActionParams };
                    _threadPool.Join(TransactInThreadPool, transactionDetail.DisplayName, transactionParams);
                }
            }
            catch (Exception ex)
            {
                //--write into error log
            }

            if (writeIntoLog)
            {
                //--write into transaction log
            }
        }

        //##ActInThreadPool
        private object TransactInThreadPool(Object objParams)
        {
            var transactionParams = objParams as TransactionParams;
            Act(transactionParams.ActionName, transactionParams.ActionParams);
            return null;
        }

        //*common
        //**gettext
        private string GetText(string funcName, string[] funcParamArray)
        {
            var returnText = "";

            if (funcName.ToLower() == "Equal".ToLower())
            {
                return funcParamArray[0];
            }
            else if (funcName.ToLower() == "GetCenterData".ToLower())
            {
                var dataName = funcParamArray[0];
                returnText = GetCenterData(dataName);
            }
            else if (funcName.ToLower() == "GetCommonParams".ToLower())
            {
                var id = "";
                if (funcParamArray.Length > 0)
                    id = funcParamArray[0];
                returnText = GetCommonParams(id);
            }
            else if (funcName.ToLower() == "GetAnnexText".ToLower())
            {
                var className = funcParamArray[0];
                var masterName = funcParamArray[1];
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 2)
                {
                    var annexTypeStr = funcParamArray[2];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var tempStr = "";
                if (_functionInitParamSet.SupportMultiCultures)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, CultureHelper.CurrentLanguageCode, GetAnnexMode.StepByStep);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, "", GetAnnexMode.FirstAnnex);
                }
                returnText = tempStr.IsNullOrEmpty() ? masterName : tempStr;
            }
            else if (funcName.ToLower() == "GetAbbrevAnnexText".ToLower())
            {
                var className = "Abbrev";
                var masterName = funcParamArray[0];
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 1)
                {
                    var annexTypeStr = funcParamArray[1];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var tempStr = "";
                if (_functionInitParamSet.SupportMultiCultures)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, CultureHelper.CurrentLanguageCode, GetAnnexMode.StepByStep);
                }
                else
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, "", GetAnnexMode.FirstAnnex);
                }
                returnText = tempStr.IsNullOrEmpty() ? masterName : tempStr;
            }
            else if (funcName.ToLower() == "GetPhraseAnnexText".ToLower())
            {
                var className = "Phrase";
                var masterName = funcParamArray[0];
                var annexType = AnnexTextType.DisplayName;
                if (funcParamArray.Length > 1)
                {
                    var annexTypeStr = funcParamArray[1];
                    annexType = AnnexHelper.GetTextType(annexTypeStr);
                }
                var tempStr = "";
                if (_functionInitParamSet.SupportMultiCultures)
                {
                    tempStr = AnnexHelper.GetText(className, masterName, _annexes, annexType, CultureHelper.CurrentLanguageCode, GetAnnexMode.OnlyByCurLang);
                    if (tempStr.IsNullOrEmpty()) tempStr = masterName;
                }
                else
                {
                    tempStr = masterName;
                }
                returnText = tempStr.IsNullOrEmpty() ? masterName : tempStr;
            }
            else if (funcName.ToLower() == "ValidateInput".ToLower())
            {
                var ReadLineName = funcParamArray[0];
                var uiItemValue = GetUiItemValue(ReadLineName + ".v");
                var uiItemValidateRules = _uiItems.Find(x => x.Name == ReadLineName).ValidationRules;
                if (uiItemValidateRules.IsNullOrEmpty())
                {
                    return "true";
                }
                else
                {
                    var ruleArry = uiItemValidateRules.GetSubParamArray(true, true);
                    foreach (var rule in ruleArry)
                    {
                        var funcParamArray1 = new string[] { uiItemValue, rule };
                        var validationResult = Getter.GetText("Validate", funcParamArray1);

                        if (validationResult == "OutOfScope") //false
                        {
                            validationResult = GetTextEx("Validate", funcParamArray1);
                        }

                        if (validationResult != "true")
                        {
                            return validationResult;
                        }

                        else
                        {
                            continue;
                        }
                    }
                    return "true";
                }
            }

            else
            {
                returnText = Getter.GetText(funcName, funcParamArray);
            }

            return returnText;


        }


        //**act
        private void Act(string actionName, string[] actionParamArray)
        {
            if (actionName.IsNullOrEmpty()) return;

            var returnStr = "";
            if (actionName.ToLower() == "Xrun".ToLower())
            {
                var actionParams = IdentifierHelper.UnwrapParamArray(actionParamArray);
                Xrun(actionParams);
            }
            else if (actionName.ToLower() == "Lrun".ToLower())
            {
                var actionParams = IdentifierHelper.UnwrapParamArray(actionParamArray);
                Lrun(actionParams);
            }
            else if (actionName.ToLower() == "Crun".ToLower())
            {
                var conVal = actionParamArray[0];
                if (conVal.ToLower() == "true")
                {
                    Xrun(actionParamArray[1]);
                }
                else
                {
                    if (actionParamArray.Length > 2)
                    {
                        Xrun(actionParamArray[2]);
                    }
                }
            }

            //--form
            else if (actionName.ToLower() == "ReturnFalse".ToLower())
            {
                BoolValue = false;
            }
            else if (actionName.ToLower() == "ReturnFalseAndExit".ToLower())
            {
                BoolValue = false;
                ExitApplication();
            }
            else if (actionName.ToLower() == "ExitApp".ToLower())
            {
                ExitApplication();
            }
            else if (actionName.ToLower() == "Sleep".ToLower())
            {
                var duration = 1000;
                if (actionParamArray.Length > 0) duration = Convert.ToInt16(actionParamArray[0]);
                System.Threading.Thread.Sleep(duration);

            }
            //--ui
            else if (actionName.ToLower() == "ClearConsole".ToLower())
            {
                Console.Clear();
            }
            else if (actionName.ToLower() == "NewLine".ToLower())
            {
                var value = actionParamArray[0];
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
            else if (actionName.ToLower() == "Beep".ToLower())
            {
                var value = actionParamArray[0];
                Beep(value);
            }
            else if (actionName.ToLower() == "WriteSpace".ToLower())
            {
                var value = actionParamArray[0];
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
            else if (actionName.ToLower() == "PressAnyKeyToExit".ToLower())
            {
                Console.Write(EasyWinAppRes.PressAnyKeyToExit);
                Console.ReadKey();
                ExitApplication();
            }
            else if (actionName.ToLower() == "Write".ToLower())
            {
                var colorStr = "";
                if (actionParamArray.Length > 1) colorStr = actionParamArray[1];
                if (!colorStr.IsNullOrEmpty()) SetConsoleForegroundColor(colorStr);
                Console.Write(actionParamArray[0]);
                if (!colorStr.IsNullOrEmpty()) Console.ForegroundColor = ConsoleColor.White;
            }
            else if (actionName.ToLower() == "WriteLine".ToLower())
            {
                var colorStr = "";
                if (actionParamArray.Length > 1) colorStr = actionParamArray[1];
                if (!colorStr.IsNullOrEmpty()) SetConsoleForegroundColor(colorStr);
                Console.WriteLine(actionParamArray[0]);
                if (!colorStr.IsNullOrEmpty()) Console.ForegroundColor = ConsoleColor.White;
            }

            //--process
            else if (actionName.ToLower() == "RefreshProcess".ToLower())
            {
                RefreshProcess();
            }
            else if (actionName.ToLower() == "RefreshProcessByGroup".ToLower())
            {
                RefreshProcessByGroup(Convert.ToInt32(actionParamArray[0].Trim()));
            }

            else if (actionName.ToLower() == "ClearProcessVariablesByGroup".ToLower())
            {
                ProcessHelper.ClearProcessVariablesByGroup(Convert.ToInt32(actionParamArray[0].Trim()), _procedures);
            }
            else if (actionName.ToLower() == "ClearProcedureVariables".ToLower())
            {
                var vars = actionParamArray[0];
                var varArr = vars.GetSubParamArray(true, true);
                foreach (var var in varArr)
                {
                    var varName = var.DeleteProcessIdentifer();
                    ProcessHelper.SetProcessVariableValue(varName, string.Empty, _procedures);
                }

            }
            else if (actionName.ToLower() == "RefreshProcessVariables".ToLower())
            {
                var vars = actionParamArray[0];
                var varArr = vars.GetSubParamArray(true, true);
                foreach (var var in varArr)
                {
                    var varName = var.DeleteProcessIdentifer();
                    RefreshProcessVariable(varName);

                }
            }
            else if (actionName.ToLower() == "SetProcessVariableValue".ToLower())
            {
                var var = actionParamArray[0].Trim();
                var varName = var.DeleteProcessIdentifer();
                ProcessHelper.SetProcessVariableValue(varName.Trim(), actionParamArray[1].Trim(), _procedures);
            }


            else//to do by Dispatcher
            {
                if (actionName.ToLower() == "run" | actionName.ToLower().StartsWith("runasadmin") |
                    actionName.ToLower() == "runcmd" | actionName.ToLower().StartsWith("runcmdasadmin"))
                {
                    var defLoc = _appLibDir;
                    var path = actionParamArray[0];
                    path = FileHelper.GetFilePath(path, defLoc);
                    actionParamArray[0] = path;
                }
                else if (actionName.ToLower() == "openfile" | actionName.ToLower().StartsWith("openfileasadmin") |
                    actionName.ToLower() == "editfile" | actionName.ToLower().StartsWith("editfileasadmin") |
                    actionName.ToLower() == "runPy" | actionName.ToLower().StartsWith("runpyasadmin")
                    )
                {
                    var defLoc = _appDataDir;
                    var path = actionParamArray[0];
                    path = FileHelper.GetFilePath(path, defLoc);
                    actionParamArray[0] = path;
                }
                returnStr = Dispatcher.Act(actionName, actionParamArray);

                if (returnStr == "OutOfScope")
                {
                    returnStr = ActEx(actionName, actionParamArray);
                }
            }

            if (actionName.ToLower() == "ExecCmd".ToLower() | actionName.ToLower() == "ExecCmdAsAdmin".ToLower() |
    actionName.ToLower() == "ExecPython".ToLower() | actionName.ToLower() == "ExecPythonAsAdmin".ToLower())
            {
                var lastParam = actionParamArray[actionParamArray.Length - 1];
                var execCmdWindowOptionStr = "";
                if (actionName.ToLower() == "ExecCmd".ToLower() | actionName.ToLower() == "ExecCmdAsAdmin".ToLower())
                {
                    if (actionParamArray.Length > 1) execCmdWindowOptionStr = actionParamArray[1];
                }
                else
                {
                    if (actionParamArray.Length > 2) execCmdWindowOptionStr = actionParamArray[2];
                }
                var execCmdWindowOption = ExecCmdWindowOption.ShowWindow;
                execCmdWindowOption = EnumHelper.GetByName<ExecCmdWindowOption>(execCmdWindowOptionStr, execCmdWindowOption);
                if (execCmdWindowOption == ExecCmdWindowOption.ShowWindow) return;


                if (lastParam.ToLower() == "save")
                {
                    var title = actionName;
                    foreach (var actParam in actionParamArray)
                    {
                        title = title + "-" + actParam.ToLegalFileName();
                    }

                    title = title + "-Executing-Result";
                    var content = GetHelper.FormatRichText(returnStr);
                    var dir = _appTempDir + "\\" + "Cmds";
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    var path = dir + "\\" + title.ToUniqueStringByNow("") + ".txt";
                    File.WriteAllText(path, content);
                }
            }
        }

        //**xrun
        //##xrun
        private void Xrun(string actParams)
        {
            var exInfo = "\n>> " + GetType().FullName + ".Xrun Error: ";
            var elementNameArr = actParams.GetSubParamArray(true, true);
            foreach (var elementName in elementNameArr)
            {

                var action = "";
                if (IdentifierHelper.IsProcessElement(elementName))
                {
                    var elementName1 = elementName.DeleteProcessIdentifer();
                    var item = _procedures.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "ProcessElementName={0} des not exist!", elementName));
                    if (item.Formula.IsNullOrEmpty()) continue;
                    action = item.Formula;
                }
                else if (IdentifierHelper.IsUiElement(elementName))
                {
                    var elementName1 = IdentifierHelper.DeleteUiIdentifer(elementName);
                    var item = _uiItems.Find(x => x.Name == elementName1);
                    if (item == null) throw new ArgumentException(string.Format(exInfo + "UiElementName={0} des not exist!", elementName));
                    if (item.Value.IsNullOrEmpty()) continue;
                    action = item.Value;
                }

                var actionNameAndParamsArray = action.GetParamArray(true, false);
                var actionName = actionNameAndParamsArray[0].Trim().ToLower();
                var actionParamArray = new string[actionNameAndParamsArray.Length - 1];
                for (int i = 0; i < actionNameAndParamsArray.Length - 1; i++)
                {
                    actionParamArray[i] = GetText(ResolveStringByRefProcessVariablesAndUiItems(actionNameAndParamsArray[i + 1].Trim()));
                }
                try
                {
                    Act(actionName, actionParamArray);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("\n>> " + GetType().FullName + ".Xrun Error: elementName=" + elementName + "; " + " action = " + action + "; " + ex.Message);
                }

            }
        }

        private void Lrun(string actParams)
        {
            if (!IdentifierHelper.ContainsLrunIdentifer(actParams)) return;
            var strArray = actParams.GetParamArray(true, false);
            var actionName = strArray[0];
            var actionParamsAndReplaceVals = strArray[1];
            var actionParamsAndReplaceValsArry = actionParamsAndReplaceVals.GetParamArray(true, false);
            var actionParams = actionParamsAndReplaceValsArry[0];
            var actionParamsArry = actionParams.GetParamArray(true, false);
            var ReplaceVals = actionParamsAndReplaceValsArry[1];
            var ReplaceValsArry = ReplaceVals.GetSubParamArray(true, false);

            foreach (var lrunParam in ReplaceValsArry)
            {
                foreach (var actionParam in actionParamsArry)
                {
                    if (IdentifierHelper.ContainsLrunIdentifer(actionParam))
                        actionParam.Replace(IdentifierHelper.LrunIdentifer, lrunParam);
                }

                Act(actionName, actionParamsArry);
            }

        }


        //**process
        //##RefreshProcess
        private void RefreshProcess()
        {
            RefreshProcessByGroup(0);

        }

        //##RefreshProcessByGroup
        private void RefreshProcessByGroup(int grpId)
        {
            var procList = new List<Procedure>();
            procList = _procedures;
            if (procList.Count == 0)
            {
                return;
            }

            var procListByGrp = new List<Procedure>();

            procListByGrp = procList.FindAll(x => x.GroupId == grpId);
            if (procListByGrp.Count != 0)
            {
                RefreshProcedures(procListByGrp, procList);
            }
        }

        //##RefreshProcedureVariable
        private void RefreshProcessVariable(string varName)
        {
            var var = _procedures.Find(x => x.Name == varName & x.Type == (int)ProcedureType.Variable);
            if (var == null)
            {
                return;
            }

            var conTxt = ProcessHelper.ResolveStringByRefProcessVariables(var.Condition, _procedures);
            var con = GetText(conTxt);
            if (string.IsNullOrEmpty(con) || con.ToLower() == "true")
            {
                if (var.Formula.IsNullOrEmpty()) return;
                var formularTxt = ProcessHelper.ResolveStringByRefProcessVariables(var.Formula, _procedures);
                var.Value = GetText(formularTxt);
            }
        }
        private void RefreshProcedures(List<Procedure> procListByGrp, List<Procedure> procListAll)
        {
            foreach (var proc in procListByGrp)
            {
                var conTxt = ProcessHelper.ResolveStringByRefProcessVariables(proc.Condition, _procedures);
                var con = conTxt.IsNullOrEmpty() ? string.Empty : GetText(conTxt);
                bool toDo = string.IsNullOrEmpty(con) || con.ToLower() == "true";

                if (toDo)
                {
                    if (proc.Type == (int)ProcedureType.Variable)
                    {
                        if (!string.IsNullOrEmpty(proc.Formula))
                        {
                            var formularTxt = ProcessHelper.ResolveStringByRefProcessVariables(proc.Formula, _procedures);
                            if (formularTxt.StartsWith("="))
                                proc.Value = GetText(formularTxt);
                        }
                    }
                    else if (proc.Type == (int)ProcedureType.Transaction)
                    {
                        if (!string.IsNullOrEmpty(proc.Formula))
                        {
                            ActByUiElementAndProcedure("#" + proc.Name + "#");
                        }
                    }
                    else if (proc.Type == (int)ProcedureType.Break) break;
                    else if (proc.Type == (int)ProcedureType.Exit) return;
                }
            }
        }



        //**resolve
        //###ResolveConstants
        private string ResolveConstants(string text)
        {
            if (text.IsNullOrEmpty()) return string.Empty;
            if (!text.Contains("%")) return text;

            var toBeRplStr = "";
            toBeRplStr = "%ArchCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ArchitectureCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ArchitectureName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ArchVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ArchitectureVersion;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%OrgCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.OrganizationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgShortName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.OrganizationShortName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%OrgName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.OrganizationName;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%AppCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ApplicationCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppVersion%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ApplicationVersion;
                return Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ScenarioCode%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _functionInitParamSet.ScenarioCode;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            //--golbal config
            toBeRplStr = "%CfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _cfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            //appCfg
            toBeRplStr = "%AppCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appCfgDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppUiDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appCfgUiDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%ScenariosDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _scenariosDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%ScenarioDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _scenarioDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%FormCfgDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _scenarioDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }



            toBeRplStr = "%AppLibDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appLibDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            toBeRplStr = "%AppTempDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _appTempDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }


            toBeRplStr = "%sysAppDataDir%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = _sysAppDataDir;
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }



            if (!text.Contains("%"))
            {
                return text;
            }
            else
            {
                var retStr = Ligg.EasyWinApp.Parser.Helpers.GetHelper.ResolveConstants(text);
                if (!retStr.Contains("%"))
                {
                    return retStr;
                }
                else
                {
                    return ResolveConstantsEx(text);
                }
            }
        }

        //##ResolveStringByRefProcedureVariablesAndControls
        private string ResolveStringByRefProcessVariablesAndUiItems(string str)
        {
            try
            {
                if (str.IsNullOrEmpty()) return "";
                if (IdentifierHelper.ContainsProcessIdentifer(str))
                {
                    str = ProcessHelper.ResolveStringByRefProcessVariables(str, _procedures.Where(x => x.Type == (int)ProcedureType.Variable | x.Type == (int)ProcedureType.Params).ToList());
                }

                if (IdentifierHelper.ContainsUiIdentifer(str))
                {
                    str = ResolveStringByRefUiItems(str);
                }
                return str;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefProcessVariablesAndUiItems Error: str=" + str + "; " + ex.Message);
            }
        }

        //##ResolveStringByRefControls
        private string ResolveStringByRefUiItems(string str)
        {
            try
            {
                var strArray = str.Split(IdentifierHelper.UiIdentifer.ToChar());
                int n = strArray.Count();
                if (n % 2 == 0)
                {
                    throw new ArgumentException(IdentifierHelper.UiIdentifer + "  no. in " + str + " is not an even! ");
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (i % 2 == 1)
                        {
                            if (strArray[i].ToLower().EndsWith(".v"))
                            {
                                var txt = strArray[i];
                                strArray[i] = GetUiItemValue(txt);
                            }
                            else
                            {
                                strArray[i] = IdentifierHelper.UiIdentifer + strArray[i] + IdentifierHelper.UiIdentifer;
                            }

                        }
                    }
                    return string.Join("", strArray);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveStringByRefUiItems Error: str='" + str + "'; " + ex.Message);
            }
        }

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
                var paramArry = value.GetSubParamArray(true, true);
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


        //*ex
        private string ResolveConstantsEx(string str)
        {
            if (!_functionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveConstantsEx Error: OutOfScope, str=" + str);
            var retStr = _adapter.ResolveConstantsEx(str);
            return retStr;
        }

        private string GetTextEx(string funcName, string[] funcParamArray)
        {
            if (!_functionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".GetTextEx Error: OutOfScope, funcName=" + funcName);
            var retStr = _adapter.GetTextEx(funcName, funcParamArray);
            return retStr;
        }

        private string ActEx(string actName, string[] actionParamArray)
        {
            if (!_functionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".ActEx Error: OutOfScope, actName=" + actName);
            var retStr = _adapter.ActEx(actName, actionParamArray);
            return retStr;
        }

        private string GetCenterData(string dataName)
        {
            if (dataName.ToLower() == "UserCode".ToLower()) return CentralData.UserCode;
            else if (dataName.ToLower() == "UserToken".ToLower()) return CentralData.UserToken;
            else if (dataName.ToLower() == "OrgCode".ToLower() | dataName.ToLower() == "OrganizationCode".ToLower()) return CentralData.OrganizationCode;
            else if (dataName.ToLower() == "OrgSname".ToLower() | dataName.ToLower() == "OrganizationShortName".ToLower()) return CentralData.OrganizationShortName;
            else if (dataName.ToLower() == "OrgName".ToLower() | dataName.ToLower() == "OrganizationName".ToLower()) return CentralData.OrganizationName;
            else throw new ArgumentException("\n>> " + GetType().FullName + ".GetCenterData Error: dataName doesn't exist dataName=" + dataName);
        }
        private string GetCommonParams(string id)
        {
            if (CentralData.CommonParams.IsNullOrEmpty()) throw new ArgumentException("\n>> " + GetType().FullName + ".GetCommonParams Error: CommonParams does not exist!");
            if (id.IsNullOrEmpty()) return CentralData.CommonParams;
            if (!id.IsPlusIntegerOrZero()) throw new ArgumentException("\n>> " + GetType().FullName + ".GetCommonParams Error: id should be plus integer or zero id=" + id);
            var CommonParamsArr = CentralData.CommonParams.GetSubParamArray(false, false);
            var idInt = Convert.ToInt32(id);
            if (idInt > CentralData.CommonParams.Length - 1) throw new ArgumentException("\n>> " + GetType().FullName + ".GetCommonParams Error: CommonParams[{id}] does not exist!");

            return CommonParamsArr[idInt];
        }




    }
}
//*end
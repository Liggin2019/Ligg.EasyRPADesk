using Ligg.Infrastructure.DataModels;
using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.WinForm.DataModels;
using Ligg.RpaDesk.WinForm.Forms;
using Ligg.RpaDesk.WinForm.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Ligg.RpaDesk
{
    public partial class StartForm : FunctionForm
    {
        public StartForm(FormInitParamSet formInitParamSet)
        {
            try
            {
                FormInitParamSet = formInitParamSet;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".StartForm Error: " + ex.Message);
            }
        }

        protected override string ResolveConstantsEx(string text)
        {
            var toBeRplStr = "%CurrentUserName%".ToLower();
            if (text.ToLower().Contains(toBeRplStr))
            {
                var rplStr = CentralData.UserName ?? "User";
                text = Regex.Replace(text, toBeRplStr, rplStr, RegexOptions.IgnoreCase);
            }

            if (!FormInitParamSet.HasCblpComponent)
            {
                return text;
            }
            if (text.Contains("%"))
            {
                text = new AdapterHandler().ResolveConstants(text);
            }

            return text;
        }

        protected override string ValidateEx(string code, string rule)
        {
            var exInfo = GetType().FullName + ".ValidateEx Error: " + "Validation Rule= " + rule;
            if (!FormInitParamSet.HasCblpComponent)
            {
                throw new ArgumentException(exInfo + " undefined");
            }

            var rst = new AdapterHandler().Validate(code, rule);
            return GenericHelper.ConvertToJson(rst);
        }

        //*get
        //#GetEx
        protected override string GetEx(string funcName, string[] funcParamArray)
        {
            var exInfo = "\n>> " + GetType().FullName + ".GetEx." + funcName + " Error: ";
            if (funcName == "GetNothing") { return string.Empty; }

            else if (funcName.StartsWith("Cblpc-"))
            {
                var arr = funcName.Split('-').Trim();//Cblpc-SingleFileDealer-Do-Update
                if (arr.Length < 3)
                {
                    //MessageHelper.PopupError(exInfo + "funcParamArray.Length can't be less than 3!");
                    throw new ArgumentException(exInfo + "CblpComponent funcName= " + funcName + " array Length can't less than 3");
                }

                return new AdapterHandler().Dispatch(arr[0], arr[1], arr[2], arr[3], funcParamArray);

            }
            else if (funcName.StartsWith("HttpClientLogon"))
            {
                if (funcParamArray.Length < 3)
                    throw new ArgumentException(exInfo + "funcParamArray Length can't less than 3");

                var url = funcParamArray[0];
                var acct = funcParamArray[1];
                var pw = funcParamArray[2];
                var rst = HttpClientHelper.Logon(url, acct, pw);
                return rst;
            }
            else if (funcName.StartsWith("HttpClientGet"))
            {
                if (funcParamArray.Length < 1)
                    throw new ArgumentException(exInfo + "funcParamArray Length can't less than 1");
                var url = funcParamArray[0];
                string[] subFuncParamArray = null;
                if (funcParamArray.Length > 1)
                    subFuncParamArray = funcParamArray.Extract(1, funcParamArray.Length - 1);
                var rst = HttpClientHelper.Get(url, subFuncParamArray);
                return rst;
            }
            else if (funcName.StartsWith("HttpClientPost"))
            {
                if (funcParamArray.Length < 2)
                    throw new ArgumentException(exInfo + "funcParamArray Length can't less than 2");
                var url = funcParamArray[0];
                string[] subFuncParamArray = null;
                var data = funcParamArray[1];
                if (funcParamArray.Length > 2)
                    subFuncParamArray = funcParamArray.Extract(2, funcParamArray.Length - 1);
                var rst = HttpClientHelper.Post(url, data, subFuncParamArray);
                return rst;
            }
            else if (funcName == "RunSenario" | funcName == "RunSenario32")
            {
                if (funcParamArray.Length < 1)
                {
                    throw new ArgumentException(exInfo + "funcParamArray.Length can't be less than 1!");
                }
                var execFuncName = "ExecCmd";
                var execFuncArr = RunSenario(funcName, funcParamArray);
                return Get(execFuncName, execFuncArr);

            }
            else if (funcName == "GetCurrentLanguageCode")
            {
                return LanguageHelper.CurrentLanguageCode; 
            }
            else
            {
                throw new NotImplementedException(exInfo + "NotImplementedException");
            }

        }

        //*do
        //#DoEx
        protected override void DoEx(string funcName, string[] funcParamArray)
        {
            var exInfo = "\n>> " + GetType().FullName + ".DoEx." + funcName + " Error: ";
            if (funcName == "DoNothing") { return; }
            else if (funcName.StartsWith("Cblpc-"))
            {
                if (funcParamArray.Length < 3)
                {
                    MessageHelper.PopupError(exInfo + "funcParamArray.Length can't be less than 3!");
                    return;
                }
                var arr = funcName.Split('-').Trim();//Cblpc-SingleFileDealer-Get-FileSize
                new AdapterHandler().Dispatch(arr[1], arr[2], arr[3], funcName, funcParamArray);

            }
            else if (funcName == "SetCurrentUser")
            {
                if (funcParamArray.Length < 1)
                {
                    MessageHelper.PopupError(exInfo + "funcParamArray.Length can't be less than 1!");
                    return;
                }
                var userInfo = funcParamArray[0].ConvertToGeneric<UserInfo>(true, TxtDataType.Json);
                CentralData.UserId = userInfo.Id;
                CentralData.UserAccount = userInfo.Account;
                CentralData.UserName = userInfo.Name;
            }

            else if (funcName.StartsWith("HttpClientPost"))
            {
                GetEx(funcName, funcParamArray);
            }
            else if (funcName == "RunSenario" | funcName == "RunSenario32" )
            {
                if (funcParamArray.Length < 1)
                {
                    MessageHelper.PopupError(exInfo + "funcParamArray.Length can't be less than 1!");
                    return;
                }
                var execFuncName = "ExecCmd";
                var execFuncArr = RunSenario(funcName, funcParamArray);
                Do(execFuncName, execFuncArr);

            }
            else if (funcName == "RefreshUi")
            {
                RefreshUi();
            }
            else if (funcName == "UpdateCurrentLanguageCode") 
            {
                if (funcParamArray.Length < 1)
                {
                    MessageHelper.PopupError(exInfo + "funcParamArray.Length can't be less than 1!");
                    return;
                }
                CentralData.CurrentLanguageCode = funcParamArray[0];
            }
            else if (funcName == "ShowDialog")
            {
                if (funcParamArray.Length < 2)
                {
                    MessageHelper.PopupError(exInfo + "funcParamArray.Length can't be less than 2!");
                    return;
                }

                string startFormRelativeLocation = "";
                string startFormTitle = "";
                string startInitParams = "";
                string startTransactions = "";

                var formTypeFlag = funcParamArray[0];
                startFormRelativeLocation = funcParamArray[1];
                if (DirectoryHelper.IsLegalAbsoluteDirectory(startFormRelativeLocation)) throw new ArgumentException("formLocation=" + startFormRelativeLocation + " can't be an absolute path! ");
                if (funcParamArray.Length > 2) startFormTitle = funcParamArray[2];
                if (funcParamArray.Length > 3) startInitParams = funcParamArray[3];
                if (funcParamArray.Length > 4) startTransactions = funcParamArray[4];

                var functionInitParamSet = new FormInitParamSet();
                functionInitParamSet.FormType = formTypeFlag == "Svi" ? FormType.Svi : FormType.Szi;
                functionInitParamSet.ArchitectureCode = FormInitParamSet.ArchitectureCode;
                functionInitParamSet.ArchitectureName = FormInitParamSet.ArchitectureName;
                functionInitParamSet.ArchitectureVersion = FormInitParamSet.ArchitectureVersion;
                functionInitParamSet.OrganizationCode = FormInitParamSet.OrganizationCode;
                functionInitParamSet.OrganizationShortName = FormInitParamSet.OrganizationShortName;
                functionInitParamSet.OrganizationName = FormInitParamSet.OrganizationName;

                functionInitParamSet.ApplicationVersion = FormInitParamSet.ApplicationVersion;
                functionInitParamSet.HelpdeskEmail = FormInitParamSet.HelpdeskEmail;
                functionInitParamSet.SupportMultiLanguages = FormInitParamSet.SupportMultiLanguages;
                functionInitParamSet.ApplicationCode = FormInitParamSet.ApplicationCode;

                functionInitParamSet.FormRelativeLocation = startFormRelativeLocation;
                functionInitParamSet.FormTitle = startFormTitle;


                functionInitParamSet.ApplicationDataDir = FormInitParamSet.ApplicationDataDir;
                functionInitParamSet.ApplicationLibDir = FormInitParamSet.ApplicationLibDir;
                functionInitParamSet.ApplicationOutputDir = FormInitParamSet.ApplicationOutputDir;

                ShowDialog(functionInitParamSet);
            }
            else if (funcName == "RunSenario" | funcName == "RunSenario32")
            {
                if (funcParamArray.Length < 1)
                {
                    throw new ArgumentException(exInfo + "funcParamArray.Length can't be less than 1!");
                }
                var execFuncName = "ExecCmd";
                var execFuncArr = RunSenario(funcName, funcParamArray);
                Get(execFuncName, execFuncArr);
            }
            else
            {
                throw new NotImplementedException(exInfo + "NotImplementedException");
            }

        }

        //*do
        private string[] RunSenario(string funcName, string[] funcParamArray)
        {
            var startArgs = new StartArgs();
            var appArgsArry = funcParamArray[0].GetLarrayArray(true, false);
            startArgs.AppCode = appArgsArry[0];
            
            startArgs.IsRedirectedFlag = "true";
            var appArgArray = new String[] { startArgs.AppCode, startArgs.IsRedirectedFlag };
            //*test 
            //new Process().Start()会吞掉 ;^； 认为;^是空格；只能用@
            //var appArgsStr = appArgArray.UnwrapFormularParamArray();
            var appArgsStr = appArgArray.Unwrap("@");
            var formArgsArray = funcParamArray[1].GetLarrayArray(true, false);
            startArgs.FormRelativeLocation = formArgsArray[0];
            startArgs.FormTitle = formArgsArray.Length > 1 ? formArgsArray[1] : "";
            var formArgArray = new String[] { startArgs.FormRelativeLocation, startArgs.FormTitle};
            var formArgsStr = formArgArray.Unwrap("@");

            startArgs.StartLanguage = LanguageHelper.CurrentId;
            var arguments = appArgsStr + " " + formArgsStr + " " + startArgs.StartLanguage;
            var path = Application.ExecutablePath;
            var dir = FileHelper.GetFileDetailByOption(path, FilePathComposition.Directory);
            path = dir + "\\ConsoleApp" + ".exe";
            if (funcName.Contains("32"))
                path = dir + "\\ConsoleApp32" + ".exe";
            
            var execCmdWindowOptionStr = funcParamArray.Length > 2 ? funcParamArray[2] : "ShowWindow";
            var interceptOutputStr = funcParamArray.Length > 3 ? funcParamArray[3].JudgeJudgementFlag() : false;
            var raiseUacLevel = funcParamArray.Length > 4 ? funcParamArray[4].JudgeJudgementFlag() : false;
            var runAsAdmin = funcParamArray.Length > 5 ? funcParamArray[5].JudgeJudgementFlag() : false;
            var execFuncArr = new string[] { path, arguments, execCmdWindowOptionStr, interceptOutputStr.ToString(), raiseUacLevel.ToString(), runAsAdmin.ToString() };
            return execFuncArr;
        }

        private void RefreshUi()
        {
            try
            {

                _procedures.Clear();
                _layoutElements.Clear();
                _annexes.Clear();
                _zonesItems.Clear();
                _renderedViewNames.Clear();
                _viewFeatures.Clear();
                _menuFeatures.Clear();
                _currentNestedMenuId = 0;

                //keep _currentViewName to pass start view
                //_currentViewName="";
                TopNavSectionLeftRegion.Controls.Clear();
                TopNavSectionCenterRegion.Controls.Clear();
                TopNavSectionRightRegion.Controls.Clear();

                ToolBarSectionLeftRegion.Controls.Clear();
                ToolBarSectionCenterRegion.Controls.Clear();
                ToolBarSectionRightRegion.Controls.Clear();
                ToolBarSectionPublicRegionToolStrip.Items.Clear();
                //ToolBarSectionPublicRegionToolStrip.Controls.Clear(); //*testerror:集合为只读。

                MiddleNavSectionLeftRegion.Controls.Clear();
                MiddleNavSectionCenterRegion.Controls.Clear();
                MiddleNavSectionRightRegion.Controls.Clear();

                DownNavSectionLeftRegion.Controls.Clear();
                DownNavSectionCenterRegion.Controls.Clear();
                DownNavSectionRightRegion.Controls.Clear();

                MainSectionLeftNavDivisionUpRegion.Controls.Clear();
                MainSectionLeftNavDivisionMiddleRegion.Controls.Clear();
                MainSectionLeftNavDivisionDownRegion.Controls.Clear();

                MainSectionLeftNavDivision1UpRegion.Controls.Clear();
                MainSectionLeftNavDivision1MiddleRegion.Controls.Clear();
                MainSectionLeftNavDivision1DownRegion.Controls.Clear();

                MainSectionMainDivisionUpRegion.Controls.Clear();
                MainSectionMainDivisionMiddleRegion.Controls.Clear();
                MainSectionMainDivisionDownRegion.Controls.Clear();

                MainSectionRightDivisionUpRegion.Controls.Clear();
                MainSectionRightDivisionMiddleRegion.Controls.Clear();
                MainSectionRightDivisionDownRegion.Controls.Clear();

                LoadForm();
                this.Refresh();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".RefreshUi Error: " + ex.Message);
            }
        }

        private void ShowDialog(FormInitParamSet functionInitParamSet)
        {
            try
            {
                var form = new StartForm(functionInitParamSet);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ShowDialog Error: " + ex.Message);
            }
        }

    }

}

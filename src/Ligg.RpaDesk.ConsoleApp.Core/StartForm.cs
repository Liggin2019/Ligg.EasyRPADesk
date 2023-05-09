using Ligg.Infrastructure.Extensions;
using Ligg.Infrastructure.Helpers;
using  Ligg.Infrastructure.Utilities.DataParserUtil;
using Ligg.RpaDesk.Helpers;
using Ligg.RpaDesk.Interface;
using Ligg.RpaDesk.Parser.DataModels;
using Ligg.RpaDesk.WinCnsl;
using Ligg.RpaDesk.WinCnsl.DataModels;
using System;
using System.Text.RegularExpressions;
using Ligg.Infrastructure.DataModels;

namespace Ligg.RpaDesk
{
    public class StartForm : ScenarioForm
    {
        private IStdServiceComponentAdapter _adapter = null;

        public StartForm(FormInitParamSet formInitParamSet)
        {
            try
            {
                FormInitParamSet = formInitParamSet;
                _adapter = Bootstrapper.Adapter;
                Load();
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
                throw new ArgumentException(exInfo + "CblpComponent does not exist");
            }

            var rst = new AdapterHandler().Validate(code, rule);
            return GenericHelper.ConvertToJson(rst);
        }

        //*get
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

            throw new NotImplementedException();
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
                    Console.WriteLine(exInfo + "funcParamArray.Length can't be less than 3!");
                    return;
                }
                var arr = funcName.Split('-').Trim();//Cblpc-SingleFileDealer-Get-FileSize
                new AdapterHandler().Dispatch(arr[1], arr[2], arr[3], funcName, funcParamArray);

            }
            else if (funcName == "SetCurrentUser")
            {
                if (funcParamArray.Length < 1)
                {
                    Console.WriteLine(exInfo + "funcParamArray.Length can't be less than 1!");
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
            throw new NotImplementedException();

        }

        private string GetInitParams(string ids)
        {
            if (CentralData.InitParams.IsNullOrEmpty()) //throw new ArgumentException("\n>> " + GetType().FullName + ".GetInitParam Error: GetInitParam does not exist!");
                return string.Empty;
            var initParams = ResolveConstants(CentralData.InitParams);
            if (ids.IsNullOrEmpty()) return initParams;
            var initParamsArr = initParams.GetLarrayArray(false, false);
            if (ids.IsPlusIntegerOrZero())
            {
                var idInt = Convert.ToInt32(ids);
                if (idInt > initParamsArr.Length - 1) throw new ArgumentException("\n>> " + GetType().FullName + ".GetInitParam Error: InitParams[{0}] does not exist!".FormatWith(idInt));
                return initParamsArr[idInt];
            }
            else
            {
                var idsArry = ids.GetLarrayArray(true, true);
                foreach (var id in idsArry)
                {
                    if (!id.IsPlusIntegerOrZero()) throw new ArgumentException("\n>> " + GetType().FullName + ".GetInitParams Error: member of ids should be PlusIntegerOrZero,  ids= " + ids);
                    var idInt = Convert.ToInt32(id);
                    if (idInt > initParamsArr.Length - 1) throw new ArgumentException("\n>> " + GetType().FullName + ".GetInitParams Error: InitParams[{0}] does not exist!".FormatWith(id));
                }

                return CentralData.InitParams.ExtractLarray(ids);
            }
        }



    }
}

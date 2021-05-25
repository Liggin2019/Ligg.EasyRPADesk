using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ligg.Infrastructure.Base.Helpers;
using Ligg.Infrastructure.Base.Extension;
using Ligg.EasyWinApp.Parser.Helpers;
using Ligg.EasyWinApp.Interface;
using Ligg.EasyWinApp.WinForm.DataModel;
using Ligg.EasyWinApp.WinForm.Forms;

namespace Ligg.EasyWinApp
{
    public partial class StartForm : FunctionForm
    {
        private ICblpAdapter _adapter = null;

        public StartForm(FormInitParamSet functionInitParamSet)
        {
            try
            {
                FunctionInitParamSet = functionInitParamSet;
                if (functionInitParamSet.IsFormInvisible) this.Hide();
                _adapter = BootStrapper.Adapter;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".StartForm Error: " + ex.Message);
            }
        }

        //#override
        protected override string ResolveConstantsEx(string text)
        {
            try
            {
                if (!FunctionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveConstantsEx Error: OutOfScope, str=" + text);
                var retStr = _adapter.ResolveConstantsEx(text);
                return retStr;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ResolveConstantsEx Error: " + ex.Message);
            }
        }

        protected override string GetTextEx(string funcName, string[] funcParamArray)
        {
            if (!FunctionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".GetTextEx Error: OutOfScope, funcName=" + funcName);
            try
            {
                var retStr = _adapter.GetTextEx(funcName, funcParamArray);
                return retStr;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".GetTextEx Error: " + ex.Message);
            }
        }


        protected override string ActEx(string actName, string[] actionParamArray)
        {
            if (!FunctionInitParamSet.HasCblpComponent) throw new ArgumentException("\n>> " + GetType().FullName + ".ActEx Error: OutOfScope, actName=" + actName);
            try
            {
                var retStr = _adapter.ActEx(actName, actionParamArray);
                return retStr;

            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ActEx Error: actName=" + actName + "; " + ex.Message);
            }
        }

        protected override void PopupSviDialogEx(FormInitParamSet functionInitParamSet)
        {
            try
            {
                var form = new StartForm(functionInitParamSet);
                form.ShowInTaskbar = false;
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".PopupZoneDialogEx Error: " + ex.Message);
            }
        }

        protected override string GetCenterData(string dataName)
        {
            if (dataName.ToLower() == "UserCode".ToLower()) return CentralData.UserCode;
            else if (dataName.ToLower() == "UserToken".ToLower()) return CentralData.UserToken;
            else if (dataName.ToLower() == "OrgCode".ToLower() | dataName.ToLower() == "OrganizationCode".ToLower()) return CentralData.OrganizationCode;
            else if (dataName.ToLower() == "OrgSname".ToLower() | dataName.ToLower() == "OrganizationShortName".ToLower()) return CentralData.OrganizationShortName;
            else if (dataName.ToLower() == "OrgName".ToLower() | dataName.ToLower() == "OrganizationName".ToLower()) return CentralData.OrganizationName;
            else throw new ArgumentException("\n>> " + GetType().FullName + ".GetCenterData Error: dataName doesn't exist dataName=" + dataName);
        }

        protected override string GetCommonParams(string id)
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

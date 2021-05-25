namespace Ligg.EasyWinApp.WinForm.DataModel
{
    public class SubMenuItem
    {
        public string Id;
        //public int Type;//such as select one/many/none
        //public string TypeName;//such as select one/many/none
        public string ControlTypeName;//Seperator
        public string ParentId;
        public string Name;
        public string DisplayName;
        public string Description;
        public string Action;
        public string ImageUrl;

        public bool Invisible;
        public bool Disabled;

        public string ExecModeFlag;
        public string ShowRunningStatusFlag;
        public string WriteIntoLogFlag;
    }



}

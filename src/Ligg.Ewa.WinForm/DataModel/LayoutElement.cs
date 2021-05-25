
namespace Ligg.EasyWinApp.WinForm.DataModel
{
    public class LayoutElement
    {
        public int Id;
        public int LayoutType;
        public int LayoutId;
        public string Name;
        public int ParentId;
        public string Container;
        public int Type;
        public string TypeName;
        public string ControlTypeName;
        public string DisplayName;
        public string Remark;

        public string Location;
        public string InputVariables;
        public string DataSource;


        public string View;
        public string DefaultSubItemIdFlag;
        public string Action;
        public string ReplacedIdentifiers;
        public string Trigger;  //only for FollowingTransactionItem in View
        public bool IsRendered;
        public bool IsChecked;
        public string InvalidFlag;
        public string DisabledFlag;
        public string InvisibleFlag;

        public string StyleClass;
        public string StyleText;
        public bool IsPopup;
        public int ZoneCpntArrangementType;
        public int DockType;
        public string DockTypeName;
        public string DockOrder;
        public int OffsetOrPositionX;
        public int OffsetOrPositionY;
        public int Width;
        public int Height;
        public string ImageUrl;
        public int ImageWidth;
        public int ImageHeight;
        public bool IsLastLevel;

        public string ExecModeFlag;
        public string WriteIntoLogFlag;
        public string ShowRunningStatusFlag;
    }

}

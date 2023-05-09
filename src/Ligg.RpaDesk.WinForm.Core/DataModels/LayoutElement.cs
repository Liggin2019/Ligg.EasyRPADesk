
namespace Ligg.RpaDesk.WinForm.DataModels
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

        public string Location;
        public string ShellArgs;
        public string DataSource;


        public string View;
        public string DefaultSubItemId; //Only for nested MenuItem
        public string Action;
        public string ReplacedIdentifiers;
        public string Trigger;  //only for watchers in View
        public bool IsRendered;
        public bool IsChecked;
        public string Invalid;
        public string Disabled;
        public string Invisible;

        public string StyleClass;  //for menu/submenu no use
        public string StyleText;

        public int ZoneCpntArrangementType;
        public int DockType;
        public string DockTypeName;
        public string DockOrder;
        public int OffsetOrPositionX;
        public int OffsetOrPositionY;
        public int Width;
        public int Height;

        public string ImageUrl;
        public bool IsLastLevel;
        public string ShowRunningStatus;


    }

}

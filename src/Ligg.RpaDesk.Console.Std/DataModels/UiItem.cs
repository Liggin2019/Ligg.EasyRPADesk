
namespace Ligg.RpaDesk.WinCnsl.DataModels
{
    public class UiItem
    {
        public int Id;
        public int Type;
        public string TypeName;
        public string Name;
        public string DisplayName;
        public string Value;
        public string ValidationRules;

        public string ExecMode;
        public string ShowRunningStatus;
        public string WriteIntoLog;
        public string Condition;// for goto

        public string Invalid;

    }

}

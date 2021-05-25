
namespace Ligg.EasyWinApp.Parser.DataModel
{
    public class Procedure
    {
        //public long Id;
        public int Type;
        public string TypeName;
        public string Name;
        public string DisplayName;

        public int GroupId;
        public string ZoneName;
        public string Condition;
        public string Formula;
        public string Value;
        public string ReplacedTabooIdentifiers;

        public string ExecOnInitFlag;

        public string ExecModeFlag;
        public string ShowRunningStatusFlag;
        public string WriteIntoLogFlag;

        public string InvalidFlag;
    }

}

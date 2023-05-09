
namespace Ligg.RpaDesk.Parser.DataModels
{
    public class Procedure
    {
        public string ShellId;
        public int GroupId;

        public int Type;
        public string TypeName;
        public string Name;
        public string DisplayName;

        public string Value;
        public string StartValue;

        public string Expression;
        public string ShowRunningStatus;
        public string SkipOnInit;//bool
        public int Status;
        public string Invalid;
    }

}

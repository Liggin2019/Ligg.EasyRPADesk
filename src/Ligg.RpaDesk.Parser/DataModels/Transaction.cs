namespace Ligg.RpaDesk.Parser.DataModels
{
    public class Transaction
    {
        public string UiItemName;
        public string Action;
        public string DisplayName;

        public string ShowRunningStatus;
    }

    public class TransactionDetail
    {
        public string Name;
        public string[] Params;
        public string DisplayName;
        public bool ShowRunningStatus;
        public ExecMode ExecMode;
        public bool WriteIntoLog;
    }

    public class TransactionParams
    {
        public string Name;
        public string[] Params;
    }

}

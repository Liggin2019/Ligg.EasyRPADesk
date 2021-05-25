namespace Ligg.EasyWinApp.Parser.DataModel
{
    using Ligg.EasyWinApp.Parser.DataModel.Enums;
    public class Transaction
    {
        public string ControlName;
        public string Action;
        public string DisplayName;

        public string ExecModeFlag;
        public string ShowRunningStatusFlag;
        public string WriteIntoLogFlag;
    }

    public class TransactionDetail
    {
        public string ActionName;
        public string[] ActionParams;
        public string DisplayName;

        public TransactionExecMode ExecMode;
        public bool WriteIntoLog;
        public bool ShowRunningStatus;
    }

    public class TransactionParams
    {
        public string ActionName;
        public string[] ActionParams;
    }

}

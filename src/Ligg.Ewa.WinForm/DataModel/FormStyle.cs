
namespace Ligg.EasyWinApp.WinForm.DataModel
{
    public class FormStyle
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TopLocationX { get; set; }//no use 
        public int TopLocationY { get; set; }//no use 

        public string StyleSheetCode { get; set; }

        //public bool MaximizeBox { get; set; }
        //public bool MinimizeBox { get; set; }
        //public bool HasNoControlBox { get; set; }
        //public bool TreatCloseBoxAsMinimizeBox { get; set; }
        public bool HasNoControlBoxes { get; set; }
        public bool CannotBeClosed { get; set; }
        public bool CannotBeMaximized { get; set; }

        public string StartWindowState { get; set; }//x
        public string OrdinaryWindowStatus { get; set; }

        public int WindowRadius { get; set; }

        public bool DrawIcon { get; set; }
        public string IconUrl { get; set; }

        public bool HasTray { get; set; }
        public string TrayIconUrl { get; set; }
        public string TrayDataSource { get; set; }

        public string ResizeParamsText { get; set; }

        public bool ShowRunningStatus { get; set; }
        public bool SupportsMultipleThreads { get; set; }
        public int ThreadPoolMaxNum { get; set; }


    }

}

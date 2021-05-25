
namespace Ligg.EasyWinApp.WinForm.DataModel
{
    public class ZoneFeature
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CpntArrangementType { get; set; }
        public string CpntArrangementTypeFlag { get; set; }
        public string StyleText { get; set; }
        public string InputParams { get; set; }
        public string DataSource { get; set; }

        public int FormWindowRadius { get; set; }
        public bool FormCannotBeClosed { get; set; }
        public bool FormHasNoControlBoxes { get; set; }

        public bool FormDrawIcon { get; set; }
        public string FormIconUrl { get; set; }

        public bool FormHasTray { get; set; }
        public string FormTrayIconUrl { get; set; }
        public string FormTrayDataSource { get; set; }

        public bool FormShowRunningStatus { get; set; }

        public bool SupportsMultipleThreads { get; set; }
        public int ThreadPoolMaxNum { get; set; }


    }

}

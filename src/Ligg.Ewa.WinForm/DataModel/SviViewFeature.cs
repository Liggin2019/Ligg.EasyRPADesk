﻿
namespace Ligg.EasyWinApp.WinForm.DataModel
{
    public class SviViewFeature
    {
        public int FormWidth { get; set; }
        public int FormHeight { get; set; }
        public string FormResizeParamsText { get; set; }
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

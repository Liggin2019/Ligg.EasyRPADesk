
namespace Ligg.RpaDesk.WinForm.DataModels
{
    public class FormStyle
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public bool HasNoControlBoxes { get; set; }

        public bool HasNoMaximizeBox { get; set; }
        public bool HasNoMinimizeBox { get; set; }
        public string StartWindowState { get; set; }//x
        public string OrdinaryWindowStatus { get; set; }
        
        public bool TopMost { get; set; }
        public bool DrawIcon { get; set; }
        public string IconUrl { get; set; }
        public bool NotShowInTaskbar { get; set; }

        public int TopLocationX { get; set; }//no use 
        public int TopLocationY { get; set; }//no use 

        public string ResizeParamsText { get; set; }//for mvi/svi

        public int RunningMessageHeight { get; set; }
        public bool ShowRunningStatus { get; set; }
        public bool ShowRunningProgress { get; set; }


    }

}

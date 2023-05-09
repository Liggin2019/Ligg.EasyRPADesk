namespace Ligg.RpaDesk.WinForm.DataModels.Enums
{

    public enum ControlType
    {
        ContextMenuStripEx=10, //tray
        MenuStrip =100,
        ToolStripMenuItem =101,
        ImageToolStripMenuItemH=102,
        ImageToolStripMenuItemV=103,

        ToolStrip = 200,
        ToolLabel = 210,
        ToolButton =211,

        ToolStripButton=220,//real ControlName
        ImageToolButton = 221,
        ImageTextToolButtonH=222, 
        ImageTextToolButtonV = 224,

        ToolStripSplitButtonEx=230, //real ControlName
        ToolSplitButton = 231,
        ImageToolSplitButton = 232,
        ImageTextToolSplitButtonH = 233,
        ImageTextToolSplitButtonV = 234,

        Panel = 1000,
        ContainerPanel = 1001,
        ShadowPanel = 1002,
        SplitRectangle = 1003,
        GroupBox = 1004,
        Row = 1009,//virtual

        Label = 1010,
        TitleLabel = 1011,

        Button = 1020,
        TextButton = 1021,
        ImageTextButtonH = 1022,
        ImageTextButtonV = 1023,
        PictureBox = 1024,

        TextBox = 1030,
        RichTextBox = 1031,

        OptionCtrl=1040,
        RadioButton = 1041,
        CheckBox = 1042,
        ComboBox = 1043,

        StatusCtrl = 1050,
        ProgressBar = 1051,
        StatusLight = 1052,
        ScoreLight = 1053,

        DataCtrl=1060,
        TreeViewEx = 1061,
        ListViewEx = 1062,
        DataGridEx=1063,

        TimerEx = 1070,

        CommandLabel = 1111,
        KeyWatchLabel = 1112,

        TabButton=1121,

        Searcher = 1161,
        HorTreeView = 1162,
        VertreeView = 1163,
        PagedListView =1164,
        SortedListView = 1165,
         
        AtTimeTimer = 1171,
        OnTimeTimer = 1172,


    }

}

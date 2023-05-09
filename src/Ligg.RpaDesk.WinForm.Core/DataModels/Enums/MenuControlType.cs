namespace Ligg.RpaDesk.WinForm.DataModels.Enums
{

    public enum MenuControlType
    {
        MenuStrip =100,//hor menu
        ToolStripMenuItem =101,
        ImageToolStripMenuItemH=102,
        ImageToolStripMenuItemV=103,

        ToolStrip = 200,//nested menu
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

        TextButton = 1021,
        ImageTextButtonH = 1022,
        ImageTextButtonV = 1023,
        CommandLabel = 1111,

    }

}

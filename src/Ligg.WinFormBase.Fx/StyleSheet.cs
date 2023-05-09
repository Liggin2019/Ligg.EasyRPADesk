using System;
using System.Drawing;

namespace Ligg.WinFormBase
{
    public static class StyleSheet
    {
        public static void SetStyleSet(string cfgXmlPath)
        {

        }

        public static Color ColorDefault = Color.Black;
        public static Color ColorCommand = Color.DarkBlue;
        public static Color ColorSucceeded = Color.ForestGreen;
        public static Color ColorError = Color.Red;
        public static Color ColorWeek = Color.FromArgb(180, 180, 180);

        //#skin set
        //##base
        public static Color BaseColor = Color.FromArgb(48, 145, 242);
        public static Color GroundColor = Color.FromArgb(255, 255, 255);

        //##form
        public static Color CaptionActiveColor = BaseColor;
        public static Color CaptionInactiveColor = Color.FromArgb(131, 209, 255);
        public static Color CaptionTextColor = Color.FromArgb(255, 255, 255);

        public static Color FormBorderColor = BaseColor;
        //public static Color FormInnerBorderColor = Color.Transparent;
        public static Color FormInnerBorderColor = BaseColor; 

        public static Color ControlBoxActiveColor = BaseColor;
        public static Color ControlBoxInactiveColor = Color.FromArgb(48, 145, 242);
        public static Color ControlBoxInnerBorderColor = Color.FromArgb(250, 250, 250);
        public static Color ControlBoxHoveringColor = Color.FromArgb(37, 114, 151);
        public static Color ControlBoxPressedColor = Color.FromArgb(27, 84, 111);
        public static Color CloseControlBoxHoveringColor = Color.FromArgb(213, 66, 22);
        public static Color CloseControlBoxPressedColor = Color.FromArgb(171, 53, 17);

        //#section
        public static Color NavigationSectionBackColor = Color.FromArgb(244, 245, 247);
        public static Color ShortcutSectionBackColor = Color.FromArgb(248, 248, 248);
        public static Color RunningStatusSectionBackColor = Color.FromArgb(246, 246, 246);


        //#division         
        public static Color MainSectionLeftNavDivisionBackColor = Color.FromArgb(246, 250, 243);
        public static Color MainSectionRightNavDivisionBackColor = Color.FromArgb(246, 250, 243);
        public static Color MainSectionHorizontalResizeDivisionBackColor = Color.FromArgb(251, 255, 245);

        //#head toolstrip
        public static Color HeadToolStripBackColor = BaseColor;
        public static Color HeadToolStripItemHoveringBackColor = Color.FromArgb(64, 134, 235);
        public static Color HeadToolStripItemPressedBackColor = GraphicsHelper.GetColor(HeadToolStripItemHoveringBackColor, 0, -2 * 13, -2 * 8, -2 * 3);
        public static Color HeadToolStripItemCheckedBorderColor = HeadToolStripItemPressedBackColor;
        public static Color HeadToolStripItemBorderColor = GraphicsHelper.GetColor(HeadToolStripItemPressedBackColor, 0, -2 * 13, -2 * 8, -2 * 3);
        public static Color HeadToolStripItemInnerBorderColor = FormInnerBorderColor;

        //ToolStrip DropDown including head toolstrip DropDown  
        public static Color ToolStripDropDownBackColor = Color.FromArgb(253, 253, 253);
        public static Color ToolStripDropDownBorderColor = Color.FromArgb(233, 238, 238);
        public static Color ToolStripDropDownPressedCenterColor = Color.FromArgb(233, 238, 238);
        public static Color ToolStripDropDownSeparatorColor = Color.FromArgb(197, 197, 197);

        public static Color ToolStripMenuItemHoveredBackColor = Color.FromArgb(152, 200, 249);
        public static Color ToolStripMenuItemPressedBackColor = Color.FromArgb(208, 227, 252);
        public static Color ToolStripMenuItemBorderColor = Color.FromArgb(197, 197, 197);
        public static Color ToolStripMenuItemInnerBorderColor = FormInnerBorderColor;

        //#Control
        public static Color ControlBackColor = Color.FromArgb(222, 242, 252);
        public static Color ControlHoveringBackColor = GraphicsHelper.GetColor(ControlBackColor, 0, -2 * 13, -2 * 8, -2 * 3);//22,129,236
        public static Color ControlPressedBackColor = GraphicsHelper.GetColor(ControlHoveringBackColor, 0, -2 * 13, -2 * 8, -2 * 3);
        public static Color ControlCheckedBackColor = BaseColor;
        public static Color ControlDisabledBackColor = Color.FromArgb(204, 204, 204);
        public static Color ControlBorderColor = Color.FromArgb(197, 197, 197);
        public static Color ControlInnerBorderColor = FormInnerBorderColor;
        public static Color ControlCheckedBorderColor = ControlPressedBackColor;

        public static Color ControlTextWeakColor = Color.FromArgb(180, 180, 180);

        //#control Class
        public static string SplitRectangleClass_Level1 = "BackColor:200,200,200|";
        public static string SplitRectangleClass_Level2 = "BackColor:235,236,235|";

        //##Panel
        public static string ContainerPanelClass_Rectangle = "Style:Borders|BorderWidth:1";
        public static string ContainerPanelClass_Ellipse = "Style:Rounded|Radius:15";

        public static string ShadowPanelClass_Default = "BackColor:250,250,250|BorderColor:180, 180, 180";

        public static Color PopupContainerBackColor = Color.FromArgb(250, 250, 250);
        public static Color PopupContainerBorderColor = Color.FromArgb(180, 180, 180);

        //##Label
        //default Font:Microsoft Sans Serif
        public static string LabelClass_FilledLabel = "BackColor:222, 242, 252|FontStyle:Bold|FontSize:8.00|TextAlign:MiddleCenter|ForeColor:255,255,255";
        public static string LabelClass_Level1 = "FontStyle:Bold|FontSize:9.00";
        public static string LabelClass_Level2 = "FontStyle:Bold|FontSize:8.00";
        public static string LabelClass_FieldName = "FontStyle:Bold|FontSize:8.00|TextAlign:MiddleRight";

        public static string TitleLabelClass_Level1 = "FontStyle:Bold|FontSize:9.00|TextAlign:MiddleLeft|HasBottomLine:true|BottomLineColor:200,200,200";
        public static string TitleLabelClass_Level2 = "FontStyle:Bold|FontSize:8.50|TextAlign:MiddleLeft|HasBottomLine:true|BottomLineColor:235,236,235";
        public static string TitleLabelClass_MenuItem = "FontStyle:Bold|FontSize:8.50|TextAlign:MiddleLeft|HasBottomLine:true|BottomLineColor:180,180,180";

        public static string CommandLabelClass_CommandLabel = "CheckType:None|ForeColor:160,160,160";
        public static string CommandLabelClass_ButtonLabel = "CheckType:Focus|ForeColor:160,160,160|CheckedColor:48, 145, 242|FontStyle:Bold";
        public static string CommandLabelClass_LinkedLabel = "CheckType:Click|ForeColor:160,160,160|HoveringColor:140, 0, 0|CheckedColor:230, 0, 0|FontStyle:Underline";
        public static string CommandLabelClass_MenuItem = "CheckType:Input|ForeColor:88,88,88|HoveringColor:22,129,236|CheckedColor:48, 145, 242";

        //##button
        public static string TextButtonClass_MenuItem = "CheckType:Input|BackColor:" + "191,219,255" + "|Font:Arial|FontSize:8.7|TextAlign:MiddleLeft";
        public static string TextButtonClass_Button = "CheckType:Focus|BackColor:" + "222, 242, 252" + "|Font:Arial|FontSize:8.7|TextAlign:MiddleCenter|HasBorder:true";

        public static string ImageTextButtonClass_MenuItem = "CheckType:Input|BackColor:" + "191,219,255" + "|Font:Arial|FontSize:8.7|TextAlign:MiddleLeft";
        public static string ImageTextButtonClass_Button = "CheckType:Focus|BackColor:" + "222, 242, 252" + "|FontStyle:bold;Font:Arial|FontSize:8.7|TextAlign:MiddleCenter|HasBorder:true";

        //##TextBox
        public static string RichTextBoxClass_Error = "ForeColor:255,0,0|BorderStyle:none";

        public static string CommandTextBoxClass_Top = "BackColor:28,125,222|ForeColor:180,180,180|CustForeColor:255,255,255";
        public static string CommandTextBoxClass_Default = "BackColor:252,252,252|ForeColor:180,180,180|CustForeColor:1,1,1";

        public static string DateTimeTextBoxClass_Default = "CustomDateTimeFormat:yyyy-MM-dd";
        
        //##option ctrls
        public static string CheckBoxClass_Level1 = "FontStyle:Bold|FontSize:8.00";

        //##status ctrls
        public static string StatusLightClass_Validator = "ArrangementType:Horizonal|HideLight:true|LabelWidth:0|MsgForeColor:255,0,0|";

        //##data ctrls
        public static string TreeViewExClass_Menu = "GetValueType:Name|TopId:0|HasBorder:true";
        public static string SearcherClass_Default = "BackColor:235,235,235|LabelWidth:50|RowHeight:28";
        public static string PagedListViewClass_Default = "BackColor:235,235,235|PageSize:30|ViewType:Details|BlankColumnWitdth:26|RowHeight:22";
        public static string SortedListViewClass_Default = "BackColor:235,235,235|";

        //##Timmer
        public static string TimerExTimingRunClass_Default = "BackColor:235,235,235|";
        public static string TimerExLoopingRunClass_Default = "BackColor:235,235,235|";




    }
}

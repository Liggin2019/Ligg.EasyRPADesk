using Ligg.RpaDesk.Parser.Helpers;
using Ligg.RpaDesk.WinForm.Controls;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.WinFormBase;
using Ligg.Infrastructure.Extensions;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using  Ligg.Infrastructure.Utilities.DataParserUtil;
using System.Collections.Generic;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Ligg.RpaDesk.WinForm.Helpers
{
    public static partial class ControlHelper
    {
        //*ToolStrip
        public static void SetToolStripMenuItemStyleByText(ToolStripMenuItem cpnt, string styleText)
        {
            /*--not work
            var backColorStr = styleText.GetLdictValue("BackColor");
            if (!string.IsNullOrEmpty(backColorStr))
            {
                if (backColorStr.ToLower() == "transparent")
                {
                    cpnt.BackColor = Color.Transparent;
                }
                else
                {
                    var backColorStrArray = backColorStr.Split(',');
                    cpnt.BackColor = Color.FromArgb(Convert.ToInt32(backColorStrArray[0]),
                        Convert.ToInt32(backColorStrArray[1]),
                        Convert.ToInt32(backColorStrArray[2]));
                }

            }
            */

            //*valid for each level.
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var foreColorStr = dict.GetLdictValue("ForeColor").ToLower();
            if (!string.IsNullOrEmpty(foreColorStr))
            {
                if (foreColorStr.ToLower() == "transparent")
                {
                    cpnt.ForeColor = Color.Transparent;
                }
                else
                {
                    var backColorStrArray = foreColorStr.GetLarrayArray(true,true);
                    cpnt.ForeColor = Color.FromArgb(Convert.ToInt32(backColorStrArray[0]),
                        Convert.ToInt32(backColorStrArray[1]),
                        Convert.ToInt32(backColorStrArray[2]));
                }
            }

            //--only valid&works for top level 
            var textAlign = dict.GetLdictValue("TextAlign").ToLower().ToLower();
            if (!string.IsNullOrEmpty(textAlign))
            {
                switch (textAlign)
                {
                    case "middleleft":
                        cpnt.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                    case "middleright":
                        cpnt.TextAlign = ContentAlignment.MiddleRight;
                        break;
                    default:
                        cpnt.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                }
            }

            //--only valid for top level. works form top level to end level
            //--when culture is zh-CN, western font not work. When change to en-US, works

            var fontStr = dict.GetLdictValue("Font");
            if (string.IsNullOrEmpty(fontStr))
            {
                fontStr = "Font:Microsoft Sans Serif";
            }

            //--valid for each level. works from self level to end level
            float fontSize = 8;
            var fontSizeStr = dict.GetLdictValue("FontSize");
            if (string.IsNullOrEmpty(fontSizeStr)) fontSize = cpnt.Font.Size;
            else fontSize = Convert.ToSingle(fontSizeStr, CultureInfo.InvariantCulture);

            //--valid for each level. works from self level to next level
            var fontStyleStrs = dict.GetLdictValue("FontStyle").ToLower();
            var fontStyle = new FontStyle();
            if (!string.IsNullOrEmpty(fontStyleStrs))
            {
                var fontStyleStrArry =fontStyleStrs.GetLarrayArray(true, true);
                foreach (var fontStyleStr in fontStyleStrArry)
                {
                    if (fontStyleStr.ToLower() == "bold")
                    {
                        fontStyle = fontStyle | FontStyle.Bold;
                    }
                    else if (fontStyleStr.ToLower() == "italic")
                    {
                        fontStyle = fontStyle | FontStyle.Italic;
                    }
                    else if (fontStyleStr.ToLower() == "underline")
                    {
                        fontStyle = fontStyle | FontStyle.Underline;
                    }
                    else if (fontStyleStr.ToLower() == "strikeout")
                    {
                        fontStyle = fontStyle | FontStyle.Strikeout;
                    }
                }

            }
            else
            {
                fontStyle = cpnt.Font.Style;
            }
            cpnt.Font = new Font(fontStr, fontSize, fontStyle);
        }

        //*Panel
        public static void SetSplitRectangleStyleByClass(SplitRectangle control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("SplitRectangleClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));

            SetSplitRectangleStyleByText(control, styleText);
        }
        public static void SetSplitRectangleStyleByText(SplitRectangle cpnt, string styleText)
        {
            ControlBaseHelper.SetControlBackColor(cpnt, styleText);
            ControlBaseHelper.SetControlPadding(cpnt, styleText);

        }

        public static void SetPanelStyleByClass(Panel control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("PanelClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));

            SetPanelStyleByText(control, styleText);
        }
        public static void SetPanelStyleByText(Panel control, string styleText)
        {
            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
        }

        //#ShadowPanel
        public static void SetShadowPanelStyleByClass(Panel control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("ShadowPanelClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));

            SetShadowPanelStyleByText(control, styleText);
        }
        public static void SetShadowPanelStyleByText(Panel control, string styleText)
        {
            SetPanelStyleByText(control, styleText);
        }

        //#ContainerPanel
        public static void SetContainerPanelStyleByClass(ContainerPanel control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("ContainerPanelClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));

            SetContainerPanelStyleByText(control, styleText);
        }
        public static void SetContainerPanelStyleByText(ContainerPanel control, string styleText)
        {
            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var style = dict.GetLdictValue("Style").ToLower();
            if (!string.IsNullOrEmpty(style))
            {
                if (style == "borders" | style == "rounded")
                {
                    var borderColor = dict.GetLdictValue("BorderColor").ToLower();
                    if (!borderColor.IsNullOrEmpty())
                    {
                        var boderColorArray = borderColor.Split(',');
                        control.BorderColor = Color.FromArgb(Convert.ToInt32(boderColorArray[0]),
                                                                   Convert.ToInt32(boderColorArray[1]),
                                                                   Convert.ToInt32(boderColorArray[2]));

                    }

                    if (style == "borders")
                    {
                        control.StyleType = ContainerPanel.ContainerPanelStyle.Borders;

                        var borderWidth = dict.GetLdictValue("BorderWidth");
                        if (!borderWidth.IsNullOrEmpty())
                        {
                            var borderWidthArray = borderWidth.Split(',');
                            if (borderWidthArray.Length == 1)
                            {
                                var w = Convert.ToInt16(borderWidthArray[0]);
                                control.BorderWidthOnTop = w;
                                control.BorderWidthOnRight = w;
                                control.BorderWidthOnBottom = w;
                                control.BorderWidthOnLeft = w;
                            }
                            else if (borderWidthArray.Length == 2)
                            {
                                var w1 = Convert.ToInt16(borderWidthArray[0]);
                                var w2 = Convert.ToInt16(borderWidthArray[1]);
                                control.BorderWidthOnTop = w1;
                                control.BorderWidthOnRight = w2;
                                control.BorderWidthOnBottom = w1;
                                control.BorderWidthOnLeft = w2;
                            }
                            else if (borderWidthArray.Length == 4)
                            {
                                var w1 = Convert.ToInt16(borderWidthArray[0]);
                                var w2 = Convert.ToInt16(borderWidthArray[1]);
                                var w3 = Convert.ToInt16(borderWidthArray[2]);
                                var w4 = Convert.ToInt16(borderWidthArray[3]);
                                control.BorderWidthOnTop = w1;
                                control.BorderWidthOnRight = w2;
                                control.BorderWidthOnBottom = w3;
                                control.BorderWidthOnLeft = w4;
                            }
                        }
                    }
                    else if (style == "rounded")
                    {
                        control.StyleType = ContainerPanel.ContainerPanelStyle.Rounded;
                        control.RoundStyle = RoundStyle.All;
                        var radius = dict.GetLdictValue("Radius");
                        if (!radius.IsNullOrEmpty())
                        {
                            control.Radius = Convert.ToInt16(radius);
                        }
                    }
                }
                SetPanelStyleByText(control, styleText);
            }
        }

        //*label
        public static void SetCommandLabelStyleByClass(CommandLabel control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("CommandLabelClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));
            SetCommandLabelStyleByText(control, styleText);
        }
        public static void SetCommandLabelStyleByText(CommandLabel control, string styleText)
        {
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var checkType = dict.GetLdictValue("CheckType").ToLower();
            if (!checkType.IsNullOrEmpty())
            {
                if (checkType == "click")
                {
                    control.CheckType = ControlCheckType.Click;
                }
                else if (checkType == "focus")
                {
                    control.CheckType = ControlCheckType.Focus;
                }
                else if (checkType == "input")
                {
                    control.CheckType = ControlCheckType.Input;
                }
                else
                {
                    control.CheckType = ControlCheckType.None;
                }
            }

            var hoveringColorStr = dict.GetLdictValue("HoveringColor").ToLower();
            if (!string.IsNullOrEmpty(hoveringColorStr))
            {
                var hoveringColorStrArray = hoveringColorStr.Split(',');
                control.HoveringColor = Color.FromArgb(Convert.ToInt32(hoveringColorStrArray[0]), Convert.ToInt32(hoveringColorStrArray[1]), Convert.ToInt32(hoveringColorStrArray[2]));
            }

            var checkedColorStr = dict.GetLdictValue("CheckedColor");
            if (!string.IsNullOrEmpty(checkedColorStr))
            {
                var checkedColorStrArray = checkedColorStr.Split(',');
                control.CheckedColor = Color.FromArgb(Convert.ToInt32(checkedColorStrArray[0]), Convert.ToInt32(checkedColorStrArray[1]), Convert.ToInt32(checkedColorStrArray[2]));
            }

            ControlHelper.SetLabelStyleByText(control, styleText);
        }


        //#TitleLabel
        public static void SetTitleLabelStyleByClass(TitleLabel control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("TitleLabelClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));
            SetTitleLabelStyleByText(control, styleText);
        }
        public static void SetTitleLabelStyleByText(TitleLabel control, string styleText)
        {
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var hasBottomLineStr = dict.GetLdictValue("HasBottomLine").ToLower();
            if (!string.IsNullOrEmpty(hasBottomLineStr))
            {
                var hasBottomLine = hasBottomLineStr.ToLower() == "true" ? true : false;
                control.HasBottomLine = hasBottomLine;
            }

            var bottomLineColorStr = dict.GetLdictValue("BottomLineColor").ToLower();
            if (!string.IsNullOrEmpty(bottomLineColorStr))
            {
                var fornColorStrArray = bottomLineColorStr.Split(',');
                control.BottomLineColor = Color.FromArgb(Convert.ToInt32(fornColorStrArray[0]), Convert.ToInt32(fornColorStrArray[1]), Convert.ToInt32(fornColorStrArray[2]));
            }

            ControlHelper.SetLabelStyleByText(control, styleText);
        }

        //#Label
        public static void SetLabelStyleByClass(Label control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("LabelClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));

            SetLabelStyleByText(control, styleText);
        }
        public static void SetLabelStyleByText(Label control, string styleText)
        {
            var textAlign = styleText.GetLdictValue("TextAlign", true, true).ToLower();
            if (!string.IsNullOrEmpty(textAlign))
            {
                switch (textAlign)
                {
                    case "middlecenter":
                        control.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                    case "middleleft":
                        control.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                    case "middleright":
                        control.TextAlign = ContentAlignment.MiddleRight;
                        break;
                    case "bottomleft":
                        control.TextAlign = ContentAlignment.BottomLeft;
                        break;
                    case "bottomright":
                        control.TextAlign = ContentAlignment.BottomRight;
                        break;
                    default:
                        control.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                }
            }

            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
            ControlBaseHelper.SetControlBackgroundImage(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
        }

        //*button

        public static void SetButtonStyleByClass(Button control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var styleText = "";
            var myInfo = myType.GetField("TextButtonClass" + "_" + styleClass);
            if (myInfo == null) return;
            styleText = Convert.ToString(myInfo.GetValue(null));

            SetButtonStyleByText(control, styleText);
        }

        public static void SetButtonStyleByText(Button control, string styleText)
        {
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);

            SetButtonTextAlign(control, styleText);
            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
        }

        //#ImageTextButton
        public static void SetImageTextButtonStyleByClass(ImageTextButton control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("ImageTextButtonClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));
            SetImageTextButtonStyleByText(control, styleText);
        }

        public static void SetImageTextButtonStyleByText(ImageTextButton control, string styleText)
        {
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var checkType = dict.GetLdictValue("CheckType").ToLower();
            if (!checkType.IsNullOrEmpty())
            {
                if (checkType == "click")
                {
                    control.CheckType = ControlCheckType.Click;
                }
                else if (checkType == "focus")
                {
                    control.CheckType = ControlCheckType.Focus;
                }
                else if (checkType == "input")
                {
                    control.CheckType = ControlCheckType.Input;
                }
                else
                {
                    control.CheckType = ControlCheckType.None;
                }
            }

            var hasBorder = dict.GetLdictValue("HasBorder").ToLower(); ;
            if (!hasBorder.IsNullOrEmpty())
            {
                if (hasBorder == "false")
                {
                    control.HasBorder = false;
                }
                else
                {
                    control.HasBorder = true;
                }
            }

            var offset = dict.GetLdictValue("Offset").ToLower(); ;
            if (!offset.IsNullOrEmpty())
            {
                control.Offset = offset;
            }

            SetButtonTextAlign(control, styleText);

            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
            ControlBaseHelper.SetControlBackgroundImage(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
        }

        //#TextButton
        public static void SetTextButtonStyleByClass(TextButton control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var styleText = "";
            var myInfo = myType.GetField("TextButtonClass" + "_" + styleClass);
            if (myInfo == null) return;
            styleText = Convert.ToString(myInfo.GetValue(null));

            SetTextButtonStyleByText(control, styleText);
        }

        public static void SetTextButtonStyleByText(TextButton control, string styleText)
        {
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var hasBorder = dict.GetLdictValue("HasBorder").ToLower();
            if (!hasBorder.IsNullOrEmpty())
            {
                if (hasBorder == "false")
                {
                    control.HasBorder = false;
                }
                else
                {
                    control.HasBorder = true;
                }
            }

            var checkType = dict.GetLdictValue("CheckType").ToLower();
            if (!checkType.IsNullOrEmpty())
            {
                if (checkType == "click")
                {
                    control.CheckType = ControlCheckType.Click;
                }
                else if (checkType == "focus")
                {
                    control.CheckType = ControlCheckType.Focus;
                }
                else if (checkType == "input")
                {
                    control.CheckType = ControlCheckType.Input;
                }
                else
                {
                    control.CheckType = ControlCheckType.None;
                }
            }


            SetButtonTextAlign(control, styleText);
            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
        }

        public static void SetButtonTextAlign(Button control, string styleText)
        {
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var textAlign = dict.GetLdictValue("TextAlign").ToLower();
            if (!string.IsNullOrEmpty(textAlign))
            {
                switch (textAlign)
                {
                    case "middleleft":
                        control.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                    case "middleright":
                        control.TextAlign = ContentAlignment.MiddleRight;
                        break;
                    default:
                        control.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                }
            }
        }

        //*textbox
        //#TextBox
        public static void SetTextBoxStyleByClass(TextBox control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("TextBoxClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));
            SetTextBoxStyleByText(control, styleText);
        }

        public static void SetTextBoxStyleByText(TextBox control, string styleText)
        {
            var dict = styleText.ConvertToGeneric<Dictionary<string, string>>(true, TxtDataType.Ldict);
            var borderStyle = dict.GetLdictValue("BorderStyle").ToLower();
            if (!string.IsNullOrEmpty(borderStyle))
            {
                switch (borderStyle)
                {
                    case "none":
                        control.BorderStyle = BorderStyle.None;
                        break;
                    case "fixed3d":
                        control.BorderStyle = BorderStyle.Fixed3D;
                        break;
                    case "fixedsingle":
                        control.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    default:
                        control.BorderStyle = BorderStyle.None;
                        break;
                }
            }

            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
        }

        //#RichTextBox
        public static void SetRichTextBoxStyleByClass(RichTextBox control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("RichTextBoxClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));
            SetRichTextBoxStyleByText(control, styleText);
        }

        public static void SetRichTextBoxStyleByText(RichTextBox control, string styleText)
        {
            var borderStyle = styleText.GetLdictValue("BorderStyle", true, true).ToLower();
            if (!string.IsNullOrEmpty(borderStyle))
            {
                switch (borderStyle)
                {
                    case "none":
                        control.BorderStyle = BorderStyle.None;
                        break;
                    case "fixed3d":
                        control.BorderStyle = BorderStyle.Fixed3D;
                        break;
                    case "fixedsingle":
                        control.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    default:
                        control.BorderStyle = BorderStyle.None;
                        break;
                }
            }

            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
        }

        //*option ctrls
        //#RadioButton
        public static void SetRadioButtonStyleByClass(RadioButton control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("RadioButton" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));

            SetRadioButtonStyleByText(control, styleText);
        }
        public static void SetRadioButtonStyleByText(RadioButton control, string styleText)
        {
            var textAlign = styleText.GetLdictValue("TextAlign", true, true).ToLower();
            if (!string.IsNullOrEmpty(textAlign))
            {
                switch (textAlign)
                {
                    case "middlecenter":
                        control.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                    case "middleleft":
                        control.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                    case "middleright":
                        control.TextAlign = ContentAlignment.MiddleRight;
                        break;
                    case "bottomleft":
                        control.TextAlign = ContentAlignment.BottomLeft;
                        break;
                    case "bottomright":
                        control.TextAlign = ContentAlignment.BottomRight;
                        break;
                    default:
                        control.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                }
            }

            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
        }

        //#CheckBox
        public static void SetCheckBoxStyleByClass(CheckBox control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("CheckBoxClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));
            SetCheckBoxStyleByText(control, styleText);
        }
        public static void SetCheckBoxStyleByText(CheckBox control, string styleText)
        {
            var textAlign = styleText.GetLdictValue("TextAlign", true, true).ToLower();
            if (!string.IsNullOrEmpty(textAlign))
            {
                switch (textAlign)
                {
                    case "middlecenter":
                        control.TextAlign = ContentAlignment.MiddleCenter;
                        break;
                    case "middleleft":
                        control.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                    case "middleright":
                        control.TextAlign = ContentAlignment.MiddleRight;
                        break;
                    case "bottomleft":
                        control.TextAlign = ContentAlignment.BottomLeft;
                        break;
                    case "bottomright":
                        control.TextAlign = ContentAlignment.BottomRight;
                        break;
                    default:
                        control.TextAlign = ContentAlignment.MiddleLeft;
                        break;
                }
            }

            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
            ControlBaseHelper.SetControlFont(control, styleText);
            ControlBaseHelper.SetControlPadding(control, styleText);
        }

        //*status ctrls
        //#ProgressBar
        public static void SetProgressBarStyleByClass(ProgressBar control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("ProgressBarClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));

            SetProgressBarStyleByText(control, styleText);
        }
        public static void SetProgressBarStyleByText(ProgressBar control, string styleText)
        {
            ControlBaseHelper.SetControlBackColor(control, styleText);
            ControlBaseHelper.SetControlForeColor(control, styleText);
        }
        public static void SetDateTimePickerExStyleByClass(DateTimePickerEx control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var myInfo = myType.GetField("DateTimePickerExClass" + "_" + styleClass);
            if (myInfo == null) return;
            var styleText = Convert.ToString(myInfo.GetValue(null));
            SetDateTimePickerExStyleByText(control, styleText);
        }

        public static void SetDateTimePickerExStyleByText(DateTimePickerEx control, string styleText)
        {
            control.StyleText = styleText;
            ControlBaseHelper.SetControlPadding(control, styleText);
        }


        public static void SetStatusLightStyleByClass(StatusLight control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var styleText = "";
            var myInfo = myType.GetField("StatusLightClass" + "_" + styleClass);
            if (myInfo == null) return;
            styleText = Convert.ToString(myInfo.GetValue(null));
            SetStatusLightStyleByText(control, styleText);
        }
        public static void SetStatusLightStyleByText(StatusLight control, string styleText)
        {
            control.StyleText = styleText;
            ControlBaseHelper.SetControlPadding(control, styleText);
            ControlBaseHelper.SetControlBackColor(control, styleText);
        }



        //*data ctrls
        //#TreeViewEx
        public static void SetTreeViewExStyleByClass(TreeViewEx control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var styleText = "";
            var myInfo = myType.GetField("TreeViewExClass" + "_" + styleClass);
            if (myInfo == null) return;
            styleText = Convert.ToString(myInfo.GetValue(null));

            SetTreeViewExStyleByText(control, styleText);
        }
        public static void SetTreeViewExStyleByText(TreeViewEx control, string styleText)
        {
            control.StyleText = styleText;
            ControlBaseHelper.SetControlPadding(control, styleText);
            ControlBaseHelper.SetControlBackColor(control, styleText);
            //SetControlForeColor(control, styleText);
            //SetControlFont(control, styleText);
        }

        public static void SetDataGridViewExStyleByClass(DataGridViewEx control, string styleClass)
        {
            if (styleClass.IsNullOrEmpty()) return;
            var myType = typeof(StyleSheet);
            var styleText = "";
            var myInfo = myType.GetField("DataGridViewExClass" + "_" + styleClass);
            if (myInfo == null) return;
            styleText = Convert.ToString(myInfo.GetValue(null));

            SetDataGridViewExStyleByText(control, styleText);
        }
        public static void SetDataGridViewExStyleByText(DataGridViewEx control, string styleText)
        {
            control.StyleText = styleText;
            ControlBaseHelper.SetControlPadding(control, styleText);
            //ControlBaseHelper.SetControlBackColor(control, styleText);
        }

    }
}
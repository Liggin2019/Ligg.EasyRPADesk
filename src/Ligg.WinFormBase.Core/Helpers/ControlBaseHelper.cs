
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
//using Ligg.Infrastructure.Utilities.DataParserUtil;

namespace Ligg.WinFormBase
{
    public static partial class ControlBaseHelper
    {
        private static readonly string _typeFullName = System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName;

        //*set
        public static void SetControlDockStyleAndLocationAndSize(Control control, int dockStyle, int width, int height, int x, int y)
        {
            if (dockStyle == (int)ControlDockType.None)
            {
                control.Dock = DockStyle.None;
                control.Width = width;
                control.Height = height;
                control.Location = new Point(x, y);
            }
            else if (dockStyle == (int)ControlDockType.Top)
            {
                control.Dock = DockStyle.Top;
                control.Height = height;
            }
            else if (dockStyle == (int)ControlDockType.Right)
            {
                control.Dock = DockStyle.Right;
                control.Width = width;
            }
            else if (dockStyle == (int)ControlDockType.Bottom)
            {
                control.Dock = DockStyle.Bottom;
                control.Height = height;
            }
            else if (dockStyle == (int)ControlDockType.Left)
            {
                control.Dock = DockStyle.Left;
                control.Width = width;
            }
            else if (dockStyle == (int)ControlDockType.Fill)
            {
                control.Dock = DockStyle.Fill;
            }
            //else //regard as none
            //{
            //    control.Dock = DockStyle.None;
            //    control.Width = width;
            //    control.Height = height;
            //    control.Location = new Point(x == -1 ? StyleSet.DefaultOffsetX : x, y == -1 ? StyleSet.DefaultOffsetY : y);
            //}
        }

        public static void SetControlOffsetByDockStyle(Control offsetCrtl, int dockStyle, int offsetX, int offsetY)
        {
            if (dockStyle == (int)ControlDockType.Top)
            {
                offsetCrtl.Dock = DockStyle.Top;
                offsetCrtl.Height = offsetY < 0 ? 0 : offsetY;
            }
            else if (dockStyle == (int)ControlDockType.Right)
            {
                offsetCrtl.Dock = DockStyle.Right;
                offsetCrtl.Width = offsetX;
            }
            else if (dockStyle == (int)ControlDockType.Bottom)
            {
                offsetCrtl.Dock = DockStyle.Bottom;
                offsetCrtl.Height = offsetY;
            }
            else if (dockStyle == (int)ControlDockType.Left)
            {
                offsetCrtl.Dock = DockStyle.Left;
                offsetCrtl.Width = offsetX;
            }
            else //none or fill
            {
                //no this case
            }
        }

        public static void SetControlPadding(Control control, string styleText)
        {
            var padding = styleText.GetStyleValue("Padding").ToLower();
            if (!string.IsNullOrEmpty(padding))
            {
                var paddingArry = padding.GetLarrayArray(true, true);
                //if (!padding.Contains(","))
                if (paddingArry.Length == 1)
                {
                    control.Padding = new Padding(Convert.ToInt32(padding));
                }
                //else if (padding.GetQtyOfIncludedChar(',') == 1) //2
                else if (paddingArry.Length == 2)
                {
                    control.Padding = new Padding(Convert.ToInt32(paddingArry[1]), Convert.ToInt32(paddingArry[0]), Convert.ToInt32(paddingArry[1]), Convert.ToInt32(paddingArry[0]));
                }
                //else if (padding.GetQtyOfIncludedChar(',') == 2) //3
                else if (paddingArry.Length == 3)
                {
                    //var paddingArry = padding.Split(',');
                    control.Padding = new Padding(Convert.ToInt32(paddingArry[1]), Convert.ToInt32(paddingArry[0]), Convert.ToInt32(paddingArry[1]), Convert.ToInt32(paddingArry[2]));
                }
                //else if (padding.GetQtyOfIncludedChar(',') == 3) //4
                else if (paddingArry.Length == 4)
                {
                    //var paddingArry = padding.Split(',');
                    control.Padding = new Padding(Convert.ToInt32(paddingArry[3]), Convert.ToInt32(paddingArry[0]), Convert.ToInt32(paddingArry[1]), Convert.ToInt32(paddingArry[2]));
                }
            }
        }
        public static void SetControlBackColor(Control control, string styleText)
        {
            var backColorStr = styleText.GetStyleValue("BackColor");

            if (!string.IsNullOrEmpty(backColorStr))
            {
                if (backColorStr.ToLower() == "transparent")
                {
                    control.BackColor = Color.Transparent;
                }
                else
                {
                    //var backColorStrArray = backColorStr.Split(',');
                    var backColorStrArray = backColorStr.GetLarrayArray(true, true); 
                    if (backColorStrArray.Length < 3)
                    {
                        throw new ArgumentException("backColorStrArray length can't be less than 3; styleText= " + styleText);
                    }
                    control.BackColor = Color.FromArgb(Convert.ToInt32(backColorStrArray[0]),
                                                               Convert.ToInt32(backColorStrArray[1]),
                                                               Convert.ToInt32(backColorStrArray[2]));
                }
            }
        }

        public static void SetControlForeColor(Control control, string styleText)
        {
            var foreColorStr = styleText.GetStyleValue("ForeColor");

            if (!string.IsNullOrEmpty(foreColorStr))
            {
                if (foreColorStr.ToLower() == "transparent")
                {
                    control.ForeColor = Color.Transparent;
                }
                else
                {
                    //var foreColorStrArray = foreColorStr.Split(',');
                    var foreColorStrArray = foreColorStr.GetLarrayArray(true, true);
                    if (foreColorStrArray.Length < 3)
                    {
                        throw new ArgumentException("foreColorStrArray length can't be less than 3; styleText= " + styleText);
                    }
                    control.ForeColor = Color.FromArgb(Convert.ToInt32(foreColorStrArray[0]),
                        Convert.ToInt32(foreColorStrArray[1]),
                        Convert.ToInt32(foreColorStrArray[2]));
                }
            }
        }

        public static void SetControlFont(Control control, string styleText)
        {
            var fontStr = styleText.GetStyleValue("Font").ToLower();
            if (string.IsNullOrEmpty(fontStr))
            {
                fontStr = "Font:Microsoft Sans Serif";
            }

            float fontSize = 8;
            var fontSizeStr = styleText.GetStyleValue("FontSize");
            if (string.IsNullOrEmpty(fontSizeStr)) fontSize = control.Font.Size;
            else fontSize = Convert.ToSingle(fontSizeStr, CultureInfo.InvariantCulture);

            var fontStyleStrs = styleText.GetStyleValue("FontStyle").ToLower();
            var fontStyle = new FontStyle();
            if (!string.IsNullOrEmpty(fontStyleStrs))
            {
                var fontStyleStrArry = fontStyleStrs.Split(',');

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
                fontStyle = control.Font.Style;
                //control.Font = new Font(fontStr, fontSize);
            }
            control.Font = new Font(fontStr, fontSize, fontStyle);
        }

        public static void SetControlBackgroundImage(Control control, string imgUrl)
        {
            if (!string.IsNullOrEmpty(imgUrl) && System.IO.File.Exists(imgUrl))
            {
                control.BackgroundImage = GetImage(imgUrl);
                control.BackgroundImageLayout = ImageLayout.Zoom;
            }
        }
        //*Hide
        public static void HideControlByDockStyle(Control control, int dockStyle)
        {
            if (dockStyle == (int)ControlDockType.None)
            {
                control.Dock = DockStyle.None;
                control.Width = 0;
                control.Height = 0;
            }
            else if (dockStyle == (int)ControlDockType.Top)
            {
                control.Dock = DockStyle.Top;
                control.Height = 0;
            }
            else if (dockStyle == (int)ControlDockType.Right)
            {
                control.Dock = DockStyle.Right;
                control.Width = 0;
            }
            else if (dockStyle == (int)ControlDockType.Bottom)
            {
                control.Dock = DockStyle.Bottom;
                control.Height = 0;
            }
            else if (dockStyle == (int)ControlDockType.Left)
            {
                control.Dock = DockStyle.Left;
                control.Width = 0;
            }
            else if (dockStyle == (int)ControlDockType.Fill)
            {
                control.Dock = DockStyle.None;
                control.Width = 0;
                control.Height = 0;
            }
            else //regard as none
            {
                control.Dock = DockStyle.None;
                control.Width = 0;
                control.Height = 0;
            }
        }

        //*common
        public static Control GetControlByNameByParent(string name, Control parent)
        {
            if (parent == null)
            {
                throw new ArgumentException("parent can't be null! ");
            }

            foreach (Control Item in parent.Controls)
            {
                if (Item.Name == name)
                {
                    return Item;
                }
                else
                {
                    var ctrl = GetControlByNameByParentForRecursionSubCall(name, Item);
                    if (ctrl != null)
                    {
                        return ctrl;
                    }
                }
            }

            throw new ArgumentException("\n>>  " + _typeFullName + ".GetControlByNameByContainer Error: GetControlByNameByParent Error; name,parent=" + name + "," + parent.Name + ", No Control Found");
        }

        public static Control GetControlByNameByParentForRecursionSubCall(string name, Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.Name == name)
                {
                    return ctrl;
                }
                else
                {
                    var subCtrl = GetControlByNameByParentForRecursionSubCall(name, ctrl);
                    if (subCtrl != null)
                    {
                        return subCtrl;
                    }
                }
            }
            return null;
        }
        public static Control GetControlByNameByContainer(Control container, string ctrlName)
        {
            var ctrls = container.Controls.Find(ctrlName, true);
            if (ctrls.Length == 0) throw new ArgumentException(_typeFullName + ".GetControlByNameByContainer Error: Control does not exist! ctrlName=" + ctrlName + "; container=" + container);
            return ctrls[0];
        }

        //*sub common
        public static Image GetImage(string imagUrl)
        {
            var isLocalPath = true;
            if (isLocalPath)
                if (!string.IsNullOrEmpty(imagUrl) && System.IO.File.Exists(imagUrl))
                {
                    Stream strm = File.Open(imagUrl, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var image = Image.FromStream(strm);
                    strm.Close();
                    return image;
                }
                else
                {

                }
            return null;
        }



    }
}
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Controls
{
    public class TextButton : Button
    {

        public TextButton()
            : base()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);

        }

        private RoundStyle _roundStyle;
        //[DefaultValue(typeof(RoundStyle), "1")]
        public RoundStyle RoundStyle
        {
            get { return _roundStyle; }
            set
            {
                _roundStyle = value;
            }
        }

        private int _radius;
        public int Radius
        {
            get { return _radius; }
            set
            {
                if (_radius != value)
                {
                    _radius = value < 4 ? 4 : value;
                }
            }
        }

        public bool HasBorder { get; set; }

        private ControlState _controlState;
        private ControlState ControlState
        {
            get { return _controlState; }
            set
            {
                if (_controlState != value) //changed
                {
                    _controlState = value;
                    base.Invalidate();
                }
            }
        }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checkType== ControlCheckType.Input&_checked != value) //changed
                {
                    _checked = value;
                    base.Invalidate();
                }
            }
        }

        private ControlCheckType _checkType;
        public ControlCheckType CheckType
        {
            get { return _checkType; }
            set { _checkType = value; }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            ControlState = ControlState.Hovering;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
             base.OnMouseLeave(e);
            ControlState = ControlState.Normal;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
           base.OnMouseDown(e); // if without this code, will lose .click event
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                if (_checkType == ControlCheckType.Click)
                    _checked = true;
                ControlState = ControlState.Pressed;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                if (ClientRectangle.Contains(e.Location))
                {
                    if (_checkType == ControlCheckType.Focus)
                    {
                        ControlState = Focused ? ControlState.Normal : ControlState.Hovering;
                    }
                    else if (_checkType == ControlCheckType.Click)
                    {
                        ControlState = Checked ? ControlState.Normal : ControlState.Hovering;
                    }
                    else if (_checkType == ControlCheckType.Input)
                    {
                        ControlState = Checked ? ControlState.Normal : ControlState.Hovering;
                    }
                    else
                    {
                        ControlState = ControlState.Hovering;
                    }
                }
            }
        }

        //if has following codes, _controlState always be ControlState.Normal
        //protected override void OnCreateControl()
        //{       
        //    _checkType = ControlCheckType.None;
        //    _checked = false;
        //    _controlState = ControlState.Normal;
        //}
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            //base.OnPaintBackground(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color backColor;
            if (Enabled)
            {
                switch (_controlState)
                {
                    case ControlState.Hovering:
                        backColor = StyleSheet.ControlHoveringBackColor;
                        break;
                    case ControlState.Pressed:
                        backColor = StyleSheet.ControlPressedBackColor;
                        break;

                    default: //normal
                        {
                            if (_checkType == ControlCheckType.Focus && Focused)
                            {
                                backColor = StyleSheet.ControlCheckedBackColor;
                            }
                            else if (_checkType == ControlCheckType.Click && _checked)
                            {
                                backColor = StyleSheet.ControlCheckedBackColor;
                            }
                            else if (_checkType == ControlCheckType.Input && Checked)
                            {
                                backColor =  StyleSheet.ControlCheckedBackColor;
                            }
                            else
                            {
                                backColor = BackColor;
                            }
                        }
                        break;
                }
            }
            else
            {
                backColor = StyleSheet.ControlDisabledBackColor;
            }

            Color borderColor = HasBorder ? StyleSheet.ControlBorderColor : backColor;
            Color innerBorderColor = HasBorder ? StyleSheet.ControlInnerBorderColor : backColor;

            GraphicsHelper.DrawBackground(
                g,
                ClientRectangle,
                backColor,
                borderColor,
                innerBorderColor,
                _roundStyle,
                _radius,
                0,
                true,
                true,
                LinearGradientMode.Vertical);
            Rectangle textRect = new Rectangle(
                        2,
                        2,
                        Width - 4,
                        Height - 4); ;

            TextRenderer.DrawText(
                 g,
                 Text,
                 Font,
                 textRect,
                 ForeColor,
                 GraphicsHelper.GetTextFormatFlags(TextAlign, RightToLeft == RightToLeft.Yes));
        }


    }
}


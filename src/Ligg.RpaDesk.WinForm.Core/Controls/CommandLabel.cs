using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Controls
{
    public class CommandLabel : Label
    {
        public CommandLabel() : base()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
        }

        private Color _hoveringColor =StyleSheet.ControlHoveringBackColor;
        //private Color _hoveringColor = Color.FromArgb(131, 209, 255);
        public Color HoveringColor
        {
            get { return _hoveringColor; }
            set { _hoveringColor = value; }
        }

        //private Color _checkedColor = Color.FromArgb(255, 0, 0);
        private Color _checkedColor = StyleSheet.ControlCheckedBackColor;
        public Color CheckedColor
        {
            get { return _checkedColor; }
            set { _checkedColor = value; }
        }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (_checkType == ControlCheckType.Input & _checked != value) //changed
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

        protected override void OnLostFocus(EventArgs e)
        {
            //base.OnLostFocus(e);
            base.Invalidate();
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
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                if (_checkType == ControlCheckType.Click)
                    _checked = true;
                if (_checkType == ControlCheckType.Focus)
                    Focus();
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
        protected override void OnForeColorChanged(EventArgs e)
        {
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color textColor = ForeColor;
            if (Enabled)
            {
                switch (_controlState)
                {
                    case ControlState.Hovering:
                        textColor = _hoveringColor;
                        break;
                    case ControlState.Pressed:
                        textColor = StyleSheet.ControlPressedBackColor;
                        break;

                    default: //normal
                        {
                            if (_checkType == ControlCheckType.Focus && Focused)
                            {
                                textColor = _checkedColor;
                            }
                            else if (_checkType == ControlCheckType.Click && _checked)
                            {
                                textColor = _checkedColor;
                            }
                            else if (_checkType == ControlCheckType.Input && Checked)
                            {
                                textColor = _checkedColor;
                            }
                            else
                            {
                                textColor = ForeColor;
                            }
                        }
                        break;
                }
            }
            else
            {
                textColor = StyleSheet.ControlDisabledBackColor;
            }

            Rectangle textRect = new Rectangle(
                        0,
                        0,
                        Width,
                        Height);

            TextRenderer.DrawText(
                 g,
                 Text,
                 Font,
                 textRect,
                 textColor,
                 GraphicsHelper.GetTextFormatFlags(TextAlign, RightToLeft == RightToLeft.Yes));
        }

    }


}


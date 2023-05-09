using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ligg.Infrastructure.Extensions;
using Ligg.RpaDesk.WinForm.DataModels.Enums;
using Ligg.WinFormBase;

namespace Ligg.RpaDesk.WinForm.Controls
{
    public class ImageTextButton : Button
    {
        public ImageTextButton()
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
        public RoundStyle RoundStyle
        {
            get { return _roundStyle; }
            set
            {
                _roundStyle = value;
            }
        }

        //[DefaultValue(8)]
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


        private int _imagePositionOffset = 2;
        private int _imageSizeOffset = 2;
        private int _textPositionOffset = 2;
        public string Offset
        {
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    string str = value;
                    var arry = str.Split(',');
                    _imagePositionOffset = arry[0].IsPlusIntegerOrZero() ? Convert.ToInt16(arry[0]) : _imagePositionOffset;
                    if (arry.Length > 1)
                        _imageSizeOffset = arry[1].IsPlusIntegerOrZero() ? Convert.ToInt16(arry[1]) : _imageSizeOffset;
                    if (arry.Length > 2)
                        _textPositionOffset = arry[2].IsPlusIntegerOrZero() ? Convert.ToInt16(arry[2]) : _textPositionOffset;
                }
            }
        }

        public bool HasBorder { get; set; }


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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color backColor; 
            if (Enabled)
            {
                switch (ControlState)
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

            Color borderColor = HasBorder?  StyleSheet.ControlBorderColor: backColor;
            Color innerBorderColor = HasBorder ? StyleSheet.ControlInnerBorderColor:backColor;


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
            Rectangle imageRect;
            Rectangle textRect;

            CalculateRect(out imageRect, out textRect);
            if (Image != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImage(
                    Image,
                    imageRect,
                    0,
                    0,
                    Image.Width,
                    Image.Height,
                    GraphicsUnit.Pixel);
            }

            TextRenderer.DrawText(
                 g,
                 Text,
                 Font,
                 textRect,
                 ForeColor,
                 GraphicsHelper.GetTextFormatFlags(TextAlign, RightToLeft == RightToLeft.Yes));
        }

        private void CalculateRect(out Rectangle imageRect, out Rectangle textRect)
        {
            var imageWH = Width > Height ? Height - 2 * _imageSizeOffset : Width - 2 * _imageSizeOffset;
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            //if (Image == null)
            //{
            //    textRect = new Rectangle(
            //       _textPositionOffset,
            //       0,
            //       Width - _textPositionOffset,
            //       Height);
            //    return;
            //}
            //imageRect = new Rectangle(
            //            _imagePositionOffset,
            //            _imageSizeOffset,
            //            imageWH,
            //            imageWH);


            switch (TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    {
                        imageRect = new Rectangle(
                                    _imagePositionOffset,
                                    _imageSizeOffset,
                                    imageWH,
                                    imageWH);

                        textRect = new Rectangle(
                                   _textPositionOffset,
                                   0,
                                   Width - _textPositionOffset,
                                   Height);
                        break;
                    }

                case TextImageRelation.ImageAboveText:
                    {
                        imageRect = new Rectangle(
                                    _imageSizeOffset, 
                                    _imagePositionOffset,
                                    imageWH,
                                    imageWH);
                        textRect = new Rectangle(
                            0,
                            imageRect.Bottom + _textPositionOffset,
                            Width,
                            Height - imageRect.Bottom - _textPositionOffset);
                        break;
                    }

                case TextImageRelation.TextAboveImage:
                    {
                        imageRect = new Rectangle(
                                    _imageSizeOffset,
                                    Height- imageWH-2* _imagePositionOffset,
                                    imageWH,
                                    imageWH);
                        textRect = new Rectangle(
                        _textPositionOffset,
                        0,
                        Width - 2 * _textPositionOffset,
                        Height - imageWH - imageRect.Top - _textPositionOffset);
                        break;
                    }

                case TextImageRelation.ImageBeforeText:
                    {
                        imageRect = new Rectangle(
                        _imagePositionOffset,
                        _imageSizeOffset,
                        imageWH,
                        imageWH);

                        textRect = new Rectangle(
                            imageRect.Right + _textPositionOffset,
                            0,
                            Width - imageRect.Right - _textPositionOffset,
                            Height);
                        break;
                    }


                case TextImageRelation.TextBeforeImage:
                    {
                        imageRect = new Rectangle(
                        Width- imageWH-2* _imagePositionOffset,
                        _imageSizeOffset,
                        imageWH,
                        imageWH);

                        textRect = new Rectangle(
                         _textPositionOffset,
                        0,
                        Width - imageRect.Left - _textPositionOffset,
                        Height);
                        break;
                    }
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                imageRect.X = Width - imageRect.Right;
                textRect.X = Width - textRect.Right;
            }
        }

    }
}


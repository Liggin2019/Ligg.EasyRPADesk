using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Ligg.EasyWinApp.WinForm.DataModel.Enums;

namespace Ligg.EasyWinApp.WinForm.Controls
{
    public class Ellipse : Control
    {
        //#Constructors
        public Ellipse() : base()
        {
            SetStyles();
        }

        //#override
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Render(g);
        }


        //#private
        private void SetStyles()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.CacheText, true);
            SetStyle(ControlStyles.Opaque, false);
            UpdateStyles();
        }


        private void Render(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(ForeColor))
            {
                g.FillEllipse(
                    brush,
                    new RectangleF(new PointF(0, 0), new SizeF(Width, Height)));
            }
        }


    }
}

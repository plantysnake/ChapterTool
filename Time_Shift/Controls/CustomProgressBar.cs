using System.Drawing;
using System.Windows.Forms;

namespace ChapterTool.Controls
{
    class CustomProgressBar : ProgressBar
    {
        Color _barColor = Color.Blue;        // Color of progress meter



        public CustomProgressBar()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rec = e.ClipRectangle;
            SolidBrush brush = new SolidBrush(_barColor);

            rec.Width = (int)(rec.Width * ((double)Value / Maximum)) - 4;
            if (ProgressBarRenderer.IsSupported)
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, e.ClipRectangle);
            rec.Height = rec.Height - 4;
            e.Graphics.FillRectangle(brush, 2, 2, rec.Width, rec.Height);

            // Clean up.
            brush.Dispose();

        }
        public Color ProgressBarColor
        {
            get
            {
                return _barColor;
            }

            set
            {
                _barColor = value;

                // Invalidate the control to get a repaint.
                Invalidate();
            }
        }


    }
}

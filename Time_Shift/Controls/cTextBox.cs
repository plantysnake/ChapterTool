using System;
using System.Windows.Forms;

namespace ChapterTool
{
    public class cTextBox : TextBox
    {
        // Methods
        public cTextBox()
        {
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
        }
        private cTextBox otherRichTextBox;
        public cTextBox OthercTextBox
        {
            get { return otherRichTextBox; }
            set { otherRichTextBox = value; }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            try
            {
                if (e.Control && (e.KeyCode == Keys.A))
                {
                    SelectAll();
                }
                else if (e.Control && (e.KeyCode == Keys.C))
                {
                    Clipboard.SetText(this.SelectedText, TextDataFormat.UnicodeText);
                }
            }
            catch (Exception)
            {
            }
        }
    }



}

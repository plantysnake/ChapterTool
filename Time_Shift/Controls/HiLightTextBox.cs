using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChapterTool.Controls
{

    public struct Pattern
    {
        public Regex PatRegex { set; get; }
        public Color PatColor { set; get; }

        public Pattern(string pat, Color color)
        {
            PatRegex = new Regex(pat);
            PatColor = color;
        }
    }

    public partial class HiLightTextBox : RichTextBox
    {
        public HiLightTextBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            _patterns = new HashSet<Pattern>();
        }

        public Color OriginalColor { get; set; } = Color.Black;

        private HashSet<Pattern> _patterns;

        public void AddPattern(Pattern pattern)
        {
            _patterns.Add(pattern);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += HighLight;
            worker.RunWorkerAsync(Rtf);
            worker.RunWorkerCompleted += (sender, args) => Rtf = args.Result as string;
            base.OnTextChanged(e);
        }

        private void HighLight(object sender, DoWorkEventArgs e)
        {
            try
            {
                RichTextBox text = new RichTextBox
                {
                    Rtf = e.Argument as string,
                    SelectionStart = 0
                };
                text.SelectionLength = text.Text.Length;
                text.SelectionColor = OriginalColor;
                text.SelectionFont = new Font("Consolas", 9, FontStyle.Regular);

                foreach (var pattern in _patterns)
                {
                    foreach (Match m in pattern.PatRegex.Matches(text.Text))
                    {
                        text.SelectionStart = m.Index;
                        text.SelectionLength = m.Length;
                        text.SelectionColor = pattern.PatColor;
                    }
                }
                e.Result = text.Rtf;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}

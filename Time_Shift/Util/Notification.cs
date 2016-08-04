using System;
using System.Drawing;
using System.Windows.Forms;
using ChapterTool.Properties;

namespace ChapterTool.Util
{
    public static class Notification
    {
        public static DialogResult ShowError(string argMessage, Exception exception)
        {
            var ret = MessageBox.Show(caption: Resources.Message_ChapterTool_Error,
                text: $"{argMessage}:{Environment.NewLine}{exception.Message}"
#if DEBUG
                + $"{Environment.NewLine}{exception.StackTrace}", buttons: MessageBoxButtons.OK
#else
                ,buttons: MessageBoxButtons.YesNo
#endif
                , icon: MessageBoxIcon.Hand);
            if (ret != DialogResult.No) return ret;
            if (ShowInfo(Resources.Message_Stack) == DialogResult.Yes)
            {
                Clipboard.SetText(exception.StackTrace);
            }
            return ret;
        }

        public static DialogResult ShowInfo(string argMessage, MessageBoxButtons buttons = MessageBoxButtons.YesNo)
        {
            return MessageBox.Show(caption: Resources.Message_ChapterTool_Info,
                text: argMessage,
                buttons: buttons, icon: MessageBoxIcon.Information);
        }


        public static string InputBox(string caption, string prompt, string defaultText)
        {
            string localInputText = defaultText;
            return InputQuery(caption, prompt, ref localInputText) ? localInputText : "";
        }

        private static int MulDiv(int number, float numerator, int denominator)
        {
            return (int)(number * numerator / denominator);
        }

        private static Size ScaleSize(Size size, float width, float height)
        {
            size.Height = (int)(size.Height*height);
            size.Width = (int) (size.Width* width);
            return size;
        }

        private static bool InputQuery(string caption, string prompt, ref string value)
        {
            var form = new Form
            {
                AutoScaleMode = AutoScaleMode.Font,
                Font = SystemFonts.IconTitleFont
            };

            var dialogUnits = form.AutoScaleDimensions;

            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Text = caption;

            form.ClientSize = new Size(
                        MulDiv(180, dialogUnits.Width, 4),
                        MulDiv(63, dialogUnits.Height, 8));

            form.StartPosition = FormStartPosition.CenterScreen;

            var lblPrompt = new System.Windows.Forms.Label
            {
                Parent = form,
                AutoSize = true,
                Left = MulDiv(8, dialogUnits.Width, 4),
                Top = MulDiv(8, dialogUnits.Height, 8),
                Text = prompt
            };

            var edInput = new System.Windows.Forms.TextBox
            {
                Parent = form,
                Left = lblPrompt.Left,
                Top = MulDiv(19, dialogUnits.Height, 8),
                Width = MulDiv(164, dialogUnits.Width, 4),
                Text = value
            };
            edInput.SelectAll();


            int buttonTop = MulDiv(41, dialogUnits.Height, 8);
            //Command buttons should be 50x14 dlus
            Size buttonSize = ScaleSize(new Size(50, 14), dialogUnits.Width / 4, dialogUnits.Height / 8);

            System.Windows.Forms.Button bbOk = new System.Windows.Forms.Button
            {
                Parent = form,
                Text = "OK",
                DialogResult = DialogResult.OK
            };
            form.AcceptButton = bbOk;
            bbOk.Location = new Point(MulDiv(38, dialogUnits.Width, 4), buttonTop);
            bbOk.Size = buttonSize;

            System.Windows.Forms.Button bbCancel = new System.Windows.Forms.Button
            {
                Parent = form,
                Text = "Cancel",
                DialogResult = DialogResult.Cancel
            };
            form.CancelButton = bbCancel;
            bbCancel.Location = new Point(MulDiv(92, dialogUnits.Width, 4), buttonTop);
            bbCancel.Size = buttonSize;

            if (form.ShowDialog() == DialogResult.OK)
            {
                value = edInput.Text;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
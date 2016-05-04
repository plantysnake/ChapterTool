using System;
using System.Windows.Forms;
using ChapterTool.Properties;

namespace ChapterTool.Util
{
    public static class Notification
    {
        public static DialogResult ShowError(string argMessage, Exception exception)
        {
            return MessageBox.Show(caption: Resources.Message_ChapterTool_Error,
                text: $"{argMessage}:{Environment.NewLine}{exception.Message}",
                buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Hand);
        }

        public static DialogResult ShowInfo(string argMessage)
        {
            return MessageBox.Show(caption: Resources.Message_ChapterTool_Info,
                text: argMessage,
                buttons: MessageBoxButtons.YesNo,icon: MessageBoxIcon.Information);
        }
    }
}
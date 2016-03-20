using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChapterTool.Util
{
    public static class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr w, IntPtr l);
        //1 = normal (green);
        //2 = error (red);
        //3 = warning (yellow);
        //
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }

        //from http://www.sukitech.com/?p=1080
        //尋找視窗
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //將視窗移動到最上層
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int BCM_SETSHIELD = 0x0000160C;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", EntryPoint = "#680", CharSet = CharSet.Unicode)]
        public static extern bool IsUserAnAdmin();

        /// <summary>
        /// 为按钮设置UAC盾牌图标
        /// </summary>
        /// <param name="btn"></param>
        public static void SetShieldIcon(Button btn)
        {
            if (Environment.OSVersion.Version.Major < 6) return;
            btn.FlatStyle = FlatStyle.System;
            SendMessage(new HandleRef(btn, btn.Handle), BCM_SETSHIELD, IntPtr.Zero, IsUserAnAdmin() ? new IntPtr(0) : new IntPtr(1));
        }

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        /// <summary>
        /// 刷新桌面
        /// </summary>
        public static void RefreshNotify()
        {
            SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
        }
    }
}

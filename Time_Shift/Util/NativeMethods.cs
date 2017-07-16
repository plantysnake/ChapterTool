using System;
using System.Diagnostics;
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

        public enum DpiType
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI,
            MDT_RAW_DPI,
            MDT_DEFAULT = MDT_EFFECTIVE_DPI
        }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint([In] System.Drawing.Point pt, [In] uint dwFlags);

        [DllImport("shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        public static void GetDpi(this Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new System.Drawing.Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2);
            GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
        }

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

        [DllImport("Kernel32.dll", EntryPoint = "CreateHardLinkW", CharSet = CharSet.Unicode)]
        private static extern bool CreateHardLink(string lpFileName, string lpExistingFileName, IntPtr lpSecurityAttributes);

        public static bool CreateHardLink(string lpFileName, string lpExistingFileName)
        {
            return CreateHardLink(lpFileName, lpExistingFileName, IntPtr.Zero);
        }

        public static void CreateHardLinkCMD(string lpFileName, string lpExistingFileName)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "fsutil",
                    Arguments = $"hardlink create \"{lpFileName}\" \"{lpExistingFileName}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}

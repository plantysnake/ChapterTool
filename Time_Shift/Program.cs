using System;
using System.Windows.Forms;
using System.Linq;
using ChapterTool.Properties;
using Microsoft.Win32;

namespace ChapterTool
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var options = args.TakeWhile(item => item.StartsWith("--")).ToArray();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!IsSupportedRuntimeVersion())
            {
                var ret = Util.Notification.ShowInfo(Resources.Message_Need_Newer_dotNet);
                System.Diagnostics.Process.Start("http://dotnetsocial.cloudapp.net/GetDotnet?tfm=.NETFramework,Version=v4.6");
                if (ret == DialogResult.Yes) Util.RegistryStorage.Save("False", name: "DoVersionCheck");
            }

            if (args.Length == 0)
            {
                Application.Run(new Forms.Form1());
            }
            else
            {
                var argsFull = string.Join(" ", args.SkipWhile(item => item.StartsWith("--")));
                Application.Run(new Forms.Form1(argsFull));
            }
        }

        private static bool IsSupportedRuntimeVersion()
        {
            // https://msdn.microsoft.com/en-us/library/hh925568
            const int minSupportedRelease = 394802;
            if (Util.RegistryStorage.Load(name: "DoVersionCheck") == "False") return true;
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
            {
                if (key?.GetValue("Release") != null)
                {
                    var releaseKey = (int)key.GetValue("Release");
                    if (releaseKey >= minSupportedRelease) return true;
                }
            }
            return false;
        }
    }
}

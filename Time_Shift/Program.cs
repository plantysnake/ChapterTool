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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            CheckDotNetVersion();
            if (args.Length == 0)
            {
                Application.Run(new Forms.Form1());
            }
            else
            {
                var option = args.TakeWhile(item => item.StartsWith("--"));
                string argsFull = string.Join(" ", args.SkipWhile(item => item.StartsWith("--")));
                Application.Run(new Forms.Form1(argsFull));
            }
        }

        private static void CheckDotNetVersion()
        {
            var doCheck = Util.RegistryStorage.Load(name: "DoVersionCheck");
            if (doCheck == "False") return;
            int dotNetVersion;
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
            if (key == null) dotNetVersion = 0;
            else dotNetVersion = (int)(key.GetValue("Release"));
            //https://msdn.microsoft.com/en-us/library/hh925568
            if (dotNetVersion >= 394802) return;
            var ret = Util.Notification.ShowInfo(Resources.Message_Need_Newer_dotNet);
            if (ret == DialogResult.Yes)
            {
                Util.RegistryStorage.Save("False", name: "DoVersionCheck");
            }
        }
    }
}

using System;
using System.Windows.Forms;

namespace ChapterTool
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            if (args.Length == 0)
            {
                Application.Run(new Forms.Form1());
            }
            else
            {
                string argsFull = string.Join(" ", args);
                //argsFull = "\"" + argsFull + "\"";
                Application.Run(new Forms.Form1(argsFull));
            }
        }
    }
}

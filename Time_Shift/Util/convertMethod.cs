using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace ChapterTool
{
    static class convertMethod
    {
        //format a pts as hh:mm:ss.sss
        public static string time2string(int pts)
        {
            decimal total = pts / 45000M;
            return time2string(total);
        }
        public static string time2string(decimal second)
        {
            decimal secondPart = Math.Floor(second);
            decimal millisecondPart = Math.Round((second - secondPart) * 1000M);
            return time2string(new TimeSpan(0, 0, 0, (int)secondPart, (int)millisecondPart));
        }
        
        
        public static string time2string(TimeSpan temp)
        {
            return temp.Hours.ToString("00") + ":" +
                 temp.Minutes.ToString("00") + ":" +
                 temp.Seconds.ToString("00") + "." +
            temp.Milliseconds.ToString("000");
        }

        static Regex RTimeFormat = new Regex(@"(?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)\.(?<Millisecond>\d{3})");
        public static TimeSpan string2Time(string input)
        {
            if (string.IsNullOrEmpty(input)) { return TimeSpan.Zero; }
            var        temp = RTimeFormat.Match(input);
            int        Hour = int.Parse(temp.Groups["Hour"].Value);
            int      Minute = int.Parse(temp.Groups["Minute"].Value);
            int      Second = int.Parse(temp.Groups["Second"].Value);
            int Millisecond = int.Parse(temp.Groups["Millisecond"].Value);
            return new TimeSpan(0, Hour, Minute, Second, Millisecond);
        }

        public static TimeSpan pts2Time(int pts)
        {
            decimal total = pts / 45000M;
            decimal secondPart = Math.Floor(total);
            decimal millisecondPart = Math.Round((total - secondPart) * 1000M);
            return new TimeSpan(0, 0, 0, (int)secondPart, (int)millisecondPart);
        }
        static Regex Rpos = new Regex(@"{X=(?<x>.+),Y=(?<y>.+)}");
        public static Point string2point(string input)
        {
            if (string.IsNullOrEmpty(input)) { return new Point(-32000, -32000); }
            var temp = Rpos.Match(input);
            int x = int.Parse(temp.Groups["x"].Value);
            int y = int.Parse(temp.Groups["y"].Value);
            return new Point(x, y);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        //1 = normal (green); 
        //2 = error (red); 
        //3 = warning (yellow).
        //
        public static void SetState(this ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}

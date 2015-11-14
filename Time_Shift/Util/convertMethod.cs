// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// ****************************************************************************
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Drawing;
using ChapterTool.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace ChapterTool.Util
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
            return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                                  temp.Hours  , temp.Minutes,
                                  temp.Seconds, temp.Milliseconds);
        }

        public static Regex RTimeFormat = new Regex(@"(?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)\.(?<Millisecond>\d{3})");

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
        public static Point string2point(string input)
        {
            Regex Rpos = new Regex(@"{X=(?<x>.+),Y=(?<y>.+)}");
            if (string.IsNullOrEmpty(input)) { return new Point(-32000, -32000); }
            var temp = Rpos.Match(input);
            int x = int.Parse(temp.Groups["x"].Value);
            int y = int.Parse(temp.Groups["y"].Value);
            return new Point(x, y);
        }

        public static int convertFR2Index(double frame)
        {
            decimal[] FrameRate = { 0M, 24000M / 1001, 24000M / 1000,
                                        25000M / 1000, 30000M / 1001,
                                        50000M / 1000, 60000M / 1001 };
            var result = Enumerable.Range(0, 7).Where(index => (Math.Abs(frame - (double)FrameRate[index])) < 1e-5);
            return result.First();
        }

        public static string GetUTF8String(byte[] buffer)
        {
            if (buffer == null) return null;
            if (buffer.Length <= 3) return Encoding.UTF8.GetString(buffer);
            byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

            if (buffer[0] == bomBuffer[0]
             && buffer[1] == bomBuffer[1]
             && buffer[2] == bomBuffer[2])
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        public static List<ChapterInfo>  PraseXML(XmlDocument doc)
        {
            List<ChapterInfo> BUFFER = new List<ChapterInfo>();
            XmlElement root = doc.DocumentElement;
            XmlNodeList EditionEntrys = root.ChildNodes;//获取各个章节的入口
            foreach (XmlNode EditionEntry in EditionEntrys)
            {
                XmlNodeList EditionEntryChildNodes = (EditionEntry as XmlElement).ChildNodes;//获取当前章节中的所有子节点
                ChapterInfo buff = new ChapterInfo();
                buff.SourceType = "XML";
                int j = 0;
                foreach (XmlNode EditionEntryChildNode in EditionEntryChildNodes)
                {
                    if (EditionEntryChildNode.Name != "ChapterAtom") { continue; }
                    XmlNodeList ChapterAtomChildNodes = (EditionEntryChildNode as XmlElement).ChildNodes;//获取Atom中的所有子节点
                    Chapter temp  = new Chapter();
                    Chapter temp2 = new Chapter();
                    foreach (XmlNode ChapterAtomChildNode in ChapterAtomChildNodes)
                    {
                        switch (ChapterAtomChildNode.Name)
                        {
                            case "ChapterTimeStart":
                                temp.Time = string2Time(RTimeFormat.Match(ChapterAtomChildNode.InnerText).Value);
                                break;
                            case "ChapterTimeEnd":
                                temp2.Time = string2Time(RTimeFormat.Match(ChapterAtomChildNode.InnerText).Value);
                                break;
                            case "ChapterDisplay":
                                temp.Name  = (ChapterAtomChildNode as XmlElement).ChildNodes.Item(0).InnerText;
                                temp2.Name = temp.Name;
                                break;
                        }
                    }
                    temp.Number = ++j;
                    buff.Chapters.Add(temp);
                    if (temp2.Time.TotalSeconds > 1e-5)
                    {
                        temp2.Number = j;
                        buff.Chapters.Add(temp2);
                    }
                }
                for (int i = 0; i < buff.Chapters.Count - 1; i++)
                {
                    if (buff.Chapters[i].Time == buff.Chapters[i+1].Time)
                    {
                        buff.Chapters.Remove(buff.Chapters[i--]);
                    }
                }
                BUFFER.Add(buff);
            }
            return BUFFER;
        }

        private static string colorProfile = "color-config.json";
        public static void saveColor(List<Color> ColorList)
        {
            StringBuilder json = new StringBuilder("[");
            foreach (var item in ColorList)
            {
                json.AppendFormat("\"#{0:X2}{1:X2}{2:X2}\",", item.R, item.G, item.B);
            }
            json[json.Length - 1] = ']';
            File.WriteAllText(colorProfile, json.ToString());
        }

        public static void loadColor(Form1 window)
        {
            if (File.Exists(colorProfile))
            {
                string json = File.ReadAllText(colorProfile);
                Regex Rcolor = new Regex("\"(?<hex>.+?)\"");
                var matchesOfJson = Rcolor.Matches(json);
                if (matchesOfJson.Count < 6) { return; }
                window.BackChange     = ColorTranslator.FromHtml(matchesOfJson[0].Groups["hex"].Value);
                window.TextBack       = ColorTranslator.FromHtml(matchesOfJson[1].Groups["hex"].Value);
                window.MouseOverColor = ColorTranslator.FromHtml(matchesOfJson[2].Groups["hex"].Value);
                window.MouseDownColor = ColorTranslator.FromHtml(matchesOfJson[3].Groups["hex"].Value);
                window.BordBackColor  = ColorTranslator.FromHtml(matchesOfJson[4].Groups["hex"].Value);
                window.TextFrontColor = ColorTranslator.FromHtml(matchesOfJson[5].Groups["hex"].Value);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        //1 = normal (green);
        //2 = error (red);
        //3 = warning (yellow).
        //
        public static void SetState(this System.Windows.Forms.ProgressBar pBar, int state)
        {
            SendMessage(pBar.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }

        //from http://www.sukitech.com/?p=1080
        //尋找視窗
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //將視窗移動到最上層
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

    }
}

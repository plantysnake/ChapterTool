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
using System.Security.Principal;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChapterTool.Util
{
    public static class ConvertMethod
    {
        //format a pts as hh:mm:ss.sss
        public static string Time2String(int pts) => Time2String(pts / 45000M);

        private static string Time2String(decimal second)
        {
            decimal secondPart = Math.Floor(second);
            decimal millisecondPart = Math.Round((second - secondPart) * 1000M);
            return Time2String(new TimeSpan(0, 0, 0, (int)secondPart, (int)millisecondPart));
        }

        public static string Time2String(this TimeSpan time) => $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";

        public static string Time2String(this Chapter item, TimeSpan offset, bool mul1K1)
        {
            return mul1K1 ? Time2String((decimal) (item.Time + offset).TotalSeconds*1.001M) : Time2String(item.Time + offset);
        }

        public static readonly Regex RLineOne    = new Regex(@"CHAPTER\d+=\d+:\d+:\d+\.\d+");
        public static readonly Regex RLineTwo    = new Regex(@"CHAPTER\d+NAME=(?<chapterName>.*)");
        public static readonly Regex RTimeFormat = new Regex(@"(?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)\.(?<Millisecond>\d{3})");

        public static TimeSpan ToTimeSpan(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return TimeSpan.Zero; }
            var        temp = RTimeFormat.Match(input);
            int        hour = int.Parse(temp.Groups["Hour"].Value);
            int      minute = int.Parse(temp.Groups["Minute"].Value);
            int      second = int.Parse(temp.Groups["Second"].Value);
            int millisecond = int.Parse(temp.Groups["Millisecond"].Value);
            return new TimeSpan(0, hour, minute, second, millisecond);
        }

        public static TimeSpan Pts2Time(int pts)
        {
            decimal total = pts / 45000M;
            decimal secondPart = Math.Floor(total);
            decimal millisecondPart = Math.Round((total - secondPart) * 1000M);
            return new TimeSpan(0, 0, 0, (int)secondPart, (int)millisecondPart);
        }

        public static TimeSpan OffsetCal(string line)
        {
            if (RLineOne.IsMatch(line))
            {
                return RTimeFormat.Match(line).Value.ToTimeSpan();
            }
            throw new Exception($"ERROR: {line} <-该行与时间行格式不匹配");
        }

        public static Point String2Point(string input)
        {
            var rpos = new Regex(@"{X=(?<x>.+),Y=(?<y>.+)}");
            if (string.IsNullOrWhiteSpace(input)) { return new Point(-32000, -32000); }
            var temp = rpos.Match(input);
            int x = int.Parse(temp.Groups["x"].Value);
            int y = int.Parse(temp.Groups["y"].Value);
            return new Point(x, y);
        }

        private static readonly decimal[] FrameRate = { 0M, 24000M / 1001, 24M, 25M, 30000M / 1001, 50M, 60000M / 1001 };

        public static int ConvertFr2Index(double frame) => Enumerable.Range(0, 7).First(index => Math.Abs(frame - (double)FrameRate[index]) < 1e-5);

        public static string GetUTF8String(byte[] buffer)
        {
            if (buffer == null) return null;
            if (buffer.Length <= 3) return Encoding.UTF8.GetString(buffer);
            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        public static int GetAccuracy(TimeSpan time, decimal fps, decimal accuracy, bool round)
        {
            var frams = (decimal)time.TotalMilliseconds * fps / 1000M;
            var answer = round ? Math.Round(frams, MidpointRounding.AwayFromZero) : frams;
            return Math.Abs(frams - answer) < accuracy ? 1 : 0;
        }

        public static IEnumerable<ChapterInfo> PraseXml(XmlDocument doc)
        {
            XmlElement root = doc.DocumentElement;
            if (root == null)
            {
                throw new ArgumentException("Empty Xml file");
            }
            if (root.Name != "Chapters")
            {
                throw new Exception($"Invalid Xml file.\nroot node Name: {root.Name}");
            }

            foreach (XmlNode editionEntry in root.ChildNodes)//Get Entrance for each chapter
            {
                if (editionEntry.NodeType == XmlNodeType.Comment) continue;
                if (editionEntry.Name != "EditionEntry")
                {
                    throw new Exception($"Invalid Xml file.\nEntry Name: {editionEntry.Name}");
                }
                ChapterInfo buff = new ChapterInfo {SourceType = "XML", Tag = doc, TagType = doc.GetType()};
                int index = 0;
                foreach (XmlNode editionEntryChildNode in ((XmlElement)editionEntry).ChildNodes)//Get all the child nodes in current chapter
                {
                    if (editionEntryChildNode.Name != "ChapterAtom") continue;
                    buff.Chapters.AddRange(PraseChapterAtom(editionEntryChildNode, ++index));
                }

                for (int i = 0; i < buff.Chapters.Count - 1; i++)
                {
                    if (buff.Chapters[i].Time == buff.Chapters[i+1].Time)
                    {
                        buff.Chapters.Remove(buff.Chapters[i--]);
                    }
                }
                //buff.Chapters = buff.Chapters.Distinct().ToList();
                yield return buff;
            }
        }

        private static IEnumerable<Chapter> PraseChapterAtom(XmlNode chapterAtom, int index)
        {
            Chapter startChapter = new Chapter {Number = index};
            Chapter endChapter   = new Chapter {Number = index};
            var innerChapterAtom = new List<Chapter>();
            foreach (XmlNode chapterAtomChildNode in ((XmlElement)chapterAtom).ChildNodes) //Get detail info for current chapter node
            {
                switch (chapterAtomChildNode.Name)
                {
                    case "ChapterTimeStart":
                        startChapter.Time = RTimeFormat.Match(chapterAtomChildNode.InnerText).Value.ToTimeSpan();
                        break;
                    case "ChapterTimeEnd":
                        endChapter.Time = RTimeFormat.Match(chapterAtomChildNode.InnerText).Value.ToTimeSpan();
                        break;
                    case "ChapterDisplay":
                        try
                        {
                            startChapter.Name = ((XmlElement)chapterAtomChildNode).ChildNodes.Cast<XmlNode>().First(node => node.Name == "ChapterString").InnerText;
                        }
                        catch
                        {
                            startChapter.Name = string.Empty;
                        }
                        endChapter.Name   = startChapter.Name;
                        break;
                    case "ChapterAtom"://Handling sub chapters.
                        innerChapterAtom.AddRange(PraseChapterAtom(chapterAtomChildNode, index));
                        break;
                }
            }
            yield return startChapter;

            foreach (var chapter in innerChapterAtom)
            {
                yield return chapter;
            }

            if (endChapter.Time.TotalSeconds > startChapter.Time.TotalSeconds)
            {
                yield return endChapter;
            }
        }

        private const string ColorProfile = "color-config.json";

        public static void SaveColor(this List<Color> colorList)
        {
            var json = new StringBuilder("[");
            foreach (var color in colorList)
            {
                json.Append($"\"#{color.R:X2}{color.G:X2}{color.B:X2}\",");
            }
            //colorList.ForEach(item => json.AppendFormat($"\"#{item.R:X2}{item.G:X2}{item.B:X2}\","));
            json[json.Length - 1] = ']';
            File.WriteAllText(ColorProfile, json.ToString());
        }

        public static void LoadColor(Form1 window)
        {
            if (!File.Exists(ColorProfile)) return;
            string json = File.ReadAllText(ColorProfile);
            Regex rcolor = new Regex("\"(?<hex>.+?)\"");
            var matchesOfJson = rcolor.Matches(json);
            if (matchesOfJson.Count < 6)  return;
            window.BackChange     = ColorTranslator.FromHtml(matchesOfJson[0].Groups["hex"].Value);
            window.TextBack       = ColorTranslator.FromHtml(matchesOfJson[1].Groups["hex"].Value);
            window.MouseOverColor = ColorTranslator.FromHtml(matchesOfJson[2].Groups["hex"].Value);
            window.MouseDownColor = ColorTranslator.FromHtml(matchesOfJson[3].Groups["hex"].Value);
            window.BordBackColor  = ColorTranslator.FromHtml(matchesOfJson[4].Groups["hex"].Value);
            window.TextFrontColor = ColorTranslator.FromHtml(matchesOfJson[5].Groups["hex"].Value);
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (identity == null) return false;
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool RunAsAdministrator()
        {
            if (IsAdministrator()) return true;
            if (!RunElevated(Application.ExecutablePath)) return false;
            Environment.Exit(0);
            return true;
        }

        private static bool RunElevated(string fileName)
        {
            System.Diagnostics.ProcessStartInfo processInfo = new System.Diagnostics.ProcessStartInfo
            {
                Verb = "runas",
                FileName = fileName
            };
            try
            {
                System.Diagnostics.Process.Start(processInfo);
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
            return false;
        }



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
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //將視窗移動到最上層
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

    }
}

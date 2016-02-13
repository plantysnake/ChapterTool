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
            if (string.IsNullOrEmpty(input)) { return TimeSpan.Zero; }
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
            if (string.IsNullOrEmpty(input)) { return new Point(-32000, -32000); }
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
                    throw new Exception($"Invalid Xml file.\nroot Entry Name: {editionEntry.Name}");
                }
                ChapterInfo buff = new ChapterInfo {SourceType = "XML"};
                int index = 0;
                foreach (XmlNode editionEntryChildNode in ((XmlElement)editionEntry).ChildNodes)//Get all the child nodes in current chapter
                {
                    if (editionEntryChildNode.Name != "ChapterAtom") continue;
                    ++index;
                    var chapterAtom = PraseChapterAtom(editionEntryChildNode);
                    foreach (var chapter in chapterAtom)
                    {
                        chapter.Number = index;
                        buff.Chapters.Add(chapter);
                    }
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

        private static IEnumerable<Chapter> PraseChapterAtom(XmlNode chapterAtom)
        {
            Chapter startChapter = new Chapter();
            Chapter endChapter   = new Chapter();
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
                        startChapter.Name = ((XmlElement) chapterAtomChildNode).ChildNodes.Item(0)?.InnerText;
                        endChapter.Name = startChapter.Name;
                        break;
                    case "ChapterAtom"://Handling sub chapters.
                        innerChapterAtom.AddRange(PraseChapterAtom(chapterAtomChildNode));
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

        private enum NextState
        {
            NsStart,
            NsTitle,
            NsNewTrack,
            NsTrack,
            NsError,
            NsFin
        }

        public static ChapterInfo PraseCue(string context)
        {
            var line = context.Split('\n');
            var cue = new ChapterInfo {SourceType = "CUE"};
            Regex rTitle = new Regex(@"TITLE\s+\""(.+)\""");
            Regex rTrack = new Regex(@"TRACK (\d+) AUDIO");
            Regex rPerformer = new Regex(@"PERFORMER\s+\""(.+)\""");
            Regex rTime = new Regex(@"INDEX (?<index>\d+) (?<M>\d{2}):(?<S>\d{2}):(?<m>\d{2})");
            NextState nxState = NextState.NsStart;
            Chapter beginChapter = null;

            foreach (var l in line)
            {
                switch (nxState)
                {
                    case NextState.NsStart:
                        var r = rTitle.Match(l);
                        if (r.Success)
                        {
                            nxState = NextState.NsNewTrack;
                            cue.Title = r.Groups[0].Value;
                        }
                        break;
                    case NextState.NsNewTrack:
                        var tt = rTrack.Match(l);
                        if (tt.Success)
                        {
                            beginChapter = new Chapter {Number = int.Parse(tt.Groups[1].Value)};
                            nxState = NextState.NsTitle;
                        }
                        break;
                    case NextState.NsTitle:
                        var rr = rTitle.Match(l);
                        if (rr.Success)
                        {
                            beginChapter.Name = rr.Groups[1].Value;
                            nxState = NextState.NsTrack;
                        }
                        break;
                    case NextState.NsTrack:
                        if (string.IsNullOrEmpty(l))
                        {
                            nxState = NextState.NsFin;
                            break;
                        }
                        var p = rPerformer.Match(l);
                        var t = rTime.Match(l);
                        var state = (1 << (p.Success ? 3 : 2)) | (1 << (t.Success ? 1 : 0));
                        switch (state)
                        {
                            case (1 << 2 | 1 << 0):
                                //nothing find
                                break;
                            case (1 << 2 | 1 << 1):
                                var trackIndex = int.Parse(t.Groups["index"].Value);
                                switch (trackIndex)
                                {
                                    case 0:
                                        // last track's end
                                        break;
                                    case 1:
                                        beginChapter.Time = new TimeSpan(0, 0, int.Parse(t.Groups["M"].Value),
                                               int.Parse(t.Groups["S"].Value), int.Parse(t.Groups["m"].Value)*10);
                                        cue.Chapters.Add(beginChapter);
                                        nxState = NextState.NsNewTrack;
                                        break;
                                    default:
                                        nxState = NextState.NsError;
                                        break;
                                }
                                break;
                            case (1 << 3 | 1 << 0):
                                beginChapter.Name += $" [{p.Groups[1].Value}]";
                                break;
                            case (1 << 3 | 1 << 1):
                                nxState = NextState.NsError;
                                break;
                            default:
                                nxState = NextState.NsError;
                                break;
                        }
                        break;
                    case NextState.NsError:
                        throw new Exception("Unable to Prase this cue");
                    case NextState.NsFin:
                        goto EXIT_1;
                    default:
                        nxState = NextState.NsError;
                        break;
                }
            }
            EXIT_1:
            cue.Duration = cue.Chapters.Last().Time;
            return cue;
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

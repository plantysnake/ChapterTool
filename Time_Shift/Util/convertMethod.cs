// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
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
using System.Text;
using System.Linq;
using System.Drawing;
using ChapterTool.Forms;
using System.Windows.Forms;
using System.Security.Principal;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace ChapterTool.Util
{
    public static class ConvertMethod
    {
        /// <summary>
        /// 将TimeSpan对象转换为 hh:mm:ss.sss 形式的字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string Time2String(this TimeSpan time) => $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds:D3}";

        /// <summary>
        /// 将给定的章节点时间以平移、修正信息修正后转换为 hh:mm:ss.sss 形式的字符串
        /// </summary>
        /// <param name="item">章节点</param>
        /// <param name="info">章节信息</param>
        /// <returns></returns>
        public static string Time2String(this Chapter item, ChapterInfo info)
        {
            return info.Mul1K1 ? new TimeSpan( (long) Math.Round((decimal) (item.Time + info.Offset).TotalSeconds*1.001M*TimeSpan.TicksPerSecond)).Time2String() : Time2String(item.Time + info.Offset);
        }

        public static readonly Regex RTimeFormat = new Regex(@"(?<Hour>\d+)\s*:\s*(?<Minute>\d+)\s*:\s*(?<Second>\d+)\s*[\.,]\s*(?<Millisecond>\d{3})");

        /// <summary>
        /// 将符合 hh:mm:ss.sss 形式的字符串转换为TimeSpan对象
        /// </summary>
        /// <param name="input">时间字符串</param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return TimeSpan.Zero;
            var        timeMatch = RTimeFormat.Match(input);
            if (!timeMatch.Success) return TimeSpan.Zero;
            int        hour = int.Parse(timeMatch.Groups["Hour"].Value);
            int      minute = int.Parse(timeMatch.Groups["Minute"].Value);
            int      second = int.Parse(timeMatch.Groups["Second"].Value);
            int millisecond = int.Parse(timeMatch.Groups["Millisecond"].Value);
            return new TimeSpan(0, hour, minute, second, millisecond);
        }

        /// <summary>
        /// 将{X=x_1,Y=y_1}格式的字符串转换为Point对象
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Point String2Point(string input)
        {
            var rpos = new Regex(@"{X=(?<x>.+),Y=(?<y>.+)}");
            var temp = rpos.Match(input);
            if (string.IsNullOrWhiteSpace(input) || !temp.Success) return new Point(-32000, -32000);
            int x = int.Parse(temp.Groups["x"].Value);
            int y = int.Parse(temp.Groups["y"].Value);
            return new Point(x, y);
        }

        private static readonly decimal[] FrameRate = { 0M, 24000M / 1001, 24M, 25M, 30000M / 1001, 50M, 60000M / 1001 };

        /// <summary>
        /// 根据给定的帧率返回它在FrameRate表中的序号
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static int ConvertFr2Index(double frame) => Enumerable.Range(0, 7).First(index => Math.Abs(frame - (double)FrameRate[index]) < 1e-5);

        /// <summary>
        /// 在无行数变动时直接修改各行的数据
        /// 提高刷新效率
        /// </summary>
        /// <param name="row">要更改的行</param>
        /// <param name="index">对应的时间戳</param>
        /// <param name="info">章节信息</param>
        /// <param name="autoGenName">是否使用自动生成的章节名</param>
        public static void EditRow(this DataGridViewRow row, int index, ChapterInfo info, bool autoGenName)
        {
            var item = info.Chapters[index];
            row.Tag  = item;
            row.DefaultCellStyle.BackColor = row.Index % 2 == 0
                ? Color.FromArgb(0x92, 0xAA, 0xF3)
                : Color.FromArgb(0xF3, 0xF7, 0xF7);
            row.Cells[0].Value = $"{item.Number:D2}";
            row.Cells[1].Value = item.Time2String(info);
            row.Cells[2].Value = autoGenName ? ChapterName.Get(row.Index + 1) : item.Name;
            row.Cells[3].Value = item.FramsInfo;
        }

        /// <summary>
        /// 读取带或不带BOM头的UTF-8文本
        /// </summary>
        /// <param name="buffer">UTF-8文本的字节串</param>
        /// <returns></returns>
        public static string GetUTF8String(this byte[] buffer)
        {
            if (buffer == null) return null;
            if (buffer.Length <= 3) return Encoding.UTF8.GetString(buffer);
            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }
            return Encoding.UTF8.GetString(buffer);
        }

        private const string ColorProfile = "color-config.json";

        /// <summary>
        /// 假装生成一个json格式的界面颜色配置文件
        /// </summary>
        /// <param name="colorList"></param>
        public static void SaveColor(this List<Color> colorList)
        {
            var json = new StringBuilder("[");
            colorList.ForEach(item => json.AppendFormat($"\"#{item.R:X2}{item.G:X2}{item.B:X2}\","));
            json[json.Length - 1] = ']';
            var path = $"{Path.GetDirectoryName(Application.ExecutablePath)}\\{ColorProfile}";
            File.WriteAllText(path, json.ToString());
        }

        /// <summary>
        /// 读取文本中的颜色数据并应用于窗体
        /// </summary>
        /// <param name="window"></param>
        public static void LoadColor(this Form1 window)
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
    }
}

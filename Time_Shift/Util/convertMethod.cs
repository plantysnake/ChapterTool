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
using System.Diagnostics;
using System.Security.Principal;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;

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

        public static string ToCueTimeStamp(this TimeSpan input)
        {
            int frames = (int) Math.Round(input.Milliseconds*75/1000F);
            return $"{input.Hours*60 + input.Minutes:D2}:{input.Seconds:D2}:{frames:D2}";
        }

        /// <summary>
        /// 将{X=x_1,Y=y_1}格式的字符串转换为Point对象
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Point String2Point(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return new Point(-32000, -32000);
            var rpos = new Regex(@"{X=(?<x>.+),Y=(?<y>.+)}");
            var result = rpos.Match(input);
            if (!result.Success) return new Point(-32000, -32000);
            int x = int.Parse(result.Groups["x"].Value);
            int y = int.Parse(result.Groups["y"].Value);
            return new Point(x, y);
        }

        /// <summary>
        /// 根据给定的帧率返回它在FrameRate表中的序号
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static int ConvertFr2Index(double frame)
            => Enumerable.Range(0, 7).First(index => Math.Abs(frame - (double)MplsData.FrameRate[index]) < 1e-5);

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
            var path = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\{ColorProfile}";
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
            if (NativeMethods.IsUserAnAdmin()) return true;
            if (!RunElevated(System.Reflection.Assembly.GetExecutingAssembly().Location)) return false;
            Environment.Exit(0);
            return true;
        }

        private static bool RunElevated(string fileName)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                Verb = "runas",
                FileName = fileName
            };
            try
            {
                Process.Start(processInfo);
                return true;
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //Do nothing. Probably the user canceled the UAC window
            }
            return false;
        }
    }

    public static class RegistryStorage
    {
        public static string Load(string subKey = @"Software\ChapterTool", string name = "SavingPath")
        {
            string path = string.Empty;
            // HKCU_CURRENT_USER\Software\
            var registryKey = Registry.CurrentUser.OpenSubKey(subKey);
            if (registryKey == null) return path;
            path = (string)registryKey.GetValue(name);
            registryKey.Close();
            return path;
        }

        public static void Save(string value, string subKey = @"Software\ChapterTool", string name = "SavingPath")
        {
            // HKCU_CURRENT_USER\Software\
            var registryKey = Registry.CurrentUser.CreateSubKey(subKey);
            registryKey?.SetValue(name, value);
            registryKey?.Close();
        }

        /// <summary>
        /// 创建文件关联
        /// </summary>
        /// <param name="programFile">应用程序文件的完整路径("C:\abc\def.exe")</param>
        /// <param name="extension">文件扩展名（例如 ".txt"）</param>
        /// <param name="typeName">文件类型名称</param>
        /// <param name="project">指向的文件打开方式</param>
        /// <param name="argument">附加参数（不包括"%1"）</param>
        public static void SetOpenMethod(string programFile, string extension, string typeName, string project, string argument = "")
        {
            Registry.ClassesRoot.CreateSubKey(extension)?.SetValue(typeName, project, RegistryValueKind.String);

            RegistryKey subKey = Registry.ClassesRoot.CreateSubKey(project);
            subKey = subKey?.CreateSubKey("shell");
            subKey = subKey?.CreateSubKey("open");
            subKey = subKey?.CreateSubKey("command");
            subKey?.SetValue("", $@"""{programFile}"" ""%1"" {argument}", RegistryValueKind.ExpandString);
            subKey?.Dispose();
            NativeMethods.RefreshNotify();
        }

        public static int RegistryAddCount(string subKey, string name, int delta = 1)
        {
            var countS = Load(subKey, name);
            int count = string.IsNullOrEmpty(countS) ? 0 : int.Parse(countS);
            count += delta;
            Save(count.ToString(), subKey, name);
            return count - delta;
        }
    }
}

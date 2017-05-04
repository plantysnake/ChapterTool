// ****************************************************************************
//
// Copyright (C) 2014-2016 TautCony (TautCony@vcb-s.com)
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
using System.Net;
using ChapterTool.Forms;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ChapterTool.Util
{
    public static class Updater
    {
        private static void OnResponse(IAsyncResult ar)
        {
            var versionRegex = new Regex(@"<meta\s+name\s*=\s*'ChapterTool'\s+content\s*=\s*'(\d+\.\d+\.\d+\.\d+)'\s*>", RegexOptions.Compiled);
            var baseUrlRegex = new Regex(@"<meta\s+name\s*=\s*'BaseUrl'\s+content\s*=\s*'(.+)'\s*>", RegexOptions.Compiled);
            var webRequest   = (WebRequest)ar.AsyncState;
            Stream responseStream;
            try
            {
                responseStream = webRequest.EndGetResponse(ar).GetResponseStream();
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("检查更新失败, 错误信息:{0}{1}{0}请联系TC以了解详情",
                    Environment.NewLine, exception.Message), @"Update Check Fail");
                responseStream = null;
            }
            if (responseStream == null) return;

            var streamReader = new StreamReader(responseStream);
            var context      = streamReader.ReadToEnd();
            var result       = versionRegex.Match(context);
            var urlResult    = baseUrlRegex.Match(context);
            if (!result.Success || !result.Success) return;

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var remoteVersion  = Version.Parse(result.Groups[1].Value);
            if (currentVersion >= remoteVersion)
            {
                MessageBox.Show($"v{currentVersion} 已是最新版", @"As Expected");
                return;
            }
            var dialogResult = MessageBox.Show(caption: @"Wow! Such Impressive", text: $"新车已发车 v{remoteVersion}，上车!",
                                               buttons: MessageBoxButtons.YesNo, icon: MessageBoxIcon.Asterisk);
            if (dialogResult != DialogResult.Yes) return;
            var formUpdater = new FormUpdater(Assembly.GetExecutingAssembly().Location, remoteVersion, urlResult.Groups[1].Value);
            formUpdater.ShowDialog();
        }

        public static void CheckUpdate()
        {
            if (!IsConnectInternet()) return;

            var webRequest = (HttpWebRequest)WebRequest.Create("http://tautcony.github.io/tcupdate.html");
            #if DEBUG
            webRequest                = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:4000/tcupdate.html");
            #endif
            webRequest.UserAgent      = $"{Environment.UserName}({Environment.OSVersion}) / {Assembly.GetExecutingAssembly().GetName().FullName}";
            webRequest.Method         = "GET";
            webRequest.Credentials    = CredentialCache.DefaultCredentials;
            webRequest.BeginGetResponse(OnResponse, webRequest);
        }

        public static bool CheckUpdateWeekly(string program)
        {
            var reg = RegistryStorage.Load(@"Software\" + program, "LastCheck");
            if (string.IsNullOrEmpty(reg))
            {
                RegistryStorage.Save(DateTime.Now.ToString(CultureInfo.InvariantCulture), @"Software\" + program, "LastCheck");
                return false;
            }
            var lastCheckTime = DateTime.Parse(reg);
            if (DateTime.Now - lastCheckTime > new TimeSpan(7, 0, 0, 0))
            {
                CheckUpdate();
                RegistryStorage.Save(DateTime.Now.ToString(CultureInfo.InvariantCulture), @"Software\" + program, "LastCheck");
                return true;
            }
            return false;
        }

        private static bool IsConnectInternet()
        {
            return InternetGetConnectedState(0, 0);
        }

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(int description, int reservedValue);
    }
}
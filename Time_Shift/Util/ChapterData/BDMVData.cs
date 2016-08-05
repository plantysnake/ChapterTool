using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChapterTool.Util.ChapterData
{
    public static class BDMVData
    {
        public delegate void LogEventHandler(string message);

        public static event LogEventHandler OnLog;

        private static readonly Regex RDiskInfo = new Regex(@"(?<idx>\d)\) (?<mpls>\d+\.mpls), (?:(?:(?<dur>\d+:\d+:\d+)[\n\s\b]*(?<fn>.+\.m2ts))|(?:(?<fn2>.+\.m2ts), (?<dur2>\d+:\d+:\d+)))");

        public static List<ChapterInfo> GetChapter(string location)
        {
            var list = new List<ChapterInfo>();
            string path = Path.Combine(Path.Combine(location, "BDMV"), "PLAYLIST");
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException("Blu-Ray disc structure not found.");
            }
            var eac3toPath = RegistryStorage.Load(name: "eac3toPath");
            if (string.IsNullOrEmpty(eac3toPath) || !File.Exists(eac3toPath))
            {
                eac3toPath = Notification.InputBox("请输入eac3to的地址", "注意不要带上多余的引号", "C:\\eac3to\\eac3to.exe");
                if (string.IsNullOrEmpty(eac3toPath)) return list;
                RegistryStorage.Save(name: "eac3toPath",value: eac3toPath);
            }
            var workingPath = Directory.GetParent(location).FullName;
            location = location.Substring(location.LastIndexOf('\\') + 1);

            ProcessStartInfo processStartInfo = new ProcessStartInfo(eac3toPath, $"\"{location}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = workingPath
            };
            Process process = Process.Start(processStartInfo);
            Debug.Assert(process != null);
            string text = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            //while (!process.HasExited) Application.DoEvents();
            if (text.Contains("HD DVD / Blu-Ray disc structure not found."))
            {
                throw new Exception("May be the path is too complex or directory contains nonAscii characters");
            }
            OnLog?.Invoke("Disc Info:\r\n" + text.Replace('\b', '\uFEFF'));

            foreach (Match match in RDiskInfo.Matches(text))
            {
                var index = match.Groups["idx"].Value;
                var mpls = match.Groups["mpls"].Value;
                var time = match.Groups["dur"].Value;
                if (string.IsNullOrEmpty(time)) time = match.Groups["dur2"].Value;
                var file = match.Groups["fn"].Value;
                if (string.IsNullOrEmpty(file)) file = match.Groups["fn2"].Value;
                OnLog?.Invoke($"+ {index}) {mpls} -> [{file}] - [{time}]");

                ChapterInfo chapterInfo = new ChapterInfo
                {
                    Duration = TimeSpan.Parse(time),
                    SourceIndex = index,
                    SourceName = file
                };
                list.Add(chapterInfo);
            }
            var toBeRemove = new List<ChapterInfo>();
            var chapterPath = Path.Combine(workingPath, "chapters.txt");
            var logPath = Path.Combine(workingPath, "chapters - Log.txt");
            foreach (ChapterInfo current in list)
            {
                processStartInfo.Arguments = $"\"{location}\" {current.SourceIndex})";
                process = Process.Start(processStartInfo);
                Debug.Assert(process != null);
                text = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (!text.Contains("Chapters"))
                {
                    toBeRemove.Add(current);
                    continue;
                }
                processStartInfo.Arguments = $"\"{location}\" {current.SourceIndex}) chapters.txt";
                process = Process.Start(processStartInfo);
                Debug.Assert(process != null);
                text = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (!text.Contains("Creating file \"chapters.txt\"...") && !text.Contains("Done!"))
                {
                    throw new Exception("Error creating chapters file.");
                }
                current.Chapters = OgmData.GetChapterInfo(File.ReadAllBytes(chapterPath).GetUTF8String()).Chapters;
                if (current.Chapters.First().Name != "") continue;
                var chapterName = ChapterName.GetChapterName("Chapter");
                current.Chapters.ForEach(chapter => chapter.Name = chapterName());
            }
            toBeRemove.ForEach(item => list.Remove(item));
            if(File.Exists(chapterPath)) File.Delete(chapterPath);
            if(File.Exists(logPath)) File.Delete(logPath);
            return list;
        }
    }
}

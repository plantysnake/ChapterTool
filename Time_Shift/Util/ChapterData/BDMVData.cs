using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChapterTool.Util.ChapterData
{
    public static class BDMVData
    {
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
                eac3toPath = Notification.InputBox("请输入eac3to的地址", "注意不要带上多余的空格", "C:\\eac3to\\eac3to.exe");
                if (string.IsNullOrEmpty(eac3toPath)) return list;
                RegistryStorage.Save(name: "eac3toPath",value: eac3toPath);
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo(eac3toPath, $"\"{location}\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Application.StartupPath
            };
            Process process = Process.Start(processStartInfo);
            Debug.Assert(process != null);
            string text = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (text.Contains("HD DVD / Blu-Ray disc structure not found."))
            {
                throw new Exception("May be path is too complex or contains nonAscii characters");
            }
            foreach (Match match in RDiskInfo.Matches(text))
            {
                var index = match.Groups["idx"].Value;
                var mpls = match.Groups["mpls"].Value;
                var time = match.Groups["dur"].Value;
                if (string.IsNullOrEmpty(time)) time = match.Groups["dur2"].Value;
                var file = match.Groups["fn"].Value;
                if (string.IsNullOrEmpty(file)) file = match.Groups["fn2"].Value;

                ChapterInfo chapterInfo = new ChapterInfo
                {
                    Duration = TimeSpan.Parse(time),
                    SourceIndex = index,
                    SourceName = file
                };
                list.Add(chapterInfo);
            }
            var toBeRemove = new List<ChapterInfo>();
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
                if (File.Exists("chapters.txt"))
                {
                    File.Delete("chapters.txt");
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
                current.Chapters = OgmData.GetChapterInfo(File.ReadAllBytes("chapters.txt").GetUTF8String()).Chapters;
                if (current.Chapters.First().Name != "") continue;
                var chapterName = ChapterName.GetChapterName("Chapter");
                current.Chapters.ForEach(chapter => chapter.Name = chapterName());
            }
            toBeRemove.ForEach(item => list.Remove(item));
            return list;
        }
    }

}
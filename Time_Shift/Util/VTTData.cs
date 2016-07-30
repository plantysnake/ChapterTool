using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChapterTool.Util
{
    public static class VTTData
    {
        public static ChapterInfo GetChapterInfo(string text)
        {
            var info  = new ChapterInfo { SourceType = "WebVTT", Tag = text, TagType = text.GetType() };
            text = text.Replace("\r", "");
            var nodes = Regex.Split(text, "\n\n");
            if (nodes.Length < 1 || nodes[0].IndexOf("WEBVTT", StringComparison.Ordinal) < 0)
            {
                throw new Exception($"ERROR: Empty or invalid file type");
            }
            int index = 0;
            nodes.Skip(1).ToList().ForEach(node =>
            {
                var lines = node.Split('\n');
                lines = lines.SkipWhile(line => line.IndexOf("-->", StringComparison.Ordinal) < 0).ToArray();
                if (lines.Length < 2)
                {
                    throw new Exception($"+Parser Failed: Happened at [{node}]");
                }
                var times = Regex.Split(lines[0], "-->").Select(TimeSpan.Parse).ToArray();
                info.Chapters.Add(new Chapter(lines[1], times[0], ++index));
            });
            return info;
        }
    }
}

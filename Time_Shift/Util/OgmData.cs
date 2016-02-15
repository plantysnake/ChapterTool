using System;
using System.Linq;
using static ChapterTool.Util.CTLogger;
using static ChapterTool.Util.ConvertMethod;

namespace ChapterTool.Util
{
    public static class OgmData
    {
       public static ChapterInfo GetChapterInfo(string text, bool autoGenName)
        {
            int index = 0;
            var info = new ChapterInfo { SourceType = "OGM", Tag = text, TagType = text.GetType() };
            var ogmData = text.Trim(' ', '\r', '\n').Split('\n').SkipWhile(string.IsNullOrWhiteSpace).GetEnumerator();
            if (!ogmData.MoveNext()) return info;
            TimeSpan iniTime = OffsetCal(ogmData.Current);
            do
            {
                string buffer1 = ogmData.Current;
                ogmData.MoveNext();
                string buffer2 = ogmData.Current;
                if (string.IsNullOrWhiteSpace(buffer1) || string.IsNullOrWhiteSpace(buffer2))
                {
                    Log($"interrupt at '{buffer1}'  '{buffer2}'");
                    break;
                }
                if (RLineOne.IsMatch(buffer1) && RLineTwo.IsMatch(buffer2))
                {
                    info.Chapters.Add(ChapterInfo.WriteToChapterInfo(buffer1, buffer2, ++index, iniTime, autoGenName));
                    continue;
                }
                throw new FormatException($"invalid format: \n'{buffer1}' \n'{buffer2}' ");
            } while (ogmData.MoveNext());
            if (info.Chapters.Count > 1)
            {
                info.Duration = info.Chapters.Last().Time;
            }
            ogmData.Dispose();
            return info;
        }
    }
}

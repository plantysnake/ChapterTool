using Knuckleball;

namespace ChapterTool.Util
{
    public class Mp4Data
    {
        public ChapterInfo Chapter { get; private set; }

        public Mp4Data(string path)
        {
            ChapterList chapterList = MP4File.Open(path).Chapters;
            Chapter = new ChapterInfo();
            int index = 0;
            foreach (var chapterClip in chapterList)
            {
                Chapter.Chapters.Add(new Chapter(chapterClip.Title, Chapter.Duration, ++index));
                Chapter.Duration += chapterClip.Duration;
            }
        }
    }
}
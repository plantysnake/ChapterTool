using ChapterTool.Util;

namespace ChapterTool.ChapterData
{
    public interface IData// : IEnumerable<ChapterInfo>
    {
        int Count { get; }

        ChapterInfo this[int index] { get; }

        string ChapterType { get; }

        //event Action<string> OnLog;
    }
}

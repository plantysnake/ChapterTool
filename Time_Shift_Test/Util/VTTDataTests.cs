using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class VTTDataTests
    {
        [TestMethod()]
        public void GetChapterInfoTest()
        {
            string path = @"..\..\[VTT_Sample]\chapter.vtt";
            if (!File.Exists(path)) path = @"..\" + path;
            var ret = VTTData.GetChapterInfo(File.ReadAllText(path));
            Console.WriteLine(ret);
            ret.Chapters.ForEach(Console.WriteLine);
        }
    }
}

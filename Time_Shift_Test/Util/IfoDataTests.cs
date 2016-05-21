using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class IfoDataTests
    {
        [TestMethod()]
        public void IfoDataTest()
        {
            string path = @"..\..\[ifo_Sample]\VTS_05_0.IFO";
            if (!File.Exists(path)) path = @"..\" + path;
            var result = IfoData.GetStreams(path).ToList();
            Assert.IsTrue(result.Count == 1);
            Assert.IsTrue(result[0].Chapters.Count == 7);

            var expectResult = new[]
            {
                new { Name = "Chapter 01", Time = "00:00:00.000" },
                new { Name = "Chapter 02", Time = "00:17:42.500" },
                new { Name = "Chapter 03", Time = "00:37:14.767" },
                new { Name = "Chapter 04", Time = "00:56:24.167" },
                new { Name = "Chapter 05", Time = "01:12:36.701" },
                new { Name = "Chapter 06", Time = "01:32:26.268" },
                new { Name = "Chapter 07", Time = "01:49:06.136" }
            };

            Console.WriteLine(result[0]);
            int index = 0;
            foreach (var chapter in result[0].Chapters)
            {
                Console.WriteLine(chapter);
                Assert.IsTrue(expectResult[index].Name == chapter.Name);
                Assert.IsTrue(expectResult[index].Time == chapter.Time.Time2String());
                ++index;
            }
        }
    }
}

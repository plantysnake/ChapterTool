using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ChapterTool.Util;
using System;
using System.IO;
using ChapterTool.Util.ChapterData;

namespace Knuckleball.Tests
{
    [TestClass()]
    public class MP4FileTests
    {
        [TestMethod()]
        public void Mp4ChapterTest()
        {
            string path = @"..\..\[Video_Sample]\Chapter.mp4";
            if (!File.Exists(path)) path = @"..\" + path;
            var expectResult = new[]
            {
                new { Name = "Chapter 01", Time = "00:00:00.000" },
                new { Name = "Chapter 02", Time = "00:00:10.000" },
                new { Name = "Chapter 03", Time = "00:00:20.000" },
                new { Name = "Chapter 04", Time = "00:00:30.000" }
            };

            Mp4Data result = new Mp4Data(path);

            Assert.IsTrue(result.Chapter.Chapters.Count == 4);

            int index = 0;
            foreach (var chapter in result.Chapter.Chapters)
            {
                Console.WriteLine(chapter);
                Assert.IsTrue(expectResult[index].Name == chapter.Name);
                Assert.IsTrue(expectResult[index].Time == chapter.Time.Time2String());
                ++index;
            }
        }
    }
}

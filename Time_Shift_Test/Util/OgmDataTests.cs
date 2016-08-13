using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChapterTool.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapterTool.Util.ChapterData;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class OgmDataTests
    {
        [TestMethod()]
        public void OgmDataTest()
        {
            string path = @"..\..\[ogm_Sample]\00001.txt";
            if (!File.Exists(path)) path = @"..\" + path;
            var result = OgmData.GetChapterInfo(File.ReadAllText(path));
            var expectResult = new[]
            {
                new { Name = "Chapter 01", Time = "00:00:00.000" },
                new { Name = "Chapter 02", Time = "00:00:41.041" },
                new { Name = "Chapter 03", Time = "00:02:12.799" },
                new { Name = "Chapter 04", Time = "00:03:36.258" },
                new { Name = "Chapter 05", Time = "00:04:37.944" },
                new { Name = "Chapter 06", Time = "00:05:44.928" },
                new { Name = "Chapter 07", Time = "00:08:59.247" }
            };

            Console.WriteLine(result);
            int index = 0;
            foreach (var chapter in result.Chapters)
            {
                Console.WriteLine(chapter);
                Assert.IsTrue(expectResult[index].Name == chapter.Name);
                Assert.IsTrue(expectResult[index].Time == chapter.Time.Time2String());
                ++index;
            }
        }
    }
}

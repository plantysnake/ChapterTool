using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ChapterTool.Util.ChapterData;
using FluentAssertions;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class CueDataTests
    {
        [TestMethod()]
        public void PraseCueTest()
        {
            string path = @"..\..\[cue_Sample]\ARCHIVES 2.cue";
            if (!File.Exists(path)) path = @"..\" + path;
            var expectResult = new[]
            {
                new { Name = "オーディオドラマ・1stパート", Time = "00:00:00.000"},
                new { Name = "初色bloomy [初春飾利(豊崎愛生)]", Time = "00:15:19.280"},
                new { Name = "オーディオドラマ・2ndパート", Time = "00:19:15.093"},
                new { Name = "ナミダ御免のGirls Beat [佐天涙子(伊藤かな恵)]", Time = "00:32:12.173"}
            };

            var result = CueData.PraseCue(File.ReadAllText(path));

            Assert.IsTrue(result.Chapters.Count == 4);

            int index = 0;
            foreach (var chapter in result.Chapters)
            {
                Console.WriteLine(chapter.ToString());
                expectResult[index].Name.Should().Be(chapter.Name);
                expectResult[index].Time.Should().Be(chapter.Time.Time2String());
                ++index;
            }
        }
    }
}

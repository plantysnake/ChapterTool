using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChapterTool.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class CueDataTests
    {
        [TestMethod()]
        public void PraseCueTest()
        {
            const string path = @"..\..\[cue_Sample]\ARCHIVES 2.cue";
            var result = CueData.PraseCue(File.ReadAllText(path));
            Assert.IsTrue(result.Chapters.Count == 4);
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Knuckleball;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapterTool.Util;

namespace Knuckleball.Tests
{
    [TestClass()]
    public class MP4FileTests
    {
        [TestMethod()]
        public void OpenTest()
        {
            const string path = @"E:\[VCB-Studio] Kiniro Mosaic S1+S2 [720p]\SPs\[VCB-Studio] Hello!! Kiniro Mosaic [NCED][720p][x264_aac].mp4";
            Knuckleball.MP4File file = Knuckleball.MP4File.Open(path);
            foreach (var chapter in file.Chapters)
            {
                Debug.WriteLine(chapter);
            }

            Mp4Data data = new Mp4Data(path);
            var t = data.Chapter;
            foreach (var chapter in t.Chapters)
            {
                Console.WriteLine(chapter);
            }
        }
    }
}
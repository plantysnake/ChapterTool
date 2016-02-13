using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChapterTool.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class MplsDataTests
    {
        [TestMethod()]
        public void MplsDataTest1()
        {
            const string mplsPath = @"..\..\[mpls_Sample]\00011_eva.mpls";
            var mplsRaw = new MplsData(mplsPath);
            //mplsRaw.EntireTimeStamp.ForEach(item=>Console.Write($"{item}, "));
            Console.WriteLine(mplsRaw.ToString());

            var expectedTimeStamps = new List<int> { 0, 648750, 984375, 23799375, 27487500, 28044375, 28276875, 28918125, 29195625, 36823125, 41679375, 52321875, 56593125, 62563125, 73524375, 83199375, 95167500, 100741875, 106155000, 116420625, 120845625, 126307500, 129403125, 139273125, 141071250, 142704375, 147866250, 151578750, 157603125, 163599375, 170810625, 178768125, 186941250, 191786250, 192165000, 202076250, 213168750, 222028125, 228003750, 236915625, 244306875, 253316250, 260053125, 271863750, 284366250, 285738750 };
            var offset = mplsRaw.ChapterClips.First().TimeStamp.First();
            Assert.IsTrue(mplsRaw.ChapterClips.Count == 1);
            Assert.IsTrue(mplsRaw.ChapterClips.First().TimeStamp.Select(item => item - offset).SequenceEqual(expectedTimeStamps));
            Assert.IsTrue(mplsRaw.ChapterClips.First().Name    == "00002");
            Assert.IsTrue(mplsRaw.ChapterClips.First().Fps     == 2);
            Assert.IsTrue(mplsRaw.ChapterClips.First().TimeIn  == 188460000);
            Assert.IsTrue(mplsRaw.ChapterClips.First().TimeOut == 474480000);
            Assert.IsTrue(mplsRaw.ChapterClips.First().Length  == 286020000);
            Assert.IsTrue(mplsRaw.EntireTimeStamp.SequenceEqual(expectedTimeStamps));
        }

        [TestMethod()]
        public void MplsDataTest2()
        {
            const string mplsPath = @"..\..\[mpls_Sample]\00001_fch.mpls";
            var mplsRaw = new MplsData(mplsPath);
            Console.WriteLine(mplsRaw.ToString());
            var expectedTimeStamps = new List<int> { 0, 41963170, 96516418, 96831733, 98138038, 102186457, 131841081, 158573411, 162621830 };
            var offset = mplsRaw.ChapterClips.First().TimeStamp.First();
            Assert.IsTrue(mplsRaw.ChapterClips.Count == 1);
            Assert.IsTrue(mplsRaw.ChapterClips.First().TimeStamp.Select(item => item - offset).SequenceEqual(expectedTimeStamps));
            Assert.IsTrue(mplsRaw.ChapterClips.First().Name    == "00001");
            Assert.IsTrue(mplsRaw.ChapterClips.First().Fps     == 1);
            Assert.IsTrue(mplsRaw.ChapterClips.First().TimeIn  == 90000);
            Assert.IsTrue(mplsRaw.ChapterClips.First().TimeOut == 163027149);
            Assert.IsTrue(mplsRaw.ChapterClips.First().Length  == 162937149);
            Assert.IsTrue(mplsRaw.EntireTimeStamp.SequenceEqual(expectedTimeStamps));
        }

        [TestMethod()]
        public void MplsDataTest3()
        {
            const string mplsPath = @"..\..\[mpls_Sample]\00002_tanji.mpls";
            var mplsRaw = new MplsData(mplsPath);

            Console.WriteLine(mplsRaw.ToString());
            Assert.IsTrue(mplsRaw.ChapterClips.Count == 9);
            var expectedClip = new List<List<int>>
            {
                new List<int> {189000000},
                new List<int>(),
                new List<int> {195654375, 216264339},
                new List<int> {237796875},
                new List<int> {243031875, 252622706},
                new List<int> {252885000, 257070431, 261118850, 276148865},
                new List<int> {310860000},
                new List<int> {315736875, 316493255},
                new List<int> {316762500, 325401755, 329453928, 344619078, 376388941, 380439238, 380664463}
            };
            for (int i = 0; i < 9; i++)
            {
                Assert.IsTrue(mplsRaw.ChapterClips[i].TimeStamp.SequenceEqual(expectedClip[i]));
            }
            var expectedClipName = new List<string> { "00005", "00006&00007", "00008", "00009&00010", "00011", "00012", "00013&00014", "00015", "00016" };
            for (int i = 0; i < 9; i++)
            {
                Assert.IsTrue(mplsRaw.ChapterClips[i].Name == expectedClipName[i]);
            }
            var expectedClipLength = new List<int> { 1664788, 4996241, 42184642, 5240235, 9862978, 58032975, 4881751, 1026650, 63947008 };
            for (int i = 0; i < 9; i++)
            {
                Assert.IsTrue(mplsRaw.ChapterClips[i].Length == expectedClipLength[i]);
            }
            Assert.IsTrue(mplsRaw.ChapterClips.First().Fps == 1);
            var expectedTimeStamps = new List<int> { 0, 6661029, 27270993, 48845671, 54085906, 63676737, 63948884, 68134315, 72182734, 87212749, 121981859, 126863610, 127619990, 127890260, 136529515, 140581688, 155746838, 187516701, 191566998, 191792223 };
            Assert.IsTrue(mplsRaw.EntireTimeStamp.SequenceEqual(expectedTimeStamps));
        }

        [TestMethod()]
        public void ToChapterInfoTest()
        {
            const string mplsPath = @"..\..\[mpls_Sample]\00011_eva.mpls";
            var mplsRaw = new MplsData(mplsPath);
            var combinedCi = mplsRaw.ToChapterInfo(100, true);
            Console.WriteLine(combinedCi.ToString());
        }
    }
}
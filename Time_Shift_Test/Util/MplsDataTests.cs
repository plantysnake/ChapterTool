using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using ChapterTool.Util.ChapterData;
using FluentAssertions;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class MplsDataTests
    {
        [TestMethod()]
        public void MplsDataTest1()
        {
            string mplsPath = @"..\..\[mpls_Sample]\00011_eva.mpls";
            if (!File.Exists(mplsPath)) mplsPath = @"..\" + mplsPath;
            var mplsRaw = new MplsData(mplsPath);
            //mplsRaw.EntireTimeStamp.ForEach(item=>Console.Write($"{item}, "));
            Console.WriteLine(mplsRaw.ToString());

            var expectedTimeStamps = new List<int> { 0, 648750, 984375, 23799375, 27487500, 28044375, 28276875, 28918125, 29195625, 36823125, 41679375, 52321875, 56593125, 62563125, 73524375, 83199375, 95167500, 100741875, 106155000, 116420625, 120845625, 126307500, 129403125, 139273125, 141071250, 142704375, 147866250, 151578750, 157603125, 163599375, 170810625, 178768125, 186941250, 191786250, 192165000, 202076250, 213168750, 222028125, 228003750, 236915625, 244306875, 253316250, 260053125, 271863750, 284366250, 285738750 };
            mplsRaw.ChapterClips.Should().HaveCount(1);
            var clip = mplsRaw.ChapterClips.First();
            var offset = clip.TimeStamp.First();
            clip.TimeStamp.Select(item => item - offset).Should().Equal(expectedTimeStamps);
            clip.Name.Should().Be("00002");
            clip.Fps.Should().Be(2);
            clip.TimeIn.Should().Be(188460000);
            clip.TimeOut.Should().Be(474480000);
            clip.Length.Should().Be(286020000);
            mplsRaw.EntireClip.TimeStamp.Should().Equal(expectedTimeStamps);
        }

        [TestMethod()]
        public void MplsDataTest2()
        {
            string mplsPath = @"..\..\[mpls_Sample]\00001_fch.mpls";
            if (!File.Exists(mplsPath)) mplsPath = @"..\" + mplsPath;
            var mplsRaw = new MplsData(mplsPath);
            Console.WriteLine(mplsRaw.ToString());

            var expectedTimeStamps = new List<int> { 0, 41963170, 96516418, 96831733, 98138038, 102186457, 131841081, 158573411, 162621830 };
            mplsRaw.ChapterClips.Should().HaveCount(1);
            var clip = mplsRaw.ChapterClips.First();
            var offset = clip.TimeStamp.First();
            clip.TimeStamp.Select(item => item - offset).Should().Equal(expectedTimeStamps);
            clip.Name.Should().Be("00001");
            clip.Fps.Should().Be(1);
            clip.TimeIn.Should().Be(90000);
            clip.TimeOut.Should().Be(163027149);
            clip.Length.Should().Be(162937149);
            mplsRaw.EntireClip.TimeStamp.Should().Equal(expectedTimeStamps);
        }

        [TestMethod()]
        public void MplsDataTest3()
        {
            string mplsPath = @"..\..\[mpls_Sample]\00002_tanji.mpls";
            if (!File.Exists(mplsPath)) mplsPath = @"..\" + mplsPath;
            var mplsRaw = new MplsData(mplsPath);

            Console.WriteLine(mplsRaw.ToString());
            mplsRaw.ChapterClips.Should().HaveCount(9);
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
                mplsRaw.ChapterClips[i].TimeStamp.Should().Equal(expectedClip[i]);
            }
            mplsRaw.ChapterClips.Select(item => item.Name).Should().Equal("00005", "00006&00007", "00008", "00009&00010", "00011", "00012", "00013&00014", "00015", "00016");
            mplsRaw.ChapterClips.Select(item => item.Length).Should().Equal(1664788, 4996241, 42184642, 5240235, 9862978, 58032975, 4881751, 1026650, 63947008);
            mplsRaw.ChapterClips.First().Fps.Should().Be(1);
            mplsRaw.EntireClip.TimeStamp.Should()
                .Equal(0, 6661029, 27270993, 48845671, 54085906, 63676737, 63948884, 68134315, 72182734, 87212749,
                    121981859, 126863610, 127619990, 127890260, 136529515, 140581688, 155746838, 187516701, 191566998,
                    191792223);
        }

        [TestMethod()]
        public void ToChapterInfoTest()
        {
            string mplsPath = @"..\..\[mpls_Sample]\00011_eva.mpls";
            if (!File.Exists(mplsPath)) mplsPath = @"..\" + mplsPath;
            var mplsRaw = new MplsData(mplsPath);
            new Action(() => mplsRaw.ToChapterInfo(1, false)).ShouldThrow<IndexOutOfRangeException>()
                .WithMessage("Index of Video Clip out of range");
            Console.WriteLine(mplsRaw.ToChapterInfo(100, true).ToString());
        }
    }
}
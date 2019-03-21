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
            // mplsRaw.EntireTimeStamp.ForEach(item=>Console.Write($"{item}, "));
            Console.WriteLine(mplsRaw.ToString());

            mplsRaw.PlayItems[0].ClipName.ToString().Should().Be("00002.M2TS");
            mplsRaw.PlayItems[0].STNTable.StreamEntries.First(item => item is PrimaryVideoStreamEntry).StreamAttributes.FrameRate.Should().Be(2);
            mplsRaw.PlayItems[0].TimeInfo.INTime.Should().Be(188460000);
            mplsRaw.PlayItems[0].TimeInfo.OUTTime.Should().Be(474480000);

            var expectedTimeStamps = new List<uint> { 0, 648750, 984375, 23799375, 27487500, 28044375, 28276875, 28918125, 29195625, 36823125, 41679375, 52321875, 56593125, 62563125, 73524375, 83199375, 95167500, 100741875, 106155000, 116420625, 120845625, 126307500, 129403125, 139273125, 141071250, 142704375, 147866250, 151578750, 157603125, 163599375, 170810625, 178768125, 186941250, 191786250, 192165000, 202076250, 213168750, 222028125, 228003750, 236915625, 244306875, 253316250, 260053125, 271863750, 284366250, 285738750 };
            mplsRaw.PlayItems.Should().HaveCount(1);
            var clip = mplsRaw.Marks.Where(mark => mark.RefToPlayItemID == 0).ToList();
            var offset = clip.First().MarkTimeStamp;
            clip.Select(item => item.MarkTimeStamp - offset).Should().Equal(expectedTimeStamps);
        }

        [TestMethod()]
        public void MplsDataTest2()
        {
            string mplsPath = @"..\..\[mpls_Sample]\00001_fch.mpls";
            if (!File.Exists(mplsPath)) mplsPath = @"..\" + mplsPath;
            var mplsRaw = new MplsData(mplsPath);
            Console.WriteLine(mplsRaw.ToString());

            var expectedTimeStamps = new List<uint> { 0, 41963170, 96516418, 96831733, 98138038, 102186457, 131841081, 158573411, 162621830 };

            mplsRaw.PlayItems[0].ClipName.ToString().Should().Be("00001.M2TS");
            mplsRaw.PlayItems[0].STNTable.StreamEntries.First(item => item is PrimaryVideoStreamEntry).StreamAttributes.FrameRate.Should().Be(1);
            mplsRaw.PlayItems[0].TimeInfo.INTime.Should().Be(90000);
            mplsRaw.PlayItems[0].TimeInfo.OUTTime.Should().Be(163027149);

            mplsRaw.PlayItems.Should().HaveCount(1);
            var clip = mplsRaw.Marks.Where(mark => mark.RefToPlayItemID == 0 && mark.MarkType == 1).ToList();
            var offset = clip.First().MarkTimeStamp;
            clip.Select(item => item.MarkTimeStamp - offset).Should().Equal(expectedTimeStamps);
        }

        [TestMethod()]
        public void MplsDataTest3()
        {
            string mplsPath = @"..\..\[mpls_Sample]\00002_tanji.mpls";
            if (!File.Exists(mplsPath)) mplsPath = @"..\" + mplsPath;
            var mplsRaw = new MplsData(mplsPath);

            Console.WriteLine(mplsRaw.ToString());
            mplsRaw.PlayItems.Should().HaveCount(9);
            var expectedClip = new List<List<uint>>
            {
                new List<uint> {189000000},
                new List<uint>(),
                new List<uint> {195654375, 216264339},
                new List<uint> {237796875},
                new List<uint> {243031875, 252622706},
                new List<uint> {252885000, 257070431, 261118850, 276148865},
                new List<uint> {310860000},
                new List<uint> {315736875, 316493255},
                new List<uint> {316762500, 325401755, 329453928, 344619078, 376388941, 380439238, 380664463}
            };
            for (int i = 0; i < 9; i++)
            {
                var index = i;
                mplsRaw.Marks.Where(mark => mark.RefToPlayItemID == index).Select(item => item.MarkTimeStamp).Should().Equal(expectedClip[i]);
            }
            mplsRaw.PlayItems.Select(item=>item.FullName).Should().Equal("00005", "00006&00007", "00008", "00009&00010", "00011", "00012", "00013&00014", "00015", "00016");
            mplsRaw.PlayItems[0].STNTable.StreamEntries.First(item => item is PrimaryVideoStreamEntry).StreamAttributes.FrameRate.Should().Be(1);
        }
    }
}
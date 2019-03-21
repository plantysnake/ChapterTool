using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using ChapterTool.Util;
using FluentAssertions;

namespace SharpDvdInfo.Tests
{
    [TestClass()]
    public class DvdInfoContainerTests
    {
        [TestMethod()]
        public void DvdInfoContainerTest()
        {
            var expectResult = new[]
            {
                new { Name = "Chapter 01", Time = "00:00:00.000" },
                new { Name = "Chapter 02", Time = "00:17:42.500" },
                new { Name = "Chapter 03", Time = "00:37:14.767" },
                new { Name = "Chapter 04", Time = "00:56:24.167" },
                new { Name = "Chapter 05", Time = "01:12:36.701" },
                new { Name = "Chapter 06", Time = "01:32:26.268" },
                new { Name = "Chapter 07", Time = "01:49:06.136" },
                new { Name = "Chapter 08", Time = "01:49:06.636" }
            };
            string path = @"..\..\[ifo_Sample]\VTS_05_0.IFO";
            if (!File.Exists(path)) path = @"..\" + path;
            var result = new DvdInfoContainer(path).GetChapterInfo();
            int index = 0;
            foreach (var chapter in result[0].Chapters)
            {
                Console.WriteLine(chapter);
                expectResult[index].Name.Should().Be(chapter.Name);
                expectResult[index].Time.Should().Be(chapter.Time.Time2String());
                ++index;
            }
        }
        /*
        [TestMethod()]
        public void GetBitsTest()
        {
            var buff = new byte[2];
            for (byte i = 0; i < byte.MaxValue; ++i)
            {
                for (byte j = 0; j < byte.MaxValue; ++j)
                {
                    buff[0] = i; buff[1] = j;
                    for (byte len = 1; len < 8; ++len)
                    {
                        for (byte start = 0; start < 8 - len; ++start)
                        {
                            int res1 = DvdInfoContainer.GetBits(buff, len, start);
                            int res2 = DvdInfoContainer.GetBits_Effi(buff, len, start);
                            if (res1 != res2)
                            {
                                Console.WriteLine($"{res1} {res2}");
                            }
                            Assert.IsTrue(res1 == res2);
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void GetBitsTest2()
        {
            var buff = new List<byte[]>
            {
                new byte[] {0x04,0xe4},
                new byte[] {0x55,0x55},
                new byte[] {0xaa,0xaa},
                new byte[] {0xff,0x00},
                new byte[] {0x00,0xff},
            };
            foreach(var bt in buff)
            {

                for(byte i = 1; i < 16; ++i)
                    Console.WriteLine($"{DvdInfoContainer.GetBits(bt, i, 0) :X}");
                Console.WriteLine();
                // for (byte i = 7; i < 10; ++i)
                // {
                //    int res1 = DvdInfoContainer.GetBits(bt, i, 0);
                //    int res2 = DvdInfoContainer.GetBits_Effi(bt, i, 0);
                //    Console.WriteLine($"{(bt[0] << 8) | bt[1]:X} {res1:X} {res2:X}");
                //    Assert.IsTrue(res1 == res2);
                // }
            }
        }
        */
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SharpDvdInfo.Tests
{
    [TestClass()]
    public class DvdInfoContainerTests
    {
        [TestMethod()]
        public void DvdInfoContainerTest()
        {
            DvdInfoContainer dvd = new DvdInfoContainer("F:\\");
            dvd.Titles.ForEach(title =>
            {
                Console.WriteLine(title.TitleNumber + " " + title.TitleNumberInSet);
                title.Chapters.ForEach(stamp => Console.WriteLine(stamp));
            });

            Console.WriteLine(@"==File test==");
            string path = @"..\..\[ifo_Sample]\VTS_05_0.IFO";
            if (!File.Exists(path)) path = @"..\" + path;
            dvd = new DvdInfoContainer(path);
            dvd.Titles.ForEach(title =>
            {
                Console.WriteLine(title.TitleNumber);
                title.Chapters.ForEach(stamp => Console.WriteLine(stamp));
            });
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
                //for (byte i = 7; i < 10; ++i)
                //{
                //    int res1 = DvdInfoContainer.GetBits(bt, i, 0);
                //    int res2 = DvdInfoContainer.GetBits_Effi(bt, i, 0);
                //    Console.WriteLine($"{(bt[0] << 8) | bt[1]:X} {res1:X} {res2:X}");
                //    Assert.IsTrue(res1 == res2);
                //}
            }
        }
        */
    }
}
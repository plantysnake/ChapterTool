using System;
using System.Text.RegularExpressions;
using ChapterTool.Util.ChapterData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChapterTool.Util.Tests
{
    [TestClass()]
    public class IfoParserTests
    {
        [TestMethod()]
        public void BcdToIntTest()
        {
            int validValueCount = 0;
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                var hex    = $"{i:X2}";
                var result = Regex.Match(hex, @"\d{2}");
                if (result.Success)
                {
                    Assert.IsTrue(int.Parse(hex) == IfoParser.BcdToInt((byte)i));
                    Console.Write($"[{hex}->{IfoParser.BcdToInt((byte)i):D3}] ");
                    ++validValueCount;
                }
                else Console.Write($"({hex}->{IfoParser.BcdToInt((byte)i):D3}) ");

                Console.Write((i + 1) % 8 == 0 ? Environment.NewLine : string.Empty);
            }
            Console.WriteLine($"Valid BCD Code under 8 bit: {validValueCount}");
        }
    }
}

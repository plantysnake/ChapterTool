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
    public class ToolKitsTests
    {
        [TestMethod()]
        public void Time2StringTest()
        {
            var tsList = new List<TimeSpan>
            {
                new TimeSpan(0, 1, 59, 45, 999),
                new TimeSpan(0, 2, 01, 15, 000),
                new TimeSpan(0, 3, 45, 59, 123),
                new TimeSpan(0, 4, 15, 01, 456),
                new TimeSpan(0, 5, 00, 00, 254)
            };

            var expectList = new List<string>
            {
                "01:59:45.999",
                "02:01:15.000",
                "03:45:59.123",
                "04:15:01.456",
                "05:00:00.254"
            };
            Assert.IsTrue(tsList.Select(item => item.Time2String()).SequenceEqual(expectList));
            Assert.IsTrue(expectList.Select(item => item.ToTimeSpan()).SequenceEqual(tsList));
        }

        [TestMethod()]
        public void ConvertFr2IndexTest()
        {
            var frameRate = new List<double> { 0, 24000D / 1001, 24D, 25D, 30000D / 1001, 50D, 60000D / 1001 };
            var expected = new List<int> {0, 1, 2, 3, 4, 6, 7};
            frameRate.Select(ToolKits.ConvertFr2Index).ToList().ForEach(Console.Write);
            Assert.IsTrue(frameRate.Select(ToolKits.ConvertFr2Index).SequenceEqual(expected));
        }
    }
}
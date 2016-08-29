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
    public class ExpressionTests
    {
        [TestMethod()]
        public void ExpressionConverTest()
        {
            var ret = new Expression("2^10%10   + 6 \t///comment sample");
            Assert.AreEqual("2 10 ^ 10 % 6 + ", ret.ToString());
            ret = new Expression("((a+b)*(c+d))/(((e)))");
            Assert.AreEqual("a b + c d + * e / ", ret.ToString());
        }

        [TestMethod()]
        public void ExpressionWEvalTest()
        {
            var ret = new Expression("1+1/2+1/3+1/4+1/5+1/6+1/7+1/8+1/9+1/10");
            Assert.AreEqual(2.9289682539682539682539682539682539M, ret.Eval());
            ret = new Expression("1-1/2-1/4+1/8-1/16+1/32-1/64+1/128-1/256+1/512-1/1024");
            Assert.AreEqual(0.3330078125M, ret.Eval());
            ret = new Expression("1-(1/2)-(1/4)+(1/8)-(1/16)+(1/32)-(1/64)+(1/128)-(1/256)+(1/512)-(1/1024)");
            Assert.AreEqual(0.3330078125M, ret.Eval());
            ret = new Expression("2^2^2^2");
            Console.WriteLine($"without bracket: {ret}");
            Assert.AreEqual(256, ret.Eval());
            ret = new Expression("2^(2^(2^2))");
            Console.WriteLine($"with    bracket: {ret}");
            Assert.AreEqual(65536, ret.Eval());
        }

        [TestMethod()]
        public void Uva12803Test()
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            string path = Directory.GetCurrentDirectory() + "\\..\\..\\";
            if (!File.Exists(path + "Util\\expression.in")) path += "..\\";
            path += "Util\\";
            var input  = File.ReadAllLines(path + "expression.in");
            var output = File.ReadAllLines(path + "expression.out");
            for (int i = 0; i < input.Length; ++i)
            {
                var exp = new Expression(input[i]);
                var tmp = exp.Eval().ToString("0.00");
                Assert.AreEqual(output[i], tmp);
                Console.Write($"{tmp} ");
            }
            timer.Stop();
            Console.WriteLine($"Duration: {timer.Elapsed.Milliseconds}ms");
        }
    }
}

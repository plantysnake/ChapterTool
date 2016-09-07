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

        private static void EvalAreEqual(decimal expected, string actual)
        {
            Assert.AreEqual(expected, new Expression(actual).Eval());
        }
        private static void EvalAreNearly(decimal expected, string actual)
        {
            var ret = new Expression(actual).Eval();
            Assert.IsTrue(Math.Abs(ret - expected) < 1e-10M);
        }

        [TestMethod()]
        public void ExpressionWithFunctionTest()
        {
            var ret = new Expression("floor(1.133) + floor(log10(1023)) - ceil(0.9)");
            Assert.AreEqual("1.133 floor 1023 log10 floor + 0.9 ceil - ", ret.ToString());
            Assert.AreEqual(3M, ret.Eval());
        }

        [TestMethod()]
        public void FunctionAbsTest()
        {
            EvalAreEqual(1908.8976M, "abs(-1908.8976)");
            EvalAreEqual(1908.8976M, "abs(1908.8976)");

            EvalAreEqual(1908, "abs(-1908)");
            EvalAreEqual(1908, "abs(1908)");
        }

        [TestMethod()]
        public void FunctionSctTest()
        {
            EvalAreNearly(1M, "sin(asin(1))");
            EvalAreNearly(1M, "cos(acos(1))");
            EvalAreNearly(1M, "tan(atan(1))");
        }

        [TestMethod()]
        public void FunctionLog10Test()
        {
            EvalAreEqual(3.0M, "log10(1000.0)");
            EvalAreEqual(3.0M, "log10(1000.0)");
            EvalAreEqual(14.0M, "log10(10 ^ 14)");
            EvalAreEqual(3.73895612695404M, "log10(5482.2158)");
            EvalAreEqual(14.6615511428938M, "log10(458723662312872.125782332587)");
            EvalAreEqual(-0.908382862219234M, "log10(0.12348583358871)");
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

        [TestMethod()]
        public void Postfix2InfixTest()
        {
            Assert.AreEqual("((x + (y * 64)) + (z * 256)) / 3", Expression.Postfix2Infix("x y 64 * + z 256 * + 3 /"));
            Assert.AreEqual("((x + y) + z) / 3", Expression.Postfix2Infix("x y + z + 3 /"));
        }
    }
}

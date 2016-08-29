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
            Assert.AreEqual(ret.ToString(), "2 10 ^ 10 % 6 + ");
            ret = new Expression("((a+b)*(c+d))/(((e)))");
            Assert.AreEqual(ret.ToString(), "a b + c d + * e / ");
        }

        [TestMethod()]
        public void ExpressionWEvalTest()
        {
            var ret = new Expression("1+1/2+1/3+1/4+1/5+1/6+1/7+1/8+1/9+1/10");
            Assert.AreEqual(ret.Eval(), 2.92896825396825396825396825396825396825396825396825396825M);
            ret = new Expression("1-1/2-1/4+1/8-1/16+1/32-1/64+1/128-1/256+1/512-1/1024");
            Assert.AreEqual(ret.Eval(), 0.3330078125M);
            ret = new Expression("2^2^2^2");
            Assert.AreEqual(ret.Eval(), 256);
            ret = new Expression("2^(2^(2^2))");
            Assert.AreEqual(ret.Eval(), 65536);
        }

        [TestMethod()]
        public void Uva12803Test()
        {
            string path = Directory.GetCurrentDirectory() + "\\..\\..\\";
            if (!File.Exists(path + "Util\\expression.in")) path += "..\\";
            path += "Util\\";
            var input = File.ReadAllLines(path + "expression.in");
            var output = File.ReadAllLines(path + "expression.out");
            for (int i = 0; i < input.Length; ++i)
            {
                var exp = new Expression(input[i]);
                var tmp = exp.Eval().ToString("0.00");
                Assert.AreEqual(output[i], tmp);
                Console.WriteLine(tmp);
            }
        }
    }
}

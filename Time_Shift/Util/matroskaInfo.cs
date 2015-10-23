using ChapterTool.Util;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace ChapterTool
{
    class matroskaInfo
    {
        public XmlDocument result;
        public matroskaInfo(string path, string program = "mkvextract.exe")
        {
            result = new XmlDocument();
            string arg = "chapters \"" + path + "\"";
            string xmlresult = runMkvextract(arg, program);
            result.LoadXml(xmlresult);
        }
        static string runMkvextract(string arguments,string program)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = program;
            process.StartInfo.Arguments = arguments;
            // 禁用操作系统外壳程序
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            process.Close();
            return output;
        }

    }
}

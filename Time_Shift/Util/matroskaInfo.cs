using ChapterTool.Util;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace ChapterTool
{
    class matroskaInfo
    {
        public ChapterInfo result;
        public matroskaInfo(string path, string program = "mkvextract.exe")
        {
            string arg = "chapters \"" + path + "\"";
            string xmlresult = runMkvextract(arg, program);
            result = parseXML(xmlresult);
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
        ChapterInfo parseXML(string input)
        {
            ChapterInfo info = new ChapterInfo();
            if (string.IsNullOrEmpty(input)) { return info; }
            Regex RTimeFormat = new Regex(@"(?<Hour>\d+):(?<Minute>\d+):(?<Second>\d+)\.(?<Millisecond>\d{3})");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);
            XmlElement root = doc.DocumentElement;
            XmlNodeList TimeNodes = root.SelectNodes("/Chapters/EditionEntry/ChapterAtom/ChapterTimeStart");
            XmlNodeList NameNodes = root.SelectNodes("/Chapters/EditionEntry/ChapterAtom/ChapterDisplay/ChapterString");
            if (TimeNodes.Count == 0 || NameNodes.Count == 0) { return info; }
            int j = 0;
            string text = string.Empty;
            foreach (XmlNode timenode in TimeNodes)
            {
                if (convertMethod.string2Time(timenode.InnerText) == new TimeSpan(0) && j != 0) { break; }//防止从mkv中读取两个章节
                Chapter temp = new Chapter();
                temp.Time = convertMethod.string2Time(RTimeFormat.Match(timenode.InnerText).ToString());
                temp.Name = NameNodes[j++].InnerText.ToString();
                temp.Number = j;
                info.Chapters.Add(temp);
            }
            return info;
        }
    }
}

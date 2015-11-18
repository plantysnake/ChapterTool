// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// ****************************************************************************

namespace ChapterTool.Util
{
    class MatroskaInfo
    {
        public System.Xml.XmlDocument Result = new System.Xml.XmlDocument();
        public MatroskaInfo(string path, string program = "mkvextract.exe")
        {
            string arg = $"chapters \"{path}\"";
            string xmlresult = RunMkvextract(arg, program);
            Result.LoadXml(xmlresult);
        }
        static string RunMkvextract(string arguments, string program)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = { FileName = program, Arguments = arguments, UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return output;
        }
    }
}

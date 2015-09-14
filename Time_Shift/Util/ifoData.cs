using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChapterTool.Util
{
    class ifoData
    {
        private byte[] data;
        //public List<Clip> chapterClips;

        public ifoData(string path)
        {
            data = File.ReadAllBytes(path);
            //chapterClips = new List<Clip>();
        }
    }
}

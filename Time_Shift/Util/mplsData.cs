using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChapterTool
{
    public class mplsData
    {
        public List<Clip> chapterClips;
        public List<int> entireTimeStamp;

        private byte[] data;
        private int PlaylistSectionStartAddress;
        private int PlaylistMarkSectionStartAddress;
        private int PlayItemNumber;
        private int PlayItemEntries;

        public mplsData(string path)
        {
            chapterClips    = new List<Clip>();
            entireTimeStamp = new List<int>();
            data = File.ReadAllBytes(path);
            parseHeader();
            int shift = 0;
            for (int playItemOrder = 0; playItemOrder < PlayItemNumber; playItemOrder++)
            {
                int length, itemStartAdress, streamCount;
                parsePlayItem(PlayItemEntries + shift, out length, out itemStartAdress, out streamCount);
                for (int streamOrder = 0; streamOrder < streamCount; streamOrder++)
                {
                    parseStream(itemStartAdress, streamOrder, playItemOrder);
                }
                shift += (length + 2);//for that not counting the two length bytes themselves.
            }
            //initializeTimeStampList();
            parsePlaylistMark();
        }

        private void parseHeader()
        {
            PlaylistSectionStartAddress = byte2int(data, 0x08, 0x0c);
            PlaylistMarkSectionStartAddress = byte2int(data, 0x0c, 0x10);
            PlayItemNumber = byte2int(data, PlaylistSectionStartAddress + 0x06, 
                                            PlaylistSectionStartAddress + 0x08);
            PlayItemEntries = PlaylistSectionStartAddress + 0x0a;
        }

        private void parsePlayItem(int PlayItemEntries, out int length, out int itemStartAdress, out int streamCount)
        {
            length = byte2int(data, PlayItemEntries + 0x00, PlayItemEntries + 0x02);
            string m2tsName = ASCIIEncoding.ASCII.GetString(data, PlayItemEntries + 0x02, 0x0b - 0x02);
            Clip streamClip = new Clip();

            streamClip.Name = m2tsName;
            streamClip.TimeIn = byte2int(data, PlayItemEntries + 0x0e, PlayItemEntries + 0x12);//start time 
            streamClip.TimeOut = byte2int(data, PlayItemEntries + 0x12, PlayItemEntries + 0x16);//end   time
            streamClip.Length = streamClip.TimeOut - streamClip.TimeIn;
            streamClip.RelativeTimeIn = chapterClips.Sum(c => c.Length);
            streamClip.RelativeTimeOut = streamClip.RelativeTimeIn + streamClip.Length;
            chapterClips.Add(streamClip);

            itemStartAdress = PlayItemEntries + 0x32;
            int UO2 = byte2int(data, PlayItemEntries + 0x22, PlayItemEntries + 0x23);
            streamCount = byte2int(data, PlayItemEntries + 0x23, PlayItemEntries + 0x24) >> 4;
            if (UO2 == 0x02)//ignore angles , can only operate angles == 1 or 2 
            {
                streamCount = byte2int(data, PlayItemEntries + 0x2f, PlayItemEntries + 0x30) >> 4;
                itemStartAdress = PlayItemEntries + 0x3e;
            }
        }
        private void parseStream(int itemStartAdress,int streamOrder,int playItemOrder)
        {
            byte[] Stream = new byte[16];
            for (int adress = 0; adress < 16; adress++)
            {
                Stream[adress] = data[itemStartAdress + streamOrder * 16 + adress];
            }

            if (Stream[01] == 0x01)// the stream type is a Play item
            {
                int streamCodingType = byte2int(Stream, 0x0b, 0x0c);
                if (streamCodingType == 0x1b)
                {
                    int attr = byte2int(Stream, 0x0c, 0x0d);
                    chapterClips[playItemOrder].fps = (attr & 0xf);//last 4 bits is the fps
                }
            }
        }
        private void parsePlaylistMark()
        {
            int PlaylistMarkNumber = byte2int(data, PlaylistMarkSectionStartAddress + 0x04, PlaylistMarkSectionStartAddress + 0x06);
            int PlaylistMarkEntries = PlaylistMarkSectionStartAddress + 0x06;
            byte[] bytelist = new byte[14];//eg. 0001 yyyy xxxxxxxx FFFF 000000
                                           // 0 unknown
                                           // 1 type
                                           // 2, 3 stream_file_index
                                           // 4, 5, 6, 7 chapter_time
                                           // 8 - 13 unknown
            
            for (int mark = 0; mark < PlaylistMarkNumber; ++mark)
            {
                Array.Copy(data, PlaylistMarkEntries, bytelist, 0, 14);
                if (1 != bytelist[1])
                {
                    PlaylistMarkEntries += 14;
                    continue;
                }
                int streamFileIndex = byte2int(bytelist, 2, 4);
                Clip streamClip = chapterClips[streamFileIndex];
                int TimeStamp = byte2int(bytelist, 4, 8);
                int relativeSeconds = TimeStamp - streamClip.TimeIn + streamClip.RelativeTimeIn;
                chapterClips[streamFileIndex].timeStamp.Add(TimeStamp);
                entireTimeStamp.Add(relativeSeconds);
                PlaylistMarkEntries += 14;
            }
        }

        private int byte2int(byte[] source, int start, int end)//0xFF
        {
            int temp = 0;
            for (int i = start; i < end; i++)
            {
                temp += source[i] * 1 << (8 * (end - i - 1));
            }
            return temp;
        }
    }

    public class Clip
    {
        public Clip()
        {
            timeStamp = new List<int>();
        }
        public string Name;
        public int fps;
        public List<int> timeStamp;
        public int Length;
        public int RelativeTimeIn;
        public int RelativeTimeOut;
        public int TimeIn;
        public int TimeOut;
    }
}

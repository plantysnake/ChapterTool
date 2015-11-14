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
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ChapterTool.Util
{
    public class mplsData
    {
        /// <summary>include all chapter in mpls divisionally</summary>
        public List<Clip> chapterClips { get; set; }
        /// <summary>include all time code in mpls</summary>
        public List<int> entireTimeStamp { get; set; }

        private byte[] data;
        private int PlaylistSectionStartAddress;
        private int PlaylistMarkSectionStartAddress;
        private int PlayItemNumber;
        private int PlayItemEntries;

        public mplsData(string path)
        {
            chapterClips    = new List<Clip>();
            entireTimeStamp = new List<int>();
            data            = File.ReadAllBytes(path);
            parseHeader();
            int shift = 0;
            for (int playItemOrder = 0; playItemOrder < PlayItemNumber; playItemOrder++)
            {
                int lengthOfPlayItem, itemStartAdress, streamCount;
                parsePlayItem(PlayItemEntries + shift, out lengthOfPlayItem, out itemStartAdress, out streamCount);
                for (int streamOrder = 0; streamOrder < streamCount; ++streamOrder)
                {
                    parseStream(itemStartAdress, streamOrder, playItemOrder);
                }
                shift += (lengthOfPlayItem + 2);//for that not counting the two length bytes themselves.
            }
            parsePlaylistMark();
        }

        private void parseHeader()
        {
            PlaylistSectionStartAddress     = byte2int(data, 0x08, 0x04);
            PlaylistMarkSectionStartAddress = byte2int(data, 0x0c, 0x04);
            PlayItemNumber = byte2int(data, PlaylistSectionStartAddress + 0x06, 0x02);
            PlayItemEntries = PlaylistSectionStartAddress + 0x0a;
        }

        private void parsePlayItem(int PlayItemEntries, out int lengthOfPlayItem, out int itemStartAdress, out int streamCount)
        {
            Clip streamClip            = new Clip();
            lengthOfPlayItem           = byte2int(data, PlayItemEntries + 0x00, 0x02);
            streamClip.Name            = Encoding.ASCII.GetString(data, PlayItemEntries + 0x02, 0x09);
            streamClip.TimeIn          = byte2int(data, PlayItemEntries + 0x0e, 0x04);
            streamClip.TimeOut         = byte2int(data, PlayItemEntries + 0x12, 0x04);
            streamClip.Length          = streamClip.TimeOut - streamClip.TimeIn;
            streamClip.RelativeTimeIn  = chapterClips.Sum(clip => clip.Length);
            streamClip.RelativeTimeOut = streamClip.RelativeTimeIn + streamClip.Length;
            chapterClips.Add(streamClip);

            itemStartAdress = PlayItemEntries + 0x32;
            int UO2     = byte2int(data, PlayItemEntries + 0x22, 0x01);
            streamCount = byte2int(data, PlayItemEntries + 0x23, 0x01) >> 4;
            if (0x02 == UO2)//ignore angles, can only operate angles == 1 or 2
            {
                streamCount     = byte2int(data, PlayItemEntries + 0x2f, 0x01) >> 4;
                itemStartAdress = PlayItemEntries + 0x3e;
            }
        }
        private void parseStream(int itemStartAdress,int streamOrder,int playItemOrder)
        {
            byte[] Stream = new byte[16];
            Array.Copy(data, itemStartAdress + streamOrder * 16, Stream, 0, 16);
            if (0x01 == Stream[01])// the stream type is a Play item
            {
                int streamCodingType = byte2int(Stream, 0x0b, 0x01);
                if (0x1b == streamCodingType)
                {
                    int attr = byte2int(Stream, 0x0c, 0x01);
                    chapterClips[playItemOrder].fps = (attr & 0xf);//last 4 bits is the fps
                }
            }
        }
        private void parsePlaylistMark()
        {
            int PlaylistMarkNumber  = byte2int(data, PlaylistMarkSectionStartAddress + 0x04, 0x02);
            int PlaylistMarkEntries = PlaylistMarkSectionStartAddress + 0x06;
            byte[] bytelist = new byte[14];// eg. 0001 yyyy xxxxxxxx FFFF 000000
                                           // 00       mark_id
                                           // 01       mark_type
                                           // 02 - 03  play_item_ref
                                           // 04 - 07  time
                                           // 08 - 09  entry_es_pid
                                           // 10 - 13  duration
            for (int mark = 0; mark < PlaylistMarkNumber; ++mark)
            {
                Array.Copy(data, PlaylistMarkEntries, bytelist, 0, 14);
                if (0x01 == bytelist[1])// the playlist mark type is an entry mark
                {
                    int streamFileIndex = byte2int(bytelist, 0x02, 0x02);
                    Clip streamClip     = chapterClips[streamFileIndex];
                    int TimeStamp       = byte2int(bytelist, 0x04, 0x04);
                    int relativeSeconds = TimeStamp - streamClip.TimeIn + streamClip.RelativeTimeIn;
                    chapterClips[streamFileIndex].timeStamp.Add(TimeStamp);
                    entireTimeStamp.Add(relativeSeconds);
                }
                PlaylistMarkEntries += 14;
            }
        }

        private int byte2int(byte[] bytes, int index, int count)//0xFF
        {
            int intValue = 0;
            for (int i = index; i < index + count; ++i)
            {
                intValue += bytes[i] << (8 * ((index + count - 1) - i));
            }
            return intValue;
        }
    }

    public class Clip
    {
        public string Name;
        public List<int> timeStamp = new List<int>();
        public int fps;
        public int Length;
        public int RelativeTimeIn;
        public int RelativeTimeOut;
        public int TimeIn;
        public int TimeOut;
    }
}

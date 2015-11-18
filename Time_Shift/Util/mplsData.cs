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
        public List<Clip> ChapterClips { get; set; }
        /// <summary>include all time code in mpls</summary>
        public List<int> EntireTimeStamp { get; set; }

        private byte[] data;
        private int _playlistSectionStartAddress;
        private int _playlistMarkSectionStartAddress;
        private int _playItemNumber;
        private int _playItemEntries;

        public mplsData(string path)
        {
            ChapterClips    = new List<Clip>();
            EntireTimeStamp = new List<int>();
            data            = File.ReadAllBytes(path);
            ParseHeader();
            int shift = 0;
            for (var playItemOrder = 0; playItemOrder < _playItemNumber; playItemOrder++)
            {
                int itemStartAdress, streamCount;
                int lengthOfPlayItem = ParsePlayItem(_playItemEntries + shift, out itemStartAdress, out streamCount);
                Enumerable.Range(0, streamCount).ToList().ForEach(streamOrder => ParseStream(itemStartAdress, streamOrder, playItemOrder));
                shift += (lengthOfPlayItem + 2);//for that not counting the two length bytes themselves.
            }
            ParsePlaylistMark();
        }

        private void ParseHeader()
        {
            _playlistSectionStartAddress     = Byte2Int(data, 0x08, 0x04);
            _playlistMarkSectionStartAddress = Byte2Int(data, 0x0c, 0x04);
            _playItemNumber = Byte2Int(data, _playlistSectionStartAddress + 0x06, 0x02);
            _playItemEntries = _playlistSectionStartAddress + 0x0a;
        }

        private int ParsePlayItem(int playItemEntries, out int itemStartAdress, out int streamCount)
        {
            int lengthOfPlayItem       = Byte2Int(data, playItemEntries + 0x00, 0x02);
            Clip streamClip = new Clip
            {
                Name = Encoding.ASCII.GetString(data, playItemEntries + 0x02, 0x09),
                TimeIn = Byte2Int(data, playItemEntries + 0x0e, 0x04),
                TimeOut = Byte2Int(data, playItemEntries + 0x12, 0x04)
            };
            streamClip.Length          = streamClip.TimeOut - streamClip.TimeIn;
            streamClip.RelativeTimeIn  = ChapterClips.Sum(clip => clip.Length);
            streamClip.RelativeTimeOut = streamClip.RelativeTimeIn + streamClip.Length;
            ChapterClips.Add(streamClip);

            itemStartAdress            = playItemEntries + 0x32;
            int UO2                    = Byte2Int(data, playItemEntries + 0x22, 0x01);
            streamCount                = Byte2Int(data, playItemEntries + 0x23, 0x01) >> 4;
            if (0x02 == UO2)//ignore angles, can only operate angles == 1 or 2
            {
                streamCount     = Byte2Int(data, playItemEntries + 0x2f, 0x01) >> 4;
                itemStartAdress = playItemEntries + 0x3e;
            }
            return lengthOfPlayItem;
        }
        private void ParseStream(int itemStartAdress,int streamOrder,int playItemOrder)
        {
            byte[] stream = new byte[16];
            Array.Copy(data, itemStartAdress + streamOrder * 16, stream, 0, 16);
            if (0x01 == stream[01])// the stream type is a Play item
            {
                int streamCodingType = Byte2Int(stream, 0x0b, 0x01);
                if (0x1b != streamCodingType) return;
                int attr = Byte2Int(stream, 0x0c, 0x01);
                ChapterClips[playItemOrder].Fps = (attr & 0xf);//last 4 bits is the fps
            }
        }
        private void ParsePlaylistMark()
        {
            int playlistMarkNumber  = Byte2Int(data, _playlistMarkSectionStartAddress + 0x04, 0x02);
            int playlistMarkEntries = _playlistMarkSectionStartAddress + 0x06;
            byte[] bytelist = new byte[14];// eg. 0001 yyyy xxxxxxxx FFFF 000000
                                           // 00       mark_id
                                           // 01       mark_type
                                           // 02 - 03  play_item_ref
                                           // 04 - 07  time
                                           // 08 - 09  entry_es_pid
                                           // 10 - 13  duration
            for (var mark = 0; mark < playlistMarkNumber; ++mark)
            {
                Array.Copy(data, playlistMarkEntries, bytelist, 0, 14);
                if (0x01 == bytelist[1])// the playlist mark type is an entry mark
                {
                    int streamFileIndex = Byte2Int(bytelist, 0x02, 0x02);
                    Clip streamClip     = ChapterClips[streamFileIndex];
                    int timeStamp       = Byte2Int(bytelist, 0x04, 0x04);
                    int relativeSeconds = timeStamp - streamClip.TimeIn + streamClip.RelativeTimeIn;
                    ChapterClips[streamFileIndex].TimeStamp.Add(timeStamp);
                    EntireTimeStamp.Add(relativeSeconds);
                }
                playlistMarkEntries += 14;
            }
        }

        private int Byte2Int(IList<byte> bytes, int index, int count)//0xFF
        {
            int intValue = 0;
            for (var i = index; i < index + count; ++i)
            {
                intValue += bytes[i] << (8 * ((index + count - 1) - i));
            }
            return intValue;
        }
    }

    public class Clip
    {
        public string Name;
        public List<int> TimeStamp = new List<int>();
        public int Fps;
        public int Length;
        public int RelativeTimeIn;
        public int RelativeTimeOut;
        public int TimeIn;
        public int TimeOut;
    }
}

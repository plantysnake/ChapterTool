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
    public class MplsData
    {
        /// <summary>include all chapters in mpls divisionally</summary>
        public List<Clip> ChapterClips { get; }
        /// <summary>include all time code in mpls</summary>
        public List<int> EntireTimeStamp { get; }

        private readonly byte[] _data;
        private int _playlistSectionStartAddress;
        private int _playlistMarkSectionStartAddress;
        private int _playItemNumber;
        private int _playItemEntries;

        public MplsData(string path)
        {
            ChapterClips    = new List<Clip>();
            EntireTimeStamp = new List<int>();
            _data           = File.ReadAllBytes(path);
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
            string fileType = Encoding.ASCII.GetString(_data, 0, 8);
            if ((fileType != "MPLS0100" && fileType != "MPLS0200")
            /*|| _data[45] != 1*/)
            {
                throw new Exception($"This Playlist has an unknown file type {fileType}.");
            }
            _playlistSectionStartAddress     = Byte2Int32(_data, 0x08);
            _playlistMarkSectionStartAddress = Byte2Int32(_data, 0x0c);
            _playItemNumber  = Byte2Int16(_data, _playlistSectionStartAddress + 0x06);
            _playItemEntries = _playlistSectionStartAddress + 0x0a;
        }

        private int ParsePlayItem(int playItemEntries, out int itemStartAdress, out int streamCount)
        {
            int lengthOfPlayItem = Byte2Int16(_data, playItemEntries);
            var bytes            = new byte[lengthOfPlayItem + 2];
            Array.Copy(_data, playItemEntries, bytes, 0, lengthOfPlayItem);
            Clip streamClip = new Clip
            {
                TimeIn  = Byte2Int32(bytes, 0x0e),
                TimeOut = Byte2Int32(bytes, 0x12)
            };
            streamClip.Length          = streamClip.TimeOut - streamClip.TimeIn;
            streamClip.RelativeTimeIn  = ChapterClips.Sum(clip => clip.Length);
            streamClip.RelativeTimeOut = streamClip.RelativeTimeIn + streamClip.Length;

            itemStartAdress            = playItemEntries + 0x32;
            streamCount                = bytes[0x23] >> 4;
            int isMultiAngle = (bytes[0x0c] >> 4) & 0x01;

            StringBuilder sb = new StringBuilder(Encoding.ASCII.GetString(bytes, 0x02, 0x05));

            if (isMultiAngle == 1)
            {
                int numberOfAngles = bytes[0x22];
                for (int i = 1; i < numberOfAngles; i++)
                {
                    sb.Append("&" + Encoding.ASCII.GetString(bytes, 0x24 + (i - 1) * 0x0a, 0x05));
                }
                itemStartAdress = playItemEntries + 0x02 + (numberOfAngles - 1) * 0x0a;
                CTLogger.Log($"Chapter with {numberOfAngles} Angle, file name: {sb}");
            }
            streamClip.Name = sb.ToString();
            ChapterClips.Add(streamClip);
            return lengthOfPlayItem;
        }

        private void ParseStream(int itemStartAdress,int streamOrder,int playItemOrder)
        {
            var stream = new byte[16];
            Array.Copy(_data, itemStartAdress + streamOrder * 16, stream, 0, 16);
            if (0x01 != stream[01]) return;
            int streamCodingType = stream[0x0b];
            if (0x1b != streamCodingType && // AVC
                0x02 != streamCodingType && // MPEG-I/II
                0xea != streamCodingType)   // VC-1
                return;
            ChapterClips[playItemOrder].Fps = stream[0x0c] & 0xf;//last 4 bits is the fps
        }
        private void ParsePlaylistMark()
        {
            int playlistMarkNumber  = Byte2Int16(_data, _playlistMarkSectionStartAddress + 0x04);
            int playlistMarkEntries = _playlistMarkSectionStartAddress + 0x06;
            var bytelist = new byte[14];    // eg. 0001 yyyy xxxxxxxx FFFF 000000
                                            // 00       mark_id
                                            // 01       mark_type
                                            // 02 - 03  play_item_ref
                                            // 04 - 07  time
                                            // 08 - 09  entry_es_pid
                                            // 10 - 13  duration
            for (var mark = 0; mark < playlistMarkNumber; ++mark)
            {
                Array.Copy(_data, playlistMarkEntries, bytelist, 0, 14);
                if (0x01 == bytelist[1])//make sure the playlist mark type is an entry mark
                {
                    int streamFileIndex = Byte2Int16(bytelist, 0x02);
                    Clip streamClip     = ChapterClips[streamFileIndex];
                    int timeStamp       = Byte2Int32(bytelist, 0x04);
                    int relativeSeconds = timeStamp - streamClip.TimeIn + streamClip.RelativeTimeIn;
                    streamClip.TimeStamp.Add(timeStamp);
                    EntireTimeStamp.Add(relativeSeconds);
                }
                playlistMarkEntries += 14;
            }
        }

        private static short Byte2Int16(IReadOnlyList<byte> bytes, int index, bool bigEndian = true)
        {
            return (short)(bigEndian ? (bytes[index] << 8) + bytes[index + 1] :
                                       (bytes[index + 1] << 8) + bytes[index]);
        }

        private static int Byte2Int32(IReadOnlyList<byte> bytes, int index, bool bigEndian = true)
        {
            return bigEndian ? (bytes[index] << 24) + (bytes[index + 1] << 16) + (bytes[index + 2] << 8) + bytes[index + 3] :
                               (bytes[index + 3] << 24) + (bytes[index + 2] << 16) + (bytes[index + 1] << 8) + bytes[index];
        }
    }
}

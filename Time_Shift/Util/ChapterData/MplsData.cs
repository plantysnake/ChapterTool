// ****************************************************************************
//
// Copyright (C) 2014-2016 TautCony (TautCony@vcb-s.com)
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChapterTool.Util.ChapterData
{
    public class MplsData
    {
        /// <summary>include all chapters in mpls divisionally</summary>
        public List<Clip> ChapterClips   { get; } = new List<Clip>();

        /// <summary>include all time code in mpls</summary>
        public Clip EntireClip { get; } = new Clip {Name = "FULL Chapter" , TimeIn = 0};

        public override string ToString() => $"MPLS: {ChapterClips.Count} Viedo Clips, {EntireClip.TimeStamp.Count} Time Stamps";

        private readonly byte[] _data;

        public static readonly decimal[] FrameRate = { 0M, 24000M / 1001, 24M, 25M, 30000M / 1001, 0M, 50M, 60000M / 1001 };

        public delegate void LogEventHandler(string message);

        public static event LogEventHandler OnLog;

        public MplsData(string path)
        {
            _data = File.ReadAllBytes(path);
            ParseMpls();
        }

        private void ParseMpls()
        {
            int playlistMarkSectionStartAddress, playItemNumber, playItemEntries;
            ParseHeader(out playlistMarkSectionStartAddress, out playItemNumber, out playItemEntries);
            for (var playItemOrder = 0; playItemOrder < playItemNumber; playItemOrder++)
            {
                int lengthOfPlayItem, itemStartAdress, streamCount;
                ParsePlayItem(playItemEntries, out lengthOfPlayItem, out itemStartAdress, out streamCount);
                for (int streamOrder = 0; streamOrder < streamCount; streamOrder++)
                {
                    ParseStream(itemStartAdress, streamOrder, playItemOrder);
                }
                playItemEntries += lengthOfPlayItem + 2;//for that not counting the two length bytes themselves.
            }
            ParsePlaylistMark(playlistMarkSectionStartAddress);
            EntireClip.TimeOut = ChapterClips.Sum(item => item.Length);
            EntireClip.Length  = EntireClip.TimeOut;
        }

        private void ParseHeader(out int playlistMarkSectionStartAddress, out int playItemNumber, out int playItemEntries)
        {
            string fileType = Encoding.ASCII.GetString(_data, 0, 8);
            if ((fileType != "MPLS0100" && fileType != "MPLS0200") /*|| _data[45] != 1*/)
            {
                throw new Exception($"This Playlist has an unknown file type {fileType}.");
            }
            int playlistSectionStartAddress = Byte2Int32(_data, 0x08);
            playlistMarkSectionStartAddress = Byte2Int32(_data, 0x0c);
            playItemNumber                  = Byte2Int16(_data, playlistSectionStartAddress + 0x06);
            playItemEntries                 = playlistSectionStartAddress + 0x0a;
        }

        private bool ParsePlayItem(int playItemEntries, out int lengthOfPlayItem, out int itemStartAdress, out int streamCount)
        {
            bool mulitAngle           = false;
            lengthOfPlayItem     = Byte2Int16(_data, playItemEntries);
            var bytes            = new byte[lengthOfPlayItem + 2];
            Array.Copy(_data, playItemEntries, bytes, 0, lengthOfPlayItem + 2);
            Clip streamClip      = new Clip
            {
                TimeIn  = Byte2Int32(bytes, 0x0e),
                TimeOut = Byte2Int32(bytes, 0x12)
            };
            streamClip.Length          = streamClip.TimeOut - streamClip.TimeIn;
            streamClip.RelativeTimeIn  = ChapterClips.Sum(clip => clip.Length);
            streamClip.RelativeTimeOut = streamClip.RelativeTimeIn + streamClip.Length;

            itemStartAdress            = playItemEntries + 0x32;
            streamCount                = bytes[0x23] >> 4;
            int isMultiAngle           = (bytes[0x0c] >> 4) & 0x01;
            StringBuilder nameBuilder  = new StringBuilder(Encoding.ASCII.GetString(bytes, 0x02, 0x05));

            //todo: fps info of multi-angle has been skiped too.
            if (isMultiAngle == 1)  //skip multi-angle
            {
                mulitAngle = true;
                int numberOfAngles = bytes[0x22];
                for (int i = 1; i < numberOfAngles; i++)
                {
                    nameBuilder.Append("&" + Encoding.ASCII.GetString(bytes, 0x24 + (i - 1) * 0x0a, 0x05));
                }
                itemStartAdress = playItemEntries + 0x02 + (numberOfAngles - 1) * 0x0a;
                OnLog?.Invoke($"Chapter with {numberOfAngles} Angle, file name: {nameBuilder}");
            }
            streamClip.Name = nameBuilder.ToString();
            ChapterClips.Add(streamClip);
            return mulitAngle;
        }

        private static void StreamAttributeLog(string message)
        {
            OnLog?.Invoke(message);
        }

        private void ParseStream(int itemStartAdress, int streamOrder, int playItemOrder)
        {
            var stream = new byte[16];
            Array.Copy(_data, itemStartAdress + streamOrder * 16, stream, 0, 16);
            var streamCodingType = stream[0x0b];
            var chapterClip      = ChapterClips[playItemOrder];
            var clipName         = 0x01 == stream[01] ? chapterClip.Name : chapterClip.Name + " Sub Path";
            StreamAttribute.OnLog += StreamAttributeLog;
            StreamAttribute.LogStreamAttributes(stream, clipName);
            StreamAttribute.OnLog -= StreamAttributeLog;

            if (0x01 != stream[01]) return; //make sure this stream is Play Item
            if (0x1b != streamCodingType && 0x02 != streamCodingType &&
                0xea != streamCodingType && 0x06 != streamCodingType) return;
            chapterClip.Fps = stream[0x0c] & 0xf;//last 4 bits is the fps
        }

        private void ParsePlaylistMark(int playlistMarkSectionStartAddress)
        {
            int playlistMarkNumber  = Byte2Int16(_data, playlistMarkSectionStartAddress + 0x04);
            int playlistMarkEntries = playlistMarkSectionStartAddress + 0x06;
            var bytelist = new byte[14];    // eg. 0001 yyyy xxxxxxxx FFFF 000000
                                            // 00       mark_id
                                            // 01       mark_type
                                            // 02 - 03  play_item_ref
                                            // 04 - 07  time
                                            // 08 - 09  entry_es_pid
                                            // 10 - 13  duration
            for (var mark = 0; mark < playlistMarkNumber; ++mark)
            {
                Array.Copy(_data, playlistMarkEntries + mark * 14, bytelist, 0, 14);
                if (0x01 != bytelist[1]) continue;// make sure the playlist mark type is an entry mark
                int streamFileIndex = Byte2Int16(bytelist, 0x02);
                Clip streamClip     = ChapterClips[streamFileIndex];
                int timeStamp       = Byte2Int32(bytelist, 0x04);
                int relativeSeconds = timeStamp - streamClip.TimeIn + streamClip.RelativeTimeIn;
                streamClip.TimeStamp.Add(timeStamp);
                EntireClip.TimeStamp.Add(relativeSeconds);
            }
        }

        private static short Byte2Int16(IReadOnlyList<byte> bytes, int index, bool bigEndian = true)
        {
            return (short)(bigEndian ? (bytes[index] << 8) | bytes[index + 1] :
                                       (bytes[index + 1] << 8) | bytes[index]);
        }

        private static int Byte2Int32(IReadOnlyList<byte> bytes, int index, bool bigEndian = true)
        {
            return bigEndian ? (bytes[index] << 24) | (bytes[index + 1] << 16) | (bytes[index + 2] << 8) | bytes[index + 3]:
                               (bytes[index + 3] << 24) | (bytes[index + 2] << 16) | (bytes[index + 1] << 8) | bytes[index];
        }

        /// <summary>
        /// 将 pts 值转换为TimeSpan对象
        /// </summary>
        /// <param name="pts"></param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="pts"/> 值小于 0。</exception>
        public static TimeSpan Pts2Time(int pts)
        {
            if (pts < 0)
            {
                throw new ArgumentOutOfRangeException($"InvalidArgument=\"{pts}\"的值对于{nameof(pts)}无效");
            }
            decimal total = pts / 45000M;
            decimal secondPart = Math.Floor(total);
            decimal millisecondPart = Math.Round((total - secondPart) * 1000M, MidpointRounding.AwayFromZero);
            return new TimeSpan(0, 0, 0, (int)secondPart, (int)millisecondPart);
        }

        public ChapterInfo ToChapterInfo(int index, bool combineChapter)
        {
            if (index > ChapterClips.Count && !combineChapter)
            {
                throw new IndexOutOfRangeException("Index of Video Clip out of range");
            }
            Clip selectedClip = combineChapter ? EntireClip : ChapterClips[index];
            ChapterInfo info = new ChapterInfo
            {
                SourceType = "MPLS",
                SourceName = selectedClip.Name,
                Duration   = Pts2Time(selectedClip.Length),
                FramesPerSecond = (double) FrameRate[ChapterClips.First().Fps]
            };
            var selectedTimeStamp = selectedClip.TimeStamp;
            if (selectedTimeStamp.Count < 2) return info;
            int offset  = selectedTimeStamp.First();
            /**
             * the begin time stamp of the chapter isn't the begin of the video
             * eg: Hidan no Aria AA, There are 24 black frames at the begining of each even episode
             *     Which results that the first time stamp should be the 00:00:01.001
             */
            if (selectedClip.TimeIn < offset)
            {
                offset = ChapterClips[index].TimeIn;
                OnLog?.Invoke($"first time stamp: {selectedTimeStamp.First()}, Time in: {offset}");
            }
            var name = new ChapterName();
            info.Chapters = selectedTimeStamp.Select(item => new Chapter
            {
                Time   = Pts2Time(item - offset),
                Number = name.Index,
                Name   = name.Get()
            }).ToList();
            return info;
        }
    }

    internal static class StreamAttribute
    {
        public delegate void LogEventHandler(string message);

        public static event LogEventHandler OnLog;

        public static void LogStreamAttributes(byte[] stream, string clipName)
        {
            var streamCodingType = stream[0x0b];
            string streamCoding;
            var result = StreamCoding.TryGetValue(streamCodingType, out streamCoding);
            if (!result) streamCoding = "und";
            OnLog?.Invoke($"Stream[{clipName}] Type: {streamCoding}");
            if (0x01 != streamCodingType && 0x02 != streamCodingType &&
                0x1b != streamCodingType && 0xea != streamCodingType)
            {
                int offset = 0x90 == streamCodingType || 0x91 == streamCodingType ? 0x0c : 0x0d;
                if (0x92 == streamCodingType)
                {
                    OnLog?.Invoke($"Stream[{clipName}] CharacterCode: {CharacterCode[stream[0x0c]]}");
                }
                var language = Encoding.ASCII.GetString(stream, offset, 3);
                if (language[0] == '\0') language = "und";
                OnLog?.Invoke($"Stream[{clipName}] Language: {language}");
                if (0x0d == offset)
                {
                    int channel = stream[0x0c] >> 4;
                    int sampleRate = stream[0x0c] & 0x0f;
                    OnLog?.Invoke($"Stream[{clipName}] Channel: {Channel[channel]}");
                    OnLog?.Invoke($"Stream[{clipName}] SampleRate: {SampleRate[sampleRate]}");
                }
                return;
            }
            OnLog?.Invoke($"Stream[{clipName}] Resolution: {Resolution[stream[0x0c] >> 4]}");
            OnLog?.Invoke($"Stream[{clipName}] FrameRate: {FrameRate[stream[0x0c] & 0xf]}");
        }

        private static readonly Dictionary<int, string> StreamCoding = new Dictionary<int, string>
        {
            [0x01] = "MPEG-1 Video Stream",
            [0x02] = "MPEG-2 Video Stream",
            [0x03] = "MPEG-1 Audio Stream",
            [0x04] = "MPEG-2 Audio Stream",
            [0x06] = "HEVC Video Stream",
            [0x1b] = "MPEG-4 AVC Video Stream",
            [0xea] = "SMPTE VC-1 Video Stream",
            [0x80] = "HDMV LPCM audio stream",
            [0x81] = "Dolby Digital (AC-3) audio stream",
            [0x82] = "DTS audio stream",
            [0x83] = "Dolby Lossless audio stream",
            [0x84] = "Dolby Digital Plus audio stream",
            [0x85] = "DTS-HD audio stream except XLL",
            [0x86] = "DTS-HD audio stream XLL for Primary audio",
            [0xA1] = "Dolby digital Plus audio stream",
            [0xA2] = "DTS-HD audio stream",
            [0x90] = "Presentation Graphics Stream",
            [0x91] = "Interactive Graphics Stream",
            [0x92] = "Text Subtitle stream"
        };

        private static readonly Dictionary<int, string> Resolution = new Dictionary<int, string>
        {
            [0x00] = "res."           , [0x01] = "720*480i",
            [0x02] = "720*576i"       , [0x03] = "720*480p",
            [0x04] = "1920*1080i"     , [0x05] = "1280*720p",
            [0x06] = "1920*1080p"     , [0x07] = "720*576p"
        };

        private static readonly Dictionary<int, string> FrameRate = new Dictionary<int, string>
        {
            [0x00] = "res."           , [0x01] = "24000/1001 FPS",
            [0x02] = "24 FPS"         , [0x03] = "25 FPS",
            [0x04] = "30000/1001 FPS" , [0x05] = "res.",
            [0x06] = "50 FPS"         , [0x07] = "60000/1001 FPS"
        };

        private static readonly Dictionary<int, string> Channel = new Dictionary<int, string>
        {
            [0x00] = "res."           , [0x01] = "mono",
            [0x03] = "stereo"         , [0x06] = "multichannel",
            [0x0C] = "stereo and multichannel"
        };

        private static readonly Dictionary<int, string> SampleRate = new Dictionary<int, string>
        {
            [0x00] = "res."           , [0x01] = "48 KHz",
            [0x04] = "96 KHz"         , [0x05] = "192 KHz",
            [0x0C] = "48 & 192 KHz"   , [0x0E] = "48 & 96 KHz"
        };

        private static readonly Dictionary<int, string> CharacterCode = new Dictionary<int, string>
        {
            [0x00] = "res."           , [0x01] = "UTF-8",
            [0x02] = "UTF-16BE"       , [0x03] = "Shift-JIS",
            [0x04] = "EUC KR"         , [0x05] = "GB18030-2000",
            [0x06] = "GB2312"         , [0x07] = "BIG5"
        };
    }
}

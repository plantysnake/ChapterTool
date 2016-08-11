// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharpDvdInfo.cs" company="JT-Soft (https://github.com/UniqProject/SharpDvdInfo)">
//   This file is part of the SharpDvdInfo source code - It may be used under the terms of the GNU General Public License.
// </copyright>
// <summary>
//   Main DVD info container
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using ChapterTool.Util;

namespace SharpDvdInfo
{
    using Model;
    using System;
    using DvdTypes;
    using System.IO;
    using System.Collections;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Container for DVD Specification
    /// </summary>
    public class DvdInfoContainer
    {
        /// <summary>
        /// Length of DVD Sector
        /// </summary>
        private const int SectorLength = 2048;

        /// <summary>
        /// DVD directory
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// VMGM properties.
        /// </summary>
        public VmgmInfo Vmgm { get; set; }

        /// <summary>
        /// List of <see cref="TitleInfo"/> containing Title information.
        /// </summary>
        public List<TitleInfo> Titles { get; set; }

        /// <summary>
        /// Creates a <see cref="DvdInfoContainer"/> and reads stream infos
        /// </summary>
        /// <param name="path">DVD directory</param>
        public DvdInfoContainer(string path)
        {
            if (File.Exists(path))
            {
                _path = Directory.GetParent(path).FullName;
                Regex rTitle = new Regex(@"VTS_(\d{2})_0.IFO");
                var result = rTitle.Match(path);
                if (!result.Success)
                    throw new Exception("Invalid file");
                byte titleSetNumber = byte.Parse(result.Groups[1].Value);
                var list = new TitleInfo
                {
                    TitleNumber = titleSetNumber,
                    TitleSetNumber = titleSetNumber,
                    TitleNumberInSet = 1
                };
                GetTitleInfo(titleSetNumber, ref list);
                Titles = new List<TitleInfo> {list};
            }
            else
            {
                if (path.IndexOf("VIDEO_TS", StringComparison.Ordinal) > 0)
                {
                    _path = path;
                }
                else if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
                {
                    _path = Path.Combine(path, "VIDEO_TS");
                }
                Vmgm = new VmgmInfo();
                Titles = new List<TitleInfo>();
                GetVmgmInfo();
            }
        }

        /// <summary>
        /// fills the <see cref="TitleInfo"/> with informations
        /// </summary>
        private void GetTitleInfo(int titleSetNumber, ref TitleInfo item)
        {
            item.VideoStream = new VideoProperties();
            item.AudioStreams = new List<AudioProperties>();
            item.SubtitleStreams = new List<SubpictureProperties>();
            item.Chapters = new List<TimeSpan>();

            var buffer = new byte[192];
            using (FileStream fs = File.OpenRead(Path.Combine(_path, $"VTS_{titleSetNumber:00}_0.IFO")))
            {
                fs.Seek(0x00C8, SeekOrigin.Begin);
                fs.Read(buffer, 0, 4);
                fs.Seek(0x0200, SeekOrigin.Begin);
                fs.Read(buffer, 0, 2);

                item.VideoStream.DisplayFormat = (DvdVideoPermittedDisplayFormat)GetBits(buffer, 2, 0);
                item.VideoStream.AspectRatio   = (DvdVideoAspectRatio)GetBits(buffer, 2, 2);
                item.VideoStream.VideoStandard = (DvdVideoStandard)GetBits(buffer, 2, 4);

                switch (item.VideoStream.VideoStandard)
                {
                case DvdVideoStandard.NTSC:
                    item.VideoStream.Framerate            = 30000f / 1001f;
                    item.VideoStream.FrameRateNumerator   = 30000;
                    item.VideoStream.FrameRateDenominator = 1001;
                    break;

                case DvdVideoStandard.PAL:
                    item.VideoStream.Framerate            = 25f;
                    item.VideoStream.FrameRateNumerator   = 25;
                    item.VideoStream.FrameRateDenominator = 1;
                    break;
                }
                item.VideoStream.CodingMode      = (DvdVideoMpegVersion)GetBits(buffer, 2, 6);
                item.VideoStream.VideoResolution = (DvdVideoResolution)GetBits(buffer, 3, 11) +
                                                   ((int)item.VideoStream.VideoStandard * 8);

                fs.Read(buffer, 0, 2);
                int numAudio = GetBits(buffer, 16, 0);
                for (int audioNum = 0; audioNum < numAudio; audioNum++)
                {
                    fs.Read(buffer, 0, 8);
                    int langMode = GetBits(buffer, 2, 2);
                    int codingMode = GetBits(buffer, 3, 5);
                    AudioProperties audioStream = new AudioProperties
                    {
                        CodingMode   = (DvdAudioFormat) codingMode,
                        Channels     = GetBits(buffer, 3, 8) + 1,
                        SampleRate   = 48000,
                        Quantization = (DvdAudioQuantization) GetBits(buffer, 2, 14),
                        StreamId     = DvdAudioId.ID[codingMode] + audioNum,
                        StreamIndex  = audioNum + 1
                    };

                    if (langMode == 1)
                    {
                        char langChar1 = (char) GetBits(buffer, 8, 16);
                        char langChar2 = (char) GetBits(buffer, 8, 24);

                        audioStream.Language = ChapterTool.Util.LanguageSelectionContainer.LookupISOCode($"{langChar1}{langChar2}");
                    }
                    else
                    {
                        audioStream.Language = ChapterTool.Util.LanguageSelectionContainer.LookupISOCode("  ");
                    }
                    audioStream.Extension = (DvdAudioType)GetBits(buffer, 8, 40);
                    item.AudioStreams.Add(audioStream);
                }

                fs.Seek(0x0254, SeekOrigin.Begin);
                fs.Read(buffer, 0, 2);
                int numSubs = GetBits(buffer, 16, 0);
                for (int subNum = 0; subNum < numSubs; subNum++)
                {
                    fs.Read(buffer, 0, 6);
                    int langMode = GetBits(buffer, 2, 0);
                    SubpictureProperties sub = new SubpictureProperties
                    {
                        Format = (DvdSubpictureFormat) GetBits(buffer, 3, 5),
                        StreamId = 0x20 + subNum,
                        StreamIndex = subNum + 1
                    };

                    if (langMode == 1)
                    {
                        char langChar1 = (char)GetBits(buffer, 8, 16);
                        char langChar2 = (char)GetBits(buffer, 8, 24);

                        string langCode = langChar1.ToString(CultureInfo.InvariantCulture) +
                                              langChar2.ToString(CultureInfo.InvariantCulture);

                        sub.Language = ChapterTool.Util.LanguageSelectionContainer.LookupISOCode(langCode);
                    }
                    else
                    {
                        sub.Language = ChapterTool.Util.LanguageSelectionContainer.LookupISOCode("  ");
                    }
                    sub.Extension = (DvdSubpictureType)GetBits(buffer, 8, 40);
                    item.SubtitleStreams.Add(sub);
                }

                fs.Seek(0xCC, SeekOrigin.Begin);
                fs.Read(buffer, 0, 4);
                int pgciSector = GetBits(buffer, 32, 0);
                int pgciAdress = pgciSector * SectorLength;

                fs.Seek(pgciAdress, SeekOrigin.Begin);
                fs.Read(buffer, 0, 8);

                fs.Seek(8 * (item.TitleNumberInSet - 1), SeekOrigin.Current);
                fs.Read(buffer, 0, 8);
                int offsetPgc = GetBits(buffer, 32, 32);
                fs.Seek(pgciAdress + offsetPgc + 2, SeekOrigin.Begin);

                fs.Read(buffer, 0, 6);
                int numCells = GetBits(buffer, 8, 8);

                int hour = GetBits(buffer, 8, 16);
                int minute = GetBits(buffer, 8, 24);
                int second = GetBits(buffer, 8, 32);
                int msec = GetBits(buffer, 8, 40);

                item.VideoStream.Runtime = DvdTime2TimeSpan(hour, minute, second, msec);

                fs.Seek(224, SeekOrigin.Current);
                fs.Read(buffer, 0, 2);
                int cellmapOffset = GetBits(buffer, 16, 0);

                fs.Seek(pgciAdress + cellmapOffset + offsetPgc, SeekOrigin.Begin);

                TimeSpan chapter = new TimeSpan();
                item.Chapters.Add(chapter);

                for (int i = 0; i < numCells; i++)
                {
                    fs.Read(buffer, 0, 24);
                    int chapHour   = GetBits(buffer, 8, 4*8);
                    int chapMinute = GetBits(buffer, 8, 5*8);
                    int chapSecond = GetBits(buffer, 8, 6*8);
                    int chapMsec   = GetBits(buffer, 8, 7*8);
                    chapter = chapter.Add(DvdTime2TimeSpan(chapHour, chapMinute, chapSecond, chapMsec));

                    item.Chapters.Add(chapter);
                }
            }
        }

        /// <summary>
        /// Gets VMGM info and initializes Title list
        /// </summary>
        private void GetVmgmInfo()
        {
            byte[] buffer = new byte[12];
            using (FileStream fs = File.OpenRead(Path.Combine(_path, "VIDEO_TS.IFO")))
            {
                fs.Seek(0x20, SeekOrigin.Begin);
                fs.Read(buffer, 0, 2);
                Vmgm.MinorVersion = GetBits(buffer, 4, 8);
                Vmgm.MajorVersion = GetBits(buffer, 4, 12);

                fs.Seek(0x3E, SeekOrigin.Begin);
                fs.Read(buffer, 0, 2);
                Vmgm.NumTitleSets = GetBits(buffer, 16, 0);

                fs.Seek(0xC4, SeekOrigin.Begin);
                fs.Read(buffer, 0, 4);
                int sector = GetBits(buffer, 32, 0);
                fs.Seek(sector * SectorLength, SeekOrigin.Begin);
                fs.Read(buffer, 0, 8);
                Vmgm.NumTitles = GetBits(buffer, 16, 0);

                for (int i = 0; i < Vmgm.NumTitles; i++)
                {
                    fs.Read(buffer, 0, 12);
                    var info = new TitleInfo
                    {
                        TitleNumber = (byte) (i + 1),
                        TitleType = (byte) GetBits(buffer, 8, 0),
                        NumAngles = (byte) GetBits(buffer, 8, 1*8),
                        NumChapters = (short) GetBits(buffer, 16, 2*8),
                        ParentalMask = (short) GetBits(buffer, 16, 4*8),
                        TitleSetNumber = (byte) GetBits(buffer, 8, 6*8),
                        TitleNumberInSet = (byte) GetBits(buffer, 8, 7*8),
                        StartSector = GetBits(buffer, 32, 8*8)
                    };
                    GetTitleInfo(info.TitleNumber, ref info);
                    Titles.Add(info);
                }
            }
        }

        public List<ChapterInfo> GetChapterInfo()
        {
            var ret = new List<ChapterInfo>();

            foreach (var titleInfo in Titles)
            {
                var chapterName = ChapterName.GetChapterName("Chapter");
                var index = 1;
                var tmp = new ChapterInfo
                {
                    SourceName = $"VTS_{titleInfo.TitleSetNumber:D2}",
                    SourceType = "DVD",
                };
                foreach (var time in titleInfo.Chapters)
                {
                    tmp.Chapters.Add(new Chapter(chapterName(), time, index++));
                }
                ret.Add(tmp);
            }
            return ret;
        }


        /// <summary>
        /// Reads up to 32 bits from a byte array and outputs an integer
        /// </summary>
        /// <param name="buffer">bytearray to read from</param>
        /// <param name="length">count of bits to read from array</param>
        /// <param name="start">position to start from</param>
        /// <returns>resulting <see cref="int"/></returns>
        public static int GetBits(byte[] buffer, byte length, byte start)
        {
            int result = 0;
            //read bytes from left to right and every bit in byte from low to high
            BitArray ba = new BitArray(buffer);

            short j = 0;
            int tempResult = 0;
            for (int i = start; i < start + length; i++)
            {
                if (ba.Get(i))
                    tempResult += (1 << j);
                j++;
                if (j % 8 == 0 || j == length)
                {
                    j = 0;
                    result <<= 8;
                    result += tempResult;
                    tempResult = 0;
                }
            }

            return result;
        }

        public static int GetBits_Effi(byte[] buffer, byte length, byte start)
        {
            if (length > 8)
            {
                length = (byte)(length/8*8);
            }
            long temp = 0;
            long mask = 0xffffffffu >> (32 - length);
            // [b1] {s} [b2] {s+l} [b3]
            for (int i = 0; i < Math.Ceiling((start + length) / 8.0); ++i)
            {
                temp |= (uint)buffer[i] << (24 - i*8);
            }
            return (int)((temp >> (32 - start - length)) & mask);
        }

        /// <summary>
        /// converts bcd formatted time to milliseconds
        /// </summary>
        /// <param name="hours">hours in bcd format</param>
        /// <param name="minutes">minutes in bcd format</param>
        /// <param name="seconds">seconds in bcd format</param>
        /// <param name="milliseconds">milliseconds in bcd format (2 high bits are the frame rate)</param>
        /// <returns>Converted time in milliseconds</returns>
        private static TimeSpan DvdTime2TimeSpan(int hours, int minutes, int seconds, int milliseconds)
        {
            int fpsMask = milliseconds >> 6;
            milliseconds &= 0x3f;
            double fps = fpsMask == 0x01 ? 25D : fpsMask == 0x03 ? (30D / 1.001D) : 0;
            hours   = BcdToInt(hours);
            minutes = BcdToInt(minutes);
            seconds = BcdToInt(seconds);
            milliseconds = fps > 0 ? (int) Math.Round(BcdToInt(milliseconds)/fps*1000) : 0;
            return new TimeSpan(0 ,hours, minutes, seconds, milliseconds);
        }

        private static int BcdToInt(int value) => (0xFF & (value >> 4)) * 10 + (value & 0x0F);
    }
}
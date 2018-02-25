// ****************************************************************************
//
// Copyright (C) 2009-2015 Kurtnoise (kurtnoise@free.fr)
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChapterTool.Util.ChapterData
{
    public static class IfoData
    {
        public static IEnumerable<ChapterInfo> GetStreams(string ifoFile)
        {
            var pgcCount = IfoParser.GetPGCnb(ifoFile);
            for (var i = 1; i <= pgcCount; i++)
            {
                yield return GetChapterInfo(ifoFile, i);
            }
        }

        private static ChapterInfo GetChapterInfo(string location, int titleSetNum)
        {
            var titleRegex   = new Regex(@"^VTS_(\d+)_0\.IFO", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var result       = titleRegex.Match(location);
            if (result.Success) titleSetNum = int.Parse(result.Groups[1].Value);

            var pgc = new ChapterInfo
            {
                SourceType  = "DVD",
            };
            var fileName = Path.GetFileNameWithoutExtension(location);
            Debug.Assert(fileName != null);
            if (fileName.Count(ch => ch == '_') == 2)
            {
                var barIndex = fileName.LastIndexOf('_');
                pgc.Title = pgc.SourceName = $"{fileName.Substring(0, barIndex)}_{titleSetNum}";
            }

            pgc.Chapters        = GetChapters(location, titleSetNum, out IfoTimeSpan duration, out decimal fps);
            pgc.Duration        = duration;
            pgc.FramesPerSecond = fps;

            if (pgc.Duration.TotalSeconds < 10)
                pgc = null;

            return pgc;
        }

        private static List<Chapter> GetChapters(string ifoFile, int programChain, out IfoTimeSpan duration, out decimal fps)
        {
            var chapters = new List<Chapter>();
            duration     = IfoTimeSpan.Zero;
            fps          = 0;

            var stream = new FileStream(ifoFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            var pcgItPosition = stream.GetPCGIP_Position();
            var programChainPrograms = -1;
            var programTime   = TimeSpan.Zero;
            decimal fpsLocal;
            if (programChain >= 0)
            {
                var chainOffset      = stream.GetChainOffset(pcgItPosition, programChain);
                programTime          = stream.ReadTimeSpan(pcgItPosition, chainOffset, out fpsLocal) ?? TimeSpan.Zero;
                programChainPrograms = stream.GetNumberOfPrograms(pcgItPosition, chainOffset);
            }
            else
            {
                var programChains = stream.GetProgramChains(pcgItPosition);
                for (var curChain = 1; curChain <= programChains; curChain++)
                {
                    var chainOffset = stream.GetChainOffset(pcgItPosition, curChain);
                    var time  = stream.ReadTimeSpan(pcgItPosition, chainOffset, out fpsLocal);
                    if (time == null) break;

                    if (time.Value <= programTime) continue;
                    programChain         = curChain;
                    programChainPrograms = stream.GetNumberOfPrograms(pcgItPosition, chainOffset);
                    programTime          = time.Value;
                }
            }
            if (programChain < 0) return null;

            chapters.Add(new Chapter { Name = "Chapter 01" ,Time = TimeSpan.Zero});

            var longestChainOffset   = stream.GetChainOffset(pcgItPosition, programChain);
            int programMapOffset     = IfoParser.ToInt16(stream.GetFileBlock((pcgItPosition + longestChainOffset) + 230, 2));
            int cellTableOffset      = IfoParser.ToInt16(stream.GetFileBlock((pcgItPosition + longestChainOffset) + 0xE8, 2));
            for (var currentProgram  = 0; currentProgram < programChainPrograms; ++currentProgram)
            {
                int entryCell        = stream.GetFileBlock(((pcgItPosition + longestChainOffset) + programMapOffset) + currentProgram, 1)[0];
                var exitCell         = entryCell;
                if (currentProgram < (programChainPrograms - 1))
                    exitCell         = stream.GetFileBlock(((pcgItPosition + longestChainOffset) + programMapOffset) + (currentProgram + 1), 1)[0] - 1;

                var totalTime = IfoTimeSpan.Zero;
                for (var currentCell = entryCell; currentCell <= exitCell; currentCell++)
                {
                    var cellStart = cellTableOffset + ((currentCell - 1) * 0x18);
                    var bytes     = stream.GetFileBlock((pcgItPosition + longestChainOffset) + cellStart, 4);
                    var cellType  = bytes[0] >> 6;
                    if (cellType == 0x00 || cellType == 0x01)
                    {
                        bytes = stream.GetFileBlock(((pcgItPosition + longestChainOffset) + cellStart) + 4, 4);
                        var ret = IfoParser.ReadTimeSpan(bytes, out fps) ?? IfoTimeSpan.Zero;
                        totalTime.IsNTSC = ret.IsNTSC;
                        totalTime += ret;
                    }
                }

                duration.IsNTSC = totalTime.IsNTSC;
                duration += totalTime;
                if (currentProgram + 1 < programChainPrograms)
                    chapters.Add(new Chapter { Name = $"Chapter {currentProgram + 2:D2}", Time = duration });
            }
            stream.Dispose();
            return chapters;
        }
    }

    public struct IfoTimeSpan
    {
        public int TotalSeconds { get; set; }
        public int Frames { get; set; }
        public bool IsNTSC { get; set; }

        public int RawFrameRate => IsNTSC ? 30 : 25;
        private decimal TimeFrameRate => IsNTSC ? 30000M / 1001 : 25;

        public int TotalFrames => TotalSeconds * RawFrameRate + Frames;
        public int Hours => (int) Math.Round((TotalSeconds + Frames / TimeFrameRate) / 3600);
        public int Minutes => (int)Math.Round((TotalSeconds + Frames / TimeFrameRate) / 60) % 60;
        public int Second => (int)Math.Round(TotalSeconds + Frames / TimeFrameRate) % 60;

        public static readonly IfoTimeSpan Zero = new IfoTimeSpan(true);

        public IfoTimeSpan(bool isNTSC)
        {
            TotalSeconds = 0;
            Frames = 0;
            IsNTSC = isNTSC;
        }

        public IfoTimeSpan(int seconds, int frames, bool isNTSC)
        {
            TotalSeconds = seconds;
            Frames = frames;
            IsNTSC = isNTSC;
        }

        public IfoTimeSpan(int hour, int minute, int second, int frames, bool isNTSC)
        {
            TotalSeconds = hour * 3600 + minute * 60 + second;
            Frames = frames;
            IsNTSC = isNTSC;
        }

        public IfoTimeSpan(TimeSpan time, bool isNTSC)
        {
            IsNTSC = isNTSC;
            TotalSeconds = (int) Math.Floor(time.TotalSeconds);
            Frames = 0;
            Frames = (int) Math.Round((decimal) (time.TotalSeconds - TotalSeconds) / TimeFrameRate);
        }

        public static implicit operator TimeSpan(IfoTimeSpan time)
        {
            return new TimeSpan(
                (long) ((time.TotalSeconds*time.RawFrameRate + time.Frames) / time.TimeFrameRate * TimeSpan.TicksPerSecond));
        }

        public static IfoTimeSpan operator +(IfoTimeSpan t1, IfoTimeSpan t2)
        {
            if (t1.IsNTSC ^ t2.IsNTSC)
                throw new InvalidOperationException("Unmatch frames rate mode");
            return new IfoTimeSpan(t1.TotalSeconds + t2.TotalSeconds, t1.Frames + t2.Frames, t1.IsNTSC);
        }

        public static IfoTimeSpan operator -(IfoTimeSpan t1, IfoTimeSpan t2)
        {
            if (t1.IsNTSC ^ t2.IsNTSC)
                throw new InvalidOperationException("Unmatch frames rate mode");
            return new IfoTimeSpan(t1.TotalSeconds - t2.TotalSeconds, t1.Frames - t2.Frames, t1.IsNTSC);
        }

        public override int GetHashCode()
        {
            return (((long) TotalSeconds << 32) | ((long) Frames << 1) | (IsNTSC ? 1L : 0L)).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            var time = (IfoTimeSpan)obj;
            return TotalSeconds == time.TotalSeconds && TotalFrames == time.TotalFrames && IsNTSC == time.IsNTSC;
        }

        public override string ToString()
        {
            return $"{Hours:D2}:{Minutes:D2}:{Second:D2}.{Frames} [{(IsNTSC?'N':'P')}]";
        }
    }
}

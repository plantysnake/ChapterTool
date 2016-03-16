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
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static ChapterTool.Util.IfoParser;

namespace ChapterTool.Util
{
    public static class IfoData
    {
        public static IEnumerable<ChapterInfo> GetStreams(string ifoFile)
        {
            int pgcCount = GetPGCnb(ifoFile);
            for (int i = 1; i <= pgcCount; i++)
            {
                yield return GetChapterInfo(ifoFile, i);
            }
        }

        private static ChapterInfo GetChapterInfo(string location, int titleSetNum)
        {
            Regex titleRegex = new Regex(@"^VTS_(\d+)_0\.IFO", RegexOptions.IgnoreCase);
            var result       = titleRegex.Match(location);
            if (result.Success) titleSetNum = int.Parse(result.Groups[1].Value);

            ChapterInfo pgc = new ChapterInfo
            {
                SourceType  = "DVD",
            };
            var fileName = Path.GetFileNameWithoutExtension(location);
            Debug.Assert(fileName != null);
            if (fileName.Count(ch => ch == '_') == 2)
            {
                int barIndex = fileName.LastIndexOf('_');
                pgc.Title = pgc.SourceName = $"{fileName.Substring(0, barIndex)}_{titleSetNum}";
            }

            TimeSpan duration;
            double fps;
            pgc.Chapters        = GetChapters(location, titleSetNum, out duration, out fps);
            pgc.Duration        = duration;
            pgc.FramesPerSecond = fps;

            if (pgc.Duration.TotalSeconds < 10)
                pgc = null;

            return pgc;
        }

        private static List<Chapter> GetChapters(string ifoFile, int programChain, out TimeSpan duration, out double fps)
        {
            var chapters = new List<Chapter>();
            duration     = TimeSpan.Zero;
            fps          = 0;

            FileStream stream = new FileStream(ifoFile, FileMode.Open, FileAccess.Read, FileShare.Read);

            long pcgItPosition = stream.GetPCGIP_Position();
            int programChainPrograms = -1;
            TimeSpan programTime     = TimeSpan.Zero;
            double fpsLocal;
            if (programChain >= 0)
            {
                uint chainOffset     = stream.GetChainOffset(pcgItPosition, programChain);
                programTime          = stream.ReadTimeSpan(pcgItPosition, chainOffset, out fpsLocal) ?? TimeSpan.Zero;
                programChainPrograms = stream.GetNumberOfPrograms(pcgItPosition, chainOffset);
            }
            else
            {
                int programChains = stream.GetProgramChains(pcgItPosition);
                for (int curChain = 1; curChain <= programChains; curChain++)
                {
                    uint chainOffset = stream.GetChainOffset(pcgItPosition, curChain);
                    TimeSpan? time   = stream.ReadTimeSpan(pcgItPosition, chainOffset, out fpsLocal);
                    if (time == null) break;

                    if (time.Value <= programTime) continue;
                    programChain         = curChain;
                    programChainPrograms = stream.GetNumberOfPrograms(pcgItPosition, chainOffset);
                    programTime          = time.Value;
                }
            }
            if (programChain < 0) return null;

            chapters.Add(new Chapter { Name = "Chapter 01" ,Time = TimeSpan.Zero});

            uint longestChainOffset   = stream.GetChainOffset(pcgItPosition, programChain);
            int programMapOffset      = ToInt16(stream.GetFileBlock((pcgItPosition + longestChainOffset) + 230, 2));
            int cellTableOffset       = ToInt16(stream.GetFileBlock((pcgItPosition + longestChainOffset) + 0xE8, 2));
            for (int currentProgram   = 0; currentProgram < programChainPrograms; ++currentProgram)
            {
                int entryCell         = stream.GetFileBlock(((pcgItPosition + longestChainOffset) + programMapOffset) + currentProgram, 1)[0];
                int exitCell          = entryCell;
                if (currentProgram < (programChainPrograms - 1))
                    exitCell          = stream.GetFileBlock(((pcgItPosition + longestChainOffset) + programMapOffset) + (currentProgram + 1), 1)[0] - 1;

                TimeSpan totalTime    = TimeSpan.Zero;
                for (int currentCell  = entryCell; currentCell <= exitCell; currentCell++)
                {
                    int cellStart     = cellTableOffset + ((currentCell - 1) * 0x18);
                    byte[] bytes      = stream.GetFileBlock((pcgItPosition + longestChainOffset) + cellStart, 4);
                    int cellType      = bytes[0] >> 6;
                    if (cellType == 0x00 || cellType == 0x01)
                    {
                        bytes = stream.GetFileBlock(((pcgItPosition + longestChainOffset) + cellStart) + 4, 4);
                        TimeSpan time = ReadTimeSpan(bytes, out fps) ?? TimeSpan.Zero;
                        totalTime    += time;
                    }
                }

                //add a constant amount of time for each chapter?
                //totalTime += TimeSpan.FromMilliseconds(fps != 0 ? (double)1000 / fps / 8D : 0);

                duration += totalTime;
                if (currentProgram + 1 < programChainPrograms)
                    chapters.Add(new Chapter { Name = $"Chapter {currentProgram + 2:D2}", Time = duration });
            }
            stream.Dispose();
            return chapters;
        }
    }
}

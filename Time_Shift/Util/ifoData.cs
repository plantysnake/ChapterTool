using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace ChapterTool.Util
{
    public class IfoData
    {
        public class ProgramChainArg : EventArgs
        {
            public ChapterInfo ProgramChain { get; set; }
        }

        public event EventHandler<ProgramChainArg> StreamDetected;
        public event EventHandler ExtractionComplete;


        public List<ChapterInfo> GetStreams(string ifoFile)
        {
            List<ChapterInfo> oList = new List<ChapterInfo>();

            int pgcCount = (int)IfoParser.GetPGCnb(ifoFile);
            for (int i = 1; i <= pgcCount; i++)
            {
                var oChapterInfo = GetChapterInfo(ifoFile, i);
                oList.Add(oChapterInfo);
            }
            return oList;
        }

        protected void OnStreamDetected(ChapterInfo pgc)
        {
            if (StreamDetected == null) return;
            ProgramChainArg e = new ProgramChainArg
            {
                ProgramChain = pgc
            };
            StreamDetected(this, e);
        }
        protected void OnExtractionComplete()
        {
            ExtractionComplete?.Invoke(this, EventArgs.Empty);
        }

        public ChapterInfo GetChapterInfo(string location, int titleSetNum)
        {
            if (location.StartsWith("VTS_"))
            {
                titleSetNum = int.Parse(Path.GetFileNameWithoutExtension(location)
                .ToUpper(System.Globalization.CultureInfo.InvariantCulture)
                .Replace("VTS_", string.Empty)
                .Replace("_0.IFO", string.Empty));
            }

            ChapterInfo pgc = new ChapterInfo
            {
                SourceType  = "DVD",
                SourceName  = $"PGC {titleSetNum:D2}",
                TitleNumber = titleSetNum,
                Title       = Path.GetFileNameWithoutExtension(location)
            };
            if (pgc.Title.Split('_').Length == 3)
                pgc.Title = $"{pgc.Title.Split('_')[0]}_{pgc.Title.Split('_')[1]}";

            TimeSpan duration;
            double fps;
            pgc.Chapters        = GetChapters(location, titleSetNum, out duration, out fps);
            pgc.Duration        = duration;
            pgc.FramesPerSecond = fps;

            if (pgc.Duration.TotalSeconds > 10)
                OnStreamDetected(pgc);
            else
                pgc = null;
            OnExtractionComplete();

            return pgc;
        }

        private static List<Chapter> GetChapters(string ifoFile, int programChain, out TimeSpan duration, out double fps)
        {
            var chapters = new List<Chapter>();
            duration     = TimeSpan.Zero;
            fps          = 0;

            long pcgItPosition       = IfoParser.GetPCGIP_Position(ifoFile);
            int programChainPrograms = -1;
            TimeSpan programTime     = TimeSpan.Zero;
            double fpsLocal;
            if (programChain >= 0)
            {
                uint chainOffset     = IfoParser.GetChainOffset(ifoFile, pcgItPosition, programChain);
                programTime          = IfoParser.ReadTimeSpan(ifoFile, pcgItPosition, chainOffset, out fpsLocal) ?? TimeSpan.Zero;
                programChainPrograms = IfoParser.GetNumberOfPrograms(ifoFile, pcgItPosition, chainOffset);
            }
            else
            {
                int programChains = IfoParser.GetProgramChains(ifoFile, pcgItPosition);
                for (int curChain = 1; curChain <= programChains; curChain++)
                {
                    uint chainOffset = IfoParser.GetChainOffset(ifoFile, pcgItPosition, curChain);
                    TimeSpan? time   = IfoParser.ReadTimeSpan(ifoFile, pcgItPosition, chainOffset, out fpsLocal);
                    if (time == null) break;

                    if (time.Value <= programTime) continue;
                    programChain         = curChain;
                    programChainPrograms = IfoParser.GetNumberOfPrograms(ifoFile, pcgItPosition, chainOffset);
                    programTime          = time.Value;
                }
            }
            if (programChain < 0) return null;

            chapters.Add(new Chapter() { Name = "Chapter 01" });

            uint longestChainOffset   = IfoParser.GetChainOffset(ifoFile, pcgItPosition, programChain);
            int programMapOffset      = IfoParser.ToInt16(IfoParser.GetFileBlock(ifoFile, (pcgItPosition + longestChainOffset) + 230, 2));
            int cellTableOffset       = IfoParser.ToInt16(IfoParser.GetFileBlock(ifoFile, (pcgItPosition + longestChainOffset) + 0xE8, 2));
            for (int currentProgram   = 0; currentProgram < programChainPrograms; ++currentProgram)
            {
                int entryCell         = IfoParser.GetFileBlock(ifoFile, ((pcgItPosition + longestChainOffset) + programMapOffset) + currentProgram, 1)[0];
                int exitCell          = entryCell;
                if (currentProgram < (programChainPrograms - 1))
                    exitCell          = IfoParser.GetFileBlock(ifoFile, ((pcgItPosition + longestChainOffset) + programMapOffset) + (currentProgram + 1), 1)[0] - 1;

                TimeSpan totalTime    = TimeSpan.Zero;
                for (int currentCell  = entryCell; currentCell <= exitCell; currentCell++)
                {
                    int cellStart     = cellTableOffset + ((currentCell - 1) * 0x18);
                    byte[] bytes      = IfoParser.GetFileBlock(ifoFile, (pcgItPosition + longestChainOffset) + cellStart, 4);
                    int cellType      = bytes[0] >> 6;
                    if (cellType      == 0x00 || cellType == 0x01)
                    {
                        bytes         = IfoParser.GetFileBlock(ifoFile, ((pcgItPosition + longestChainOffset) + cellStart) + 4, 4);
                        TimeSpan time = IfoParser.ReadTimeSpan(bytes, out fps) ?? TimeSpan.Zero;
                        totalTime    += time;
                    }
                }

                //add a constant amount of time for each chapter?
                //totalTime += TimeSpan.FromMilliseconds(fps != 0 ? (double)1000 / fps / 8D : 0);

                duration += totalTime;
                if (currentProgram + 1 < programChainPrograms)
                    chapters.Add(new Chapter { Name = $"Chapter {currentProgram + 2:D2}", Time = duration });
            }
            return chapters;
        }
    }
}

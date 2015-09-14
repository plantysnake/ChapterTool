using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;


namespace ChapterTool.Util
{
    public class ifoData
    {
        public class ProgramChainArg : EventArgs
        {
            public ChapterInfo ProgramChain { get; set; }
        }

        public event EventHandler<ProgramChainArg> StreamDetected;
        public event EventHandler ExtractionComplete;

        public static string ComputeMD5Sum(string path)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
        }


        public List<ChapterInfo> GetStreams(string ifoFile)
        {
            List<ChapterInfo> oList = new List<ChapterInfo>();
            ChapterInfo oChapterInfo;

            int pgcCount = (int)ifoParser.getPGCnb(ifoFile);
            for (int i = 1; i <= pgcCount; i++)
            {
                oChapterInfo = GetChapterInfo(ifoFile, i);
                oList.Add(oChapterInfo);
            }
            return oList;
        }

        protected void OnStreamDetected(ChapterInfo pgc)
        {
            if (this.StreamDetected != null)
            {
                ProgramChainArg e = new ProgramChainArg
                {
                    ProgramChain = pgc
                };
                this.StreamDetected(this, e);
            }
        }
        protected void OnExtractionComplete()
        {
            if (this.ExtractionComplete != null)
            {
                this.ExtractionComplete(this, EventArgs.Empty);
            }
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

            ChapterInfo pgc = new ChapterInfo();
            pgc.SourceType = "DVD";
            pgc.SourceName = "PGC " + titleSetNum.ToString("D2");
            pgc.TitleNumber = titleSetNum;
            pgc.SourceHash = ComputeMD5Sum(location);
            pgc.Title = Path.GetFileNameWithoutExtension(location);
            if (pgc.Title.Split('_').Length == 3)
                pgc.Title = pgc.Title.Split('_')[0] + "_" + pgc.Title.Split('_')[1];

            TimeSpan duration;
            double fps;
            pgc.Chapters = GetChapters(location, titleSetNum, out duration, out fps);
            pgc.Duration = duration;
            pgc.FramesPerSecond = fps;
            
            if (pgc.Duration.TotalSeconds > 900)
                OnStreamDetected(pgc);
            else
                pgc = null;
            OnExtractionComplete();
            
            return pgc;
        }

        List<Chapter> GetChapters(string ifoFile, int programChain, out TimeSpan duration, out double fps)
        {
            List<Chapter> chapters = new List<Chapter>();
            duration = TimeSpan.Zero;
            fps = 0;

            long pcgITPosition = ifoParser.GetPCGIP_Position(ifoFile);
            int programChainPrograms = -1;
            TimeSpan programTime = TimeSpan.Zero;
            if (programChain >= 0)
            {
                double FPS;
                uint chainOffset = ifoParser.GetChainOffset(ifoFile, pcgITPosition, programChain);
                programTime = ifoParser.ReadTimeSpan(ifoFile, pcgITPosition, chainOffset, out FPS) ?? TimeSpan.Zero;
                programChainPrograms = ifoParser.GetNumberOfPrograms(ifoFile, pcgITPosition, chainOffset);
            }
            else
            {
                int programChains = ifoParser.GetProgramChains(ifoFile, pcgITPosition);
                for (int curChain = 1; curChain <= programChains; curChain++)
                {
                    double FPS;
                    uint chainOffset = ifoParser.GetChainOffset(ifoFile, pcgITPosition, curChain);
                    TimeSpan? time = ifoParser.ReadTimeSpan(ifoFile, pcgITPosition, chainOffset, out FPS);
                    if (time == null)
                        break;

                    if (time.Value > programTime)
                    {
                        programChain = curChain;
                        programChainPrograms = ifoParser.GetNumberOfPrograms(ifoFile, pcgITPosition, chainOffset);
                        programTime = time.Value;
                    }
                }
            }
            if (programChain < 0)
                return null;

            chapters.Add(new Chapter() { Name = "Chapter 01" });

            uint longestChainOffset = ifoParser.GetChainOffset(ifoFile, pcgITPosition, programChain);
            int programMapOffset = ifoParser.ToInt16(ifoParser.GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + 230, 2));
            int cellTableOffset = ifoParser.ToInt16(ifoParser.GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + 0xE8, 2));
            for (int currentProgram = 0; currentProgram < programChainPrograms; ++currentProgram)
            {
                int entryCell = ifoParser.GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + programMapOffset) + currentProgram, 1)[0];
                int exitCell = entryCell;
                if (currentProgram < (programChainPrograms - 1))
                    exitCell = ifoParser.GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + programMapOffset) + (currentProgram + 1), 1)[0] - 1;

                TimeSpan totalTime = TimeSpan.Zero;
                for (int currentCell = entryCell; currentCell <= exitCell; currentCell++)
                {
                    int cellStart = cellTableOffset + ((currentCell - 1) * 0x18);
                    byte[] bytes = ifoParser.GetFileBlock(ifoFile, (pcgITPosition + longestChainOffset) + cellStart, 4);
                    int cellType = bytes[0] >> 6;
                    if (cellType == 0x00 || cellType == 0x01)
                    {
                        bytes = ifoParser.GetFileBlock(ifoFile, ((pcgITPosition + longestChainOffset) + cellStart) + 4, 4);
                        TimeSpan time = ifoParser.ReadTimeSpan(bytes, out fps) ?? TimeSpan.Zero;
                        totalTime += time;
                    }
                }

                //add a constant amount of time for each chapter?
                //totalTime += TimeSpan.FromMilliseconds(fps != 0 ? (double)1000 / fps / 8D : 0);

                duration += totalTime;
                if (currentProgram + 1 < programChainPrograms)
                    chapters.Add(new Chapter() { Name = string.Format("Chapter {0:D2}", currentProgram + 2), Time = duration });
            }
            return chapters;
        }
    }
}

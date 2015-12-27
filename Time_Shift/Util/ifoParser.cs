// ****************************************************************************
//
// Copyright (C) 2009-2015 Kurtnoise (kurtnoise@free.fr)
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
using System.Collections;
using System.Windows.Forms;

namespace ChapterTool.Util
{
    public sealed class IfoParser
    {
        internal static byte[] GetFileBlock(string strFile, long pos, int count)
        {
            if (pos < 0)
            {
                throw new Exception("Invalid Ifo file");
            }
            using (FileStream stream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buf = new byte[count];
                stream.Seek(pos, SeekOrigin.Begin);
                stream.Read(buf, 0, count);
                return buf;
            }
        }

        internal static short ToInt16(byte[] bytes) { return (short)((bytes[0] << 8) + bytes[1]); }
        private static  uint  ToInt32(byte[] bytes) { return (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3]); }
        private static long ToFilePosition(byte[] bytes) { return ToInt32(bytes) * 0x800L; }

        private static long GetTotalFrames(TimeSpan time, int fps)
        {
            return (long)Math.Round(fps * time.TotalSeconds);
        }

        private static string TwoLong(int val) { return $"{val:D2}"; }

        private static int AsHex(int val)
        {
            int ret;
            int.TryParse($"{val:X2}", out ret);
            return ret;
        }

        private static short? GetFrames(byte val)
        {
            int byte0High = val >> 4;
            int byte0Low = val & 0x0F;
            if (byte0High > 11)
                return (short)(((byte0High - 12) * 10) + byte0Low);
            if ((byte0High <= 3) || (byte0High >= 8))
                return null;
            return (short)(((byte0High - 4) * 10) + byte0Low);
        }

        private static int GetFrames(TimeSpan time, int fps)
        {
            return (int)Math.Round(fps * time.Milliseconds / 1000.0);
        }

        internal static long GetPCGIP_Position(string ifoFile)
        {
            return ToFilePosition(GetFileBlock(ifoFile, 0xCC, 4));
        }

        internal static int GetProgramChains(string ifoFile, long pcgitPosition)
        {
            return ToInt16(GetFileBlock(ifoFile, pcgitPosition, 2));
        }

        internal static uint GetChainOffset(string ifoFile, long pcgitPosition, int programChain)
        {
            return ToInt32(GetFileBlock(ifoFile, (pcgitPosition + (8 * programChain)) + 4, 4));
        }

        internal static int GetNumberOfPrograms(string ifoFile, long pcgitPosition, uint chainOffset)
        {
            return GetFileBlock(ifoFile, (pcgitPosition + chainOffset) + 2, 1)[0];
        }

        internal static TimeSpan? ReadTimeSpan(string ifoFile, long pcgitPosition, uint chainOffset, out double fps)
        {
            return ReadTimeSpan(GetFileBlock(ifoFile, (pcgitPosition + chainOffset) + 4, 4), out fps);
        }

        internal static TimeSpan? ReadTimeSpan(byte[] playbackBytes, out double fps)
        {
            short? frames = GetFrames(playbackBytes[3]);
            int fpsMask = playbackBytes[3] >> 6;
            fps = fpsMask == 0x01 ? 25D : fpsMask == 0x03 ? (30D / 1.001D) : 0;
            if (frames == null)
                return null;

            try
            {
                int hours = AsHex(playbackBytes[0]);
                int minutes = AsHex(playbackBytes[1]);
                int seconds = AsHex(playbackBytes[2]);
                TimeSpan ret = new TimeSpan(hours, minutes, seconds);
                if (Math.Abs(fps) > 1e-5)
                    ret = ret.Add(TimeSpan.FromSeconds((double)frames / fps));
                return ret;
            }
            catch { return null; }
        }

        /// <summary>
        /// get number of PGCs
        /// </summary>
        /// <param name="fileName">name of the IFO file</param>
        /// <returns>number of PGS as unsigned integer</returns>
        public static uint GetPGCnb(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Stream sr = br.BaseStream;

            sr.Seek(0xCC, SeekOrigin.Begin);
            uint buf = ReadUInt32(br);									// Read PGC offset
            sr.Seek(2048 * buf + 0x1, SeekOrigin.Begin);			// Move to beginning of PGC
            //long VTS_PGCITI_start_position = sr.Position - 1;
            byte nPGCs = br.ReadByte();									// Number of PGCs
            fs.Close();

            return nPGCs;
        }

        private static uint ReadUInt32(BinaryReader br)
        {
            uint val = (
                ((uint)br.ReadByte()) << 24 |
                ((uint)br.ReadByte()) << 16 |
                ((uint)br.ReadByte()) << 8 |
                ((uint)br.ReadByte()));
            return val;
        }
    }
}

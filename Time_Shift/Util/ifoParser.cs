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

namespace ChapterTool.Util
{
    public static class IfoParser
    {
        internal static byte[] GetFileBlock(this FileStream ifoStream, long position, int count)
        {
            if (position < 0) throw new Exception("Invalid Ifo file");
            var buf = new byte[count];
            ifoStream.Seek(position, SeekOrigin.Begin);
            ifoStream.Read(buf, 0, count);
            return buf;
        }

        private static byte? GetFrames(byte value)
        {
            if (((value >> 6) & 0x01) == 1) //check whether the second bit of value is 1
            {
                return (byte)BcdToInt((byte)(value & 0x3F)); //only last 6 bits is in use, show as BCD code
            }
            return null;
        }

        internal static long GetPCGIP_Position(this FileStream ifoStream)
        {
            return ToFilePosition(ifoStream.GetFileBlock(0xCC, 4));
        }

        internal static int GetProgramChains(this FileStream ifoStream, long pcgitPosition)
        {
            return ToInt16(ifoStream.GetFileBlock(pcgitPosition, 2));
        }

        internal static uint GetChainOffset(this FileStream ifoStream, long pcgitPosition, int programChain)
        {
            return ToInt32(ifoStream.GetFileBlock((pcgitPosition + (8 * programChain)) + 4, 4));
        }

        internal static int GetNumberOfPrograms(this FileStream ifoStream, long pcgitPosition, uint chainOffset)
        {
            return ifoStream.GetFileBlock((pcgitPosition + chainOffset) + 2, 1)[0];
        }

        internal static TimeSpan? ReadTimeSpan(this FileStream ifoStream, long pcgitPosition, uint chainOffset, out double fps)
        {
            return ReadTimeSpan(ifoStream.GetFileBlock((pcgitPosition + chainOffset) + 4, 4), out fps);
        }

        internal static TimeSpan? ReadTimeSpan(byte[] playbackBytes, out double fps)
        {
            var frames    = GetFrames(playbackBytes[3]);
            int fpsMask   = playbackBytes[3] >> 6;
            fps = fpsMask == 0x01 ? 25D : fpsMask == 0x03 ? (30D / 1.001D) : 0;
            if (frames == null) return null;
            try
            {
                int hours    = BcdToInt(playbackBytes[0]);
                int minutes  = BcdToInt(playbackBytes[1]);
                int seconds  = BcdToInt(playbackBytes[2]);
                TimeSpan ret = new TimeSpan(hours, minutes, seconds);
                if (Math.Abs(fps) > 1e-5)
                    ret += TimeSpan.FromSeconds((double) frames/fps);
                return ret;
            }
            catch { return null; }
        }

        /// <summary>
        /// get number of PGCs
        /// </summary>
        /// <param name="fileName">name of the IFO file</param>
        /// <returns>number of PGS as an integer</returns>
        public static int GetPGCnb(string fileName)
        {
            FileStream ifoStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            uint offset          = ToInt32(GetFileBlock(ifoStream, 0xCC, 4));    // Read PGC offset
            ifoStream.Seek(2048 * offset + 0x1, SeekOrigin.Begin);               // Move to beginning of PGC
            //long VTS_PGCITI_start_position = ifoStream.Position - 1;
            int nPGCs = ifoStream.ReadByte();       // Number of PGCs
            ifoStream.Close();
            return nPGCs;
        }

        internal static short ToInt16(byte[] bytes) => (short)((bytes[0] << 8) + bytes[1]);
        private static uint ToInt32(byte[] bytes)   => (uint)((bytes[0] << 24) + (bytes[1] << 16) + (bytes[2] << 8) + bytes[3]);
        public static int BcdToInt(byte value)      => (0xFF & (value >> 4)) * 10 + (value & 0x0F);

        private static long ToFilePosition(byte[] bytes) => ToInt32(bytes) * 0x800L;
    }
}

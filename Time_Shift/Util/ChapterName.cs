// ****************************************************************************
//
// Copyright (C) 2014-2015 TautCony (TautCony@vcb-s.com)
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

namespace ChapterTool.Util
{
    public class ChapterName
    {
        public int Index { get; private set; }

        public ChapterName(int index)
        {
            Index = index;
        }

        public ChapterName()
        {
            Index = 1;
        }

        public void Reset()
        {
            Index = 1;
        }

        public string Get()
        {
            return $"Chapter {Index++:D2}";
        }

        /// <summary>
        /// 生成指定范围内的标准章节名的序列
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<string> Range(int start, int count)
        {
            if (start < 0 || start > 99) throw new ArgumentOutOfRangeException(nameof(start));
            var max = start + count - 1;
            if (count < 0 || max > 99) throw new ArgumentOutOfRangeException(nameof(count));
            return RangeIterator(start, count);
        }

        private static IEnumerable<string> RangeIterator(int start, int count)
        {
            for (int i = 0; i < count; i++) yield return $"Chapter {start + i:D2}";
        }
    }
}
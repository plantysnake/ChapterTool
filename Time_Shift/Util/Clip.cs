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
using System.Collections.Generic;

namespace ChapterTool.Util
{
    public class Clip
    {
        public string Name         { get; set; }
        public List<int> TimeStamp { get; } = new List<int>();
        public int Fps             { get; set; }
        public int Length          { get; set; }
        public int RelativeTimeIn  { get; set; }
        public int RelativeTimeOut { get; set; }
        public int TimeIn          { get; set; }
        public int TimeOut         { get; set; }
        public override string ToString() => $"{Name} - {ConvertMethod.Pts2Time(Length)}";
    }
}
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
#include <iostream>
#include <vector>
#include <string>
#include <fstream>

namespace ChapterTool
{
const double FrameRate[8] = { 0, 24000.0 / 1001, 24, 25, 30000.0 / 1001, 0, 50, 60000.0 / 1001 };

struct ChapterUnit
{
    size_t time;//in milliseconds
    size_t frames;
    std::string chapterName;
};

struct ChapterInfo
{
    std::vector<struct ChapterUnit> TimeStamp;
    std::string VideoName;
};

class Clip
{
public:
    std::string Name;
    int Fps;
    int Length;
    int RelativeTimeIn;
    int RelativeTimeOut;
    int TimeIn;
    int TimeOut;
    std::vector<int> TimeStamp;

    Clip() { init(); };
    Clip(char *name) { Name = std::string(name); init(); }
    Clip(std::string name) { Name = name; init(); }
    void init() { Length = RelativeTimeIn = RelativeTimeOut = TimeIn = TimeOut = 0; }
    struct ChapterInfo ToChapterInfo();
private:
    std::string _getChapterName(int index)
    {
        char buff[100];
        snprintf(buff, sizeof(buff), "Chapter %02d", index);
        return std::string(buff);
    }
};

struct ChapterInfo Clip::ToChapterInfo()
{
    struct ChapterInfo ci;
    ci.VideoName = Name;
    int index = 1;
    for (const int& stamp : TimeStamp)
    {
        struct ChapterUnit cu;
        cu.time = (stamp - TimeIn) / 45;
        cu.frames = (size_t)round((stamp - TimeIn) / 45000.0 * FrameRate[Fps]);
        cu.chapterName = _getChapterName(index++);
        ci.TimeStamp.push_back(cu);
    }
    return ci;
}

class MplsChapter
{
private:
    std::string _mplsPath;
    std::vector<Clip> _chapterClips;
    Clip _entireClip;
    std::vector<unsigned char> _data;
    void ParseMpls();
    void ParseHeader(int& playlistMarkSectionStartAddress, int& playItemNumber, int& playItemEntries);
    void ParsePlayItem(int playItemEntries, int& lengthOfPlayItem, int& itemStartAdress, int& streamCount);
    void ParseStream(int itemStartAdress, int streamOrder, int playItemOrder);
    void ParsePlaylistMark(int playlistMarkSectionStartAddress);

    static short Byte2Int16(const std::vector<unsigned char> &bytes, int index, bool bigEndian = true);
    static int   Byte2Int32(const std::vector<unsigned char> &bytes, int index, bool bigEndian = true);
    static void ReadAllBytes(const std::string &filename, std::vector<unsigned char>& result);
    static std::string GetString(const std::vector<unsigned char> &data, int index, int length);
    static bool IsEmptyOrWhiteSpace(const std::string &value);
public:
    MplsChapter() {};
    MplsChapter(const std::string &path);
    MplsChapter(const char *path);
    void Parse(const std::string &path);
    void Parse(const char *path);
    std::vector<Clip> &ChapterClips() { return _chapterClips; }
    Clip &EntireClip() { return _entireClip; }

    static double FrameRate[8];
};

MplsChapter::MplsChapter(const std::string &path)
{
    _mplsPath = path;
    ParseMpls();
}

MplsChapter::MplsChapter(const char *path)
{
    _mplsPath = std::string(path);
    ParseMpls();
}

void MplsChapter::Parse(const std::string &path)
{
    _mplsPath = path;
    ParseMpls();
}

void MplsChapter::Parse(const char *path)
{
    _mplsPath = std::string(path);
    ParseMpls();
}

void MplsChapter::ParseMpls()
{
    if (IsEmptyOrWhiteSpace(_mplsPath))
    {
        throw "Empty File Path";
    }
    ReadAllBytes(_mplsPath, _data);
    int playlistMarkSectionStartAddress, playItemNumber, playItemEntries;
    ParseHeader(playlistMarkSectionStartAddress, playItemNumber, playItemEntries);
    for (int playItemOrder = 0; playItemOrder < playItemNumber; playItemOrder++)
    {
        int lengthOfPlayItem, itemStartAdress, streamCount;
        ParsePlayItem(playItemEntries, lengthOfPlayItem, itemStartAdress, streamCount);
        for (int streamOrder = 0; streamOrder < streamCount; streamOrder++)
        {
            ParseStream(itemStartAdress, streamOrder, playItemOrder);
        }
        playItemEntries += lengthOfPlayItem + 2;//for that not counting the two length bytes themselves.
    }
    ParsePlaylistMark(playlistMarkSectionStartAddress);
    _entireClip.Name = "FULL Chapter";
    _entireClip.TimeIn = 0;
    for (const auto &item : _chapterClips)
    {
        _entireClip.TimeOut += item.Length;
    }
    _entireClip.Length = _entireClip.TimeOut;
}

void MplsChapter::ParseHeader(int &playlistMarkSectionStartAddress, int &playItemNumber, int &playItemEntries)
{
    std::string fileType = GetString(_data, 0, 8);
    if ((fileType != "MPLS0100" && fileType != "MPLS0200") /*|| _data[45] != 1*/)
    {
        throw ("This Playlist has an unknown file type " + fileType);
    }
    int playlistSectionStartAddress = Byte2Int32(_data, 0x08);
    playlistMarkSectionStartAddress = Byte2Int32(_data, 0x0c);
    playItemNumber = Byte2Int16(_data, playlistSectionStartAddress + 0x06);
    playItemEntries = playlistSectionStartAddress + 0x0a;
}

void MplsChapter::ParsePlayItem(int playItemEntries, int &lengthOfPlayItem, int &itemStartAdress, int &streamCount)
{
    lengthOfPlayItem = Byte2Int16(_data, playItemEntries);
    auto cpyBegin = _data.begin() + playItemEntries;
    auto bytes = std::vector<unsigned char>(cpyBegin, cpyBegin + lengthOfPlayItem);
    Clip streamClip = Clip();
    streamClip.TimeIn = Byte2Int32(bytes, 0x0e);
    streamClip.TimeOut = Byte2Int32(bytes, 0x12);

    streamClip.Length = streamClip.TimeOut - streamClip.TimeIn;
    for (const auto &item : _chapterClips)
    {
        streamClip.RelativeTimeIn += item.Length;
    }
    streamClip.RelativeTimeOut = streamClip.RelativeTimeIn + streamClip.Length;
    itemStartAdress = playItemEntries + 0x32;
    streamCount = bytes[0x23] >> 4;
    int isMultiAngle = (bytes[0x0c] >> 4) & 0x01;
    std::string nameBuilder = GetString(bytes, 0x02, 0x05);

    if (isMultiAngle == 1)  //skip multi-angle
    {
        int numberOfAngles = bytes[0x22];
        for (int i = 1; i < numberOfAngles; i++)
        {
            nameBuilder += ("&" + GetString(bytes, 0x24 + (i - 1) * 0x0a, 0x05));
        }
        itemStartAdress = playItemEntries + 0x02 + (numberOfAngles - 1) * 0x0a;
    }
    streamClip.Name = nameBuilder;
    _chapterClips.push_back(streamClip);
}

void MplsChapter::ParseStream(int itemStartAdress, int streamOrder, int playItemOrder)
{
    auto cpyBegin = _data.begin() + itemStartAdress + streamOrder * 16;
    auto stream = std::vector<unsigned char>(cpyBegin, cpyBegin + 16);
    if (0x01 != stream[01]) return; //make sure this stream is Play Item
    auto streamCodingType = stream[0x0b];
    auto &chapterClip = _chapterClips[playItemOrder];
    if (0x1b != streamCodingType && 0x02 != streamCodingType &&
        0xea != streamCodingType && 0x06 != streamCodingType) return;
    chapterClip.Fps = stream[0x0c] & 0xf;//last 4 bits is the fps
}

void MplsChapter::ParsePlaylistMark(int playlistMarkSectionStartAddress)
{
    int playlistMarkNumber = Byte2Int16(_data, playlistMarkSectionStartAddress + 0x04);
    int playlistMarkEntries = playlistMarkSectionStartAddress + 0x06;
    // eg. 0001 yyyy xxxxxxxx FFFF 000000
    // 00       mark_id
    // 01       mark_type
    // 02 - 03  play_item_ref
    // 04 - 07  time
    // 08 - 09  entry_es_pid
    // 10 - 13  duration
    for (int mark = 0; mark < playlistMarkNumber; ++mark)
    {
        auto cpyBegin = _data.begin() + playlistMarkEntries + mark * 14;
        auto bytelist = std::vector<unsigned char>(cpyBegin, cpyBegin + 14);
        if (0x01 != bytelist[1]) continue;// make sure the playlist mark type is an entry mark
        int streamFileIndex = Byte2Int16(bytelist, 0x02);
        Clip &streamClip = _chapterClips[streamFileIndex];
        int timeStamp = Byte2Int32(bytelist, 0x04);
        int relativeSeconds = timeStamp - streamClip.TimeIn + streamClip.RelativeTimeIn;
        streamClip.TimeStamp.push_back(timeStamp);
        _entireClip.TimeStamp.push_back(relativeSeconds);
    }
}

void MplsChapter::ReadAllBytes(const std::string &filename, std::vector<unsigned char>& result)
{
    std::ifstream stream(filename, std::ios::in | std::ios::binary);
    result = std::vector<unsigned char>(std::istreambuf_iterator<char>(stream),
        std::istreambuf_iterator<char>());
}

short MplsChapter::Byte2Int16(const std::vector<unsigned char> &bytes, int index, bool bigEndian)
{
    return (short)(bigEndian ? (bytes[index] << 8) | bytes[index + 1] :
        (bytes[index + 1] << 8) | bytes[index]);
}

int MplsChapter::Byte2Int32(const std::vector<unsigned char> &bytes, int index, bool bigEndian)
{
    return bigEndian ? (bytes[index] << 24) | (bytes[index + 1] << 16) | (bytes[index + 2] << 8) | bytes[index + 3] :
        (bytes[index + 3] << 24) | (bytes[index + 2] << 16) | (bytes[index + 1] << 8) | bytes[index];
}

std::string MplsChapter::GetString(const std::vector<unsigned char> &data, int index, int length)
{
    std::string res = "";
    for (int i = index; i < index + length; ++i)
    {
        res.push_back(data[i]);
    }
    return res;
}

bool MplsChapter::IsEmptyOrWhiteSpace(const std::string &value)
{
    if (value == "")
    {
        return true;
    }
    for (const auto &c : value)
    {
        if (!((c == ' ') || (c >= '\x0009' && c <= '\x000d') || c == '\x00a0' || c == '\x0085'))
        {
            return false;
        }
    }
    return true;
}
}

int main()
{
    std::string path = "D:\\Code\\@ChapterTool\\Sample\\MPLS\\00001_Hidan_no_Aria_AA.mpls";
    //std::cin >> path;
    auto res = ChapterTool::MplsChapter(path);
    auto entire = res.EntireClip();
    std::cout << entire.Name << " " << entire.TimeStamp.size() << std::endl;
    for (const auto &item : entire.TimeStamp)
    {
        std::cout << item << " ";
    }
    std::cout << std::endl;
    auto ct = res.ChapterClips();
    for (auto &clip : ct)
    {
        //std::cout << clip.Name << " " << clip.TimeStamp.size() << " " << clip.Fps << std::endl;
        auto ci = clip.ToChapterInfo();
        /*
        for (auto &item : clip.TimeStamp)
        {
            std::cout << item << " ";
        }
        */
        std::cout << std::endl;
        std::cout << "---------" << std::endl;
        std::cout << ci.VideoName << ".m2ts" << std::endl;
        for (auto &i : ci.TimeStamp)
        {
            std::cout << i.chapterName << " " << i.time << " " << i.frames << std::endl;
        }
    }
    return 0;
}

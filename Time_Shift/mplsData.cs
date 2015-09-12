using System;
using System.IO;
using System.Collections.Generic;

namespace ChapterTool
{

    public class mplsData
    {
        public List<string> fileName;                //the file name for each file
        public List<KeyValuePair<int, int>> lastTime;//the start time and the end time for each file
        public List<int> fps;                        //the fps code for each file
        public List<List<int>> timeStamp;            //the chapters for each file

        private byte[] data;
        private int PlaylistSectionStartAddress;
        private int PlaylistMarkSectionStartAddress;
        private int PlatItemNumber;
        private int PlayItemEntries;

        public mplsData(string path)
        {
            initializeClass();
            loadFile(path);
            parseHeader();
            int shift = 0;
            for (int _ = 0; _ < PlatItemNumber; _++)
            {
                int length, itemStartAdress, streamCount;
                parsePlayItem(PlayItemEntries + shift, out length, out itemStartAdress, out streamCount);
                for (int streamOrder = 0; streamOrder < streamCount; streamOrder++)
                {
                    parseStream(itemStartAdress, streamOrder);
                }
                shift += (length + 2);//for that not counting the two length bytes themselves.
            }
            initializeTimeStampList();
            parsePlaylistMark();
        }

        private void initializeClass()
        {
            fileName = new List<string>();
            lastTime = new List<KeyValuePair<int, int>>();
            fps = new List<int>();
            timeStamp = new List<List<int>>();
            //timeStampInOne = new List<int>();
        }

        private void loadFile(string path)
        {
            ///read the whole mpls file into memory
            //File.SetAttributes(path, FileAttributes.Normal);
            // FileMode.Open));
            //binReader.BaseStream.Seek(0, SeekOrigin.Begin);
            using (BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
            {
                data = new byte[binReader.BaseStream.Length];
                binReader.Read(data, 0, (int)binReader.BaseStream.Length);
            }
            
            /*
            int i = 0;
            while (binReader.BaseStream.Position < binReader.BaseStream.Length)
            {
                data[i++] = binReader.ReadByte();
            }
            */

            
        }

        private void parseHeader()
        {
            PlaylistSectionStartAddress = byte2int(data, 0x08, 0x0c);
            PlaylistMarkSectionStartAddress = byte2int(data, 0x0c, 0x10);
            PlatItemNumber = byte2int(data, PlaylistSectionStartAddress + 0x06, 
                                            PlaylistSectionStartAddress + 0x08);
            PlayItemEntries = PlaylistSectionStartAddress + 0x0a;
        }
        private void initializeTimeStampList()
        {
            for (int _ = 0; _ < fileName.Count; _++)
            {
                timeStamp.Add(new List<int>());
            }
        }
        private void parsePlayItem(int PlayItemEntries, out int length, out int itemStartAdress, out int streamCount)
        {
            length = byte2int(data, PlayItemEntries + 0x00, PlayItemEntries + 0x02);
            string m2tsName = byte2string(data, PlayItemEntries + 0x02, PlayItemEntries + 0x0b);//name
            fileName.Add(m2tsName);

            int startTime = byte2int(data, PlayItemEntries + 0x0e, PlayItemEntries + 0x12);//start time 
            int   endTime = byte2int(data, PlayItemEntries + 0x12, PlayItemEntries + 0x16);//end   time
            lastTime.Add(new KeyValuePair<int, int>(startTime, endTime));

            //int audioAndPGS = byte2int(data, PlayItemEntries + 0x23, PlayItemEntries + 0x24) >> 4;//audio stream + subpath

            itemStartAdress = PlayItemEntries + 0x32;
            int UO2 = byte2int(data, PlayItemEntries + 0x22, PlayItemEntries + 0x23);
            streamCount = byte2int(data, PlayItemEntries + 0x23, PlayItemEntries + 0x24) >> 4;
            if (UO2 == 0x02)//this can only operate case: UO2 == 0x02,address shift value now is the hard code 
            {
                streamCount = byte2int(data, PlayItemEntries + 0x2f, PlayItemEntries + 0x30) >> 4;
                itemStartAdress = PlayItemEntries + 0x3e;
            }
        }
        private void parseStream(int itemStartAdress,int streamOrder)
        {
            byte[] Stream = new byte[16];
            for (int adress = 0; adress < 16; adress++)
            {
                Stream[adress] = data[itemStartAdress + streamOrder * 16 + adress];
            }

            //Occultism Oriented Programming Start
            //玄之又玄，众妙之门
            //itemStartAdress.ToString();//DO NOT DELETE THIS SENTENCE. IT HAS INDESCRIBABLE MAGIC
            //玄之又玄，众妙之门
            //Occultism Oriented Programming End

            if (Stream[01] == 0x01)// the stream type is a Play item
            {
                //int mPID = byte2int(Stream, 0x02, 0x04);//mPID
                int streamCodingType = byte2int(Stream, 0x0b, 0x0c);
                //System.Windows.Forms.MessageBox.Show(byte2Hex(Stream, 0, 16));
                if (streamCodingType == 0x1b)
                {
                    int attr = byte2int(Stream, 0x0c, 0x0d);
                    fps.Add(attr & 0xf);//last 4 bits is the fps
                }
            }
        }
        private void parsePlaylistMark()
        {
            int PlaylistMarkNumber = byte2int(data, PlaylistMarkSectionStartAddress + 0x04, PlaylistMarkSectionStartAddress + 0x06);
            int PlaylistMarkEntries = PlaylistMarkSectionStartAddress + 0x06;
            byte[] bytelist = new byte[14];//eg. 0001 yyyy xxxxxxxx FFFF 000000
                                           // 0 unknown
                                           // 1 type
                                           // 2, 3 stream_file_index
                                           // 4, 5, 6, 7 chapter_time
                                           // 8 - 13 unknown
            for (int mark = 0; mark < PlaylistMarkNumber; ++mark)
            {
                for (int x = 0; x < 14; x++)
                {
                    bytelist[x] = data[PlaylistMarkEntries + x];
                }
                if (1 != byte2int(bytelist,0,2)) { continue; }
                int index = byte2int(bytelist, 2, 4);
                int TimeStamp = byte2int(bytelist, 4, 8);
                timeStamp[index].Add(TimeStamp);
                PlaylistMarkEntries += 14;
            }
        }

        private int byte2int(byte[] source, int start, int end)//0xFF
        {
            int temp = 0;
            for (int i = start; i < end; i++)
            {
                temp += source[i] * 1 << (8 * (end - i - 1));
            }
            return temp;
        }
        private string byte2string(byte[] source, int start, int end)
        {
            string temp = string.Empty;
            for (int i = start; i < end; i++)
            {
                temp += Convert.ToChar(source[i]);
            }
            return temp;
        }
        /*
        private string byte2Hex(byte[] source, int start, int end)
        {
            string temp = string.Empty;
            for (int n = start; n < end; n++)
            {
                temp += source[n].ToString("x2") + " ";
            }
            return temp;
        }
        */
        /*
        public string second2time(int pts)//format a pts as hh:mm:ss.sss
        {
            double total    = pts / 45000.0;
            int Hour        = (int)(total / 3600);
            int Minute      = (int)((total - Hour * 3600) / 60);
            int Second      = (int) (total - Hour * 3600 - Minute * 60);
            int Millisecond = (int)((total - Hour * 3600 - Minute * 60 - Second) * 1000);
            return   Hour.ToString("00") + ":" +
                   Minute.ToString("00") + ":" +
                   Second.ToString("00") + "." +
                   Millisecond.ToString("000");
        }
        */
    }
}

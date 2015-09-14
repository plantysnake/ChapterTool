using System;
using System.Collections.Specialized;
using System.IO;

namespace ChapterTool
{
    public class TSStreamBuffer
    {
        private byte[] Buffer = new byte[0x1000];
        private int BufferLength;
        private int SkipBits;
        private MemoryStream Stream = new MemoryStream();
        public int TransferLength;

        public TSStreamBuffer()
        {
            this.Stream = new MemoryStream(this.Buffer);
        }

        public void Add(byte[] buffer, int offset, int length)
        {
            this.TransferLength += length;
            if ((this.BufferLength + length) >= this.Buffer.Length)
            {
                length = this.Buffer.Length - this.BufferLength;
            }
            if (length > 0)
            {
                Array.Copy(buffer, offset, this.Buffer, this.BufferLength, length);
                this.BufferLength += length;
            }
        }

        public void BeginRead()
        {
            this.SkipBits = 0;
            this.Stream.Seek(0L, SeekOrigin.Begin);
        }

        public void EndRead()
        {
        }

        public int ReadBits(int bits)
        {
            long position = this.Stream.Position;
            int num2 = 0x18;
            int data = 0;
            for (int i = 0; i < 4; i++)
            {
                if ((position + i) >= this.BufferLength)
                {
                    break;
                }
                data += this.Stream.ReadByte() << num2;
                num2 -= 8;
            }
            BitVector32 vector = new BitVector32(data);
            int num5 = 0;
            for (int j = this.SkipBits; j < (this.SkipBits + bits); j++)
            {
                num5 = num5 << 1;
                num5 += vector[((int)1) << ((0x20 - j) - 1)] ? 1 : 0;
            }
            this.SkipBits += bits;
            this.Stream.Seek(position + (this.SkipBits >> 3), SeekOrigin.Begin);
            this.SkipBits = this.SkipBits % 8;
            return num5;
        }

        public byte ReadByte()
        {
            return (byte)this.Stream.ReadByte();
        }

        public byte[] ReadBytes(int bytes)
        {
            if ((this.Stream.Position + bytes) >= this.BufferLength)
            {
                return null;
            }
            byte[] buffer = new byte[bytes];
            this.Stream.Read(buffer, 0, bytes);
            return buffer;
        }

        public void Reset()
        {
            this.BufferLength = 0;
            this.TransferLength = 0;
        }

        public void Seek(long offset, SeekOrigin loc)
        {
            this.Stream.Seek(offset, loc);
        }

        public long Length
        {
            get
            {
                return (long)this.BufferLength;
            }
        }

        public long Position
        {
            get
            {
                return this.Stream.Position;
            }
        }
    }


}

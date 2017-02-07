using System;
using System.IO;
using System.Threading;


namespace App.Common.Wrappers.Implementations
{
    public class StreamWrapper : IStreamWrapper
    {
        public Stream Stream { get; private set; }
       
        public StreamWrapper(Stream stream)
        {
            Stream = stream;
        }

        public int ReadByte()
        {
            return Stream.ReadByte();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return Stream.Read(buffer, offset, count);
        }

        public string StreamReadLine()
        {
            var data = "";
            while (true)
            {
                var nextChar = Stream.ReadByte();
                if (nextChar == '\n')
                {
                    break;
                }
                switch (nextChar)
                {
                    case '\r':
                        continue;
                    case -1:
                        Thread.Sleep(1);
                        continue;
                }
                data += Convert.ToChar(nextChar);
            }
            return data;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        ~StreamWrapper()
        {
            Dispose(false);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if(disposing && Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }
        }
    }
}
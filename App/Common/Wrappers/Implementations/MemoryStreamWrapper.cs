using System;
using System.IO;


namespace App.Common.Wrappers.Implementations
{
    public class MemoryStreamWrapper : IMemoryStreamWrapper
    {
        private MemoryStream memoryStream;

        public MemoryStreamWrapper(MemoryStream memoryStream)
        {
            this.memoryStream = memoryStream;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            memoryStream.Write(buffer, offset, count);
        }

        public void Seek(long offset, SeekOrigin loc)
        {
            memoryStream.Seek(offset, loc);
        }

        public byte[] ToArray()
        {
            return memoryStream.ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MemoryStreamWrapper()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && memoryStream != null)
            {
                memoryStream.Dispose();
                memoryStream = null;
            }
        }
    }
}
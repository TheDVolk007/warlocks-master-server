using System;
using System.IO;


namespace App.Common.Wrappers.Implementations
{
    public class StreamWriterWrapper : IStreamWriterWrapper
    {
        private StreamWriter streamWriter;

        public StreamWriterWrapper(IStreamWrapper streamWrapper)
        {
            streamWriter = new StreamWriter(streamWrapper.Stream);
        }

        public void Write(string value)
        {
            streamWriter.Write(value);
        }

        public void WriteThroughBaseStream(byte[] buffer, int offset, int count)
        {
            streamWriter.BaseStream.Write(buffer, offset, count);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~StreamWriterWrapper()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && streamWriter != null)
            {
                streamWriter.Dispose();
                streamWriter = null;
            }
        }
    }
}
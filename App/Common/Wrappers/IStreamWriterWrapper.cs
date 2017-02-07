using System;


namespace App.Common.Wrappers
{
    public interface IStreamWriterWrapper : IDisposable
    {
        void Write(string value);
        void WriteThroughBaseStream(byte[] buffer, int offset, int count);
    }
}
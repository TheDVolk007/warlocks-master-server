using System;
using System.IO;


namespace App.Common.Wrappers
{
    public interface IMemoryStreamWrapper : IDisposable
    {
        void Write(byte[] buffer, int offset, int count);
        void Seek(long offset, SeekOrigin loc);
        byte[] ToArray();
    }
}
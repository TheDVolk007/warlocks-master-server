using System;
using System.IO;


namespace App.Common.Wrappers
{
    public interface IStreamWrapper : IDisposable
    {
        Stream Stream { get; }
        int Read(byte[] buffer, int offset, int count);
        int ReadByte();
        string StreamReadLine();
    }
}
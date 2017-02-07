using System.IO;


namespace App.Common.Wrappers.Implementations
{
    public class StreamFactory : IStreamFactory
    {
        public IStreamWriterWrapper GetStreamWriterWrapper(IStreamWrapper streamWrapper)
        {
            return new StreamWriterWrapper(streamWrapper);
        }

        public IMemoryStreamWrapper GetMemoryStreamWrapper()
        {
            return new MemoryStreamWrapper(new MemoryStream());
        }
    }
}
namespace App.Common.Wrappers
{
    public interface IStreamFactory
    {
        IStreamWriterWrapper GetStreamWriterWrapper(IStreamWrapper streamWrapper);

        IMemoryStreamWrapper GetMemoryStreamWrapper();
    }
}
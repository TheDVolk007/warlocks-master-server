using System;
using App.Common.Wrappers;


namespace App.HttpServerScripts
{
    public interface IHttpProcessorFactory
    {
        IHttpProcessor GetHttpProcessor(ITcpClientWrapper tcpClient,
            IStreamFactory streamFactory,
            Action<string> getHandler,
            Action<string, IMemoryStreamWrapper, IStreamWriterWrapper> postHandler);
    }
}
using System;
using App.Common.Wrappers;


namespace App.HttpServerScripts.Implementations
{
    public class HttpProcessorFactory : IHttpProcessorFactory
    {
        public IHttpProcessor GetHttpProcessor(ITcpClientWrapper tcpClient,
            IStreamFactory streamFactory, Action<string> getHandler,
            Action<string, IMemoryStreamWrapper, IStreamWriterWrapper> postHandler)
        {
            return new HttpProcessor(tcpClient, streamFactory, getHandler, postHandler);
        }
    }
}
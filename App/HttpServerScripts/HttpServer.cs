using System.Net.Sockets;
using System.Threading;
using App.Common.Wrappers;


// ReSharper disable InconsistentNaming


namespace App.HttpServerScripts
{
    public abstract class HttpServer
    {
        private readonly ITcpListenerWrapper listener;
        private readonly IStreamFactory streamFactory;
        private readonly IHttpProcessorFactory httpProcessorFactory;

        protected HttpServer(ITcpListenerWrapper listener,
            IStreamFactory streamFactory,
            IHttpProcessorFactory httpProcessorFactory)
        {
            this.listener = listener;
            this.streamFactory = streamFactory;
            this.httpProcessorFactory = httpProcessorFactory;
        }

        protected void Listen()
        {
            listener.Start();
            while (true)
            {
                ITcpClientWrapper tcpClient;
                try
                {
                    tcpClient = listener.AcceptTcpClient();
                }
                catch (SocketException)
                {
                    // "Необработанное исключение типа "System.Net.Sockets.SocketException" в System.dll
                    // Дополнительные сведения: Операция блокирования прервана вызовом WSACancelBlockingCall"
                    // при закрытии приложения
                    break;
                }
                var processor = httpProcessorFactory.GetHttpProcessor(tcpClient, streamFactory, HandleGETRequest, HandlePOSTRequest);
                var thread = new Thread(processor.Process);
                thread.Start();
                Thread.Sleep(1);
            }
        }

        protected void Stop()
        {
            listener.Stop();
        }

        public abstract void StartHttpServer();

        public abstract void StopHttpServer();

        protected abstract void HandleGETRequest(string httpUrl);

        protected abstract void HandlePOSTRequest(string httpUrl, IMemoryStreamWrapper ms, IStreamWriterWrapper outputStream);
    }
}
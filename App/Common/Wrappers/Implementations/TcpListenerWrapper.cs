using System.Net;
using System.Net.Sockets;
using App.Properties;


namespace App.Common.Wrappers.Implementations
{
    public class TcpListenerWrapper : ITcpListenerWrapper
    {
        private readonly TcpListener listener;

        public TcpListenerWrapper()
        {
            listener = new TcpListener(IPAddress.Any, Settings.Default.HttpPort);
        }

        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }

        public ITcpClientWrapper AcceptTcpClient()
        {
            return new TcpClientWrapper(listener.AcceptTcpClient());
        }
    }
}
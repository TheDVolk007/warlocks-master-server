using System.Net.Sockets;


namespace App.Common.Wrappers.Implementations
{
    public class TcpClientWrapper : ITcpClientWrapper
    {
        private readonly TcpClient tcpClient;

        public TcpClientWrapper(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        public IStreamWrapper GetStream()
        {
            return new StreamWrapper(tcpClient.GetStream());
        }

        public void Close()
        {
            tcpClient.Close();
        }
    }
}
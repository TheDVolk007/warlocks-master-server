namespace App.Common.Wrappers
{
    public interface ITcpListenerWrapper
    {
        ITcpClientWrapper AcceptTcpClient();
        void Start();
        void Stop();
    }
}
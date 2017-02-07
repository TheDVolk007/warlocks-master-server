namespace App.Common.Wrappers
{
    public interface ITcpClientWrapper
    {
        void Close();
        IStreamWrapper GetStream();
    }
}
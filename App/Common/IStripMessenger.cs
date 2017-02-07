namespace App.Common
{
    public interface IStripMessenger
    {
        string StripMessage { get; set; }

        event UpdateHandler OnNewMessageReceived;
    }

    public delegate void UpdateHandler();
}
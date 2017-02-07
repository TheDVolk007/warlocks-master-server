using System;
using Dal.Wrappers;

namespace App.Common
{
    public class StripMessenger : IStripMessenger
    {
        private readonly IErrorHandler errorHandler;
        public event UpdateHandler OnNewMessageReceived;

        private string stripMessage;

        public string StripMessage
        {
            get { return stripMessage; }
            set
            {
                stripMessage = value;
                if(OnNewMessageReceived != null)
                {
                    OnNewMessageReceived();
                }
            }
        }

        public StripMessenger(IErrorHandler errorHandler)
        {
            if (errorHandler == null)
                throw new ArgumentNullException(nameof(errorHandler));
            this.errorHandler = errorHandler;
            this.errorHandler.OnError += ErrorHandler_OnError;
        }

        private void ErrorHandler_OnError(string message)
        {
            StripMessage = message;
        }
    }
}

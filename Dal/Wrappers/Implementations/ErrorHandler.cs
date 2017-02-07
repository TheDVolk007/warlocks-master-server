using System;
using MongoDB.Driver;

namespace Dal.Wrappers.Implementations
{
    public class ErrorHandler : IErrorHandler
    {
        public log4net.ILog log;
        public event Wrappers.ErrorHandler OnError;

        public ErrorHandler(log4net.ILog log)
        {
            this.log = log;
        }

        public void HandleError(Exception e)
        {
            // логируем
            log.Error("A handled error has occurred", e);

            if (e is TimeoutException)
            {
                // закрыть процесс MongoDB
                // открыть его
                // попробовать подключиться
            }

            if (e is MongoConnectionException) // возникает, пока не возникнет TimeoutException
            {
            }

            if (OnError != null)
            {
                var message = e.ToString();
                if (message.Length > 128)
                    message = message.Substring(0, 128) + "...";
                OnError(message);
            }
        }
    }
}
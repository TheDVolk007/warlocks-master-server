using System;

namespace Dal.Wrappers
{
    public interface IErrorHandler
    {
        event ErrorHandler OnError;

        void HandleError(Exception e);
    }

    public delegate void ErrorHandler(string message);
}
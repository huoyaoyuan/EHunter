using System;

namespace EHunter.Provider.Pixiv.Messages
{
    internal sealed class LoginFailedMessage
    {
        public LoginFailedMessage(Exception exception) => Exception = exception;

        public Exception Exception { get; }
    }
}

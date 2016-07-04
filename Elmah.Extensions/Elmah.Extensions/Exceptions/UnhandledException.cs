using System;

namespace Elmah.Extensions.Exceptions
{
    [Serializable]
    public class UnhandledException : Exception
    {
        public UnhandledException(Exception innerException): base("Unhandled exception occurred. Check the Inner Exception.", innerException)
        {

        }
    }
}

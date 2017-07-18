using System;

namespace Wrld.Scripts.Utilities
{
    public class InvalidApiKeyException : Exception
    {
        public InvalidApiKeyException(string message)
            : base(message)
        {

        }
    }
}


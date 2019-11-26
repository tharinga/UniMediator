using System;

namespace UniMediator.Internal
{
    public class UniMediatorException : Exception
    {
        public UniMediatorException(string message) : base(message) {}
    }
}
using System;

namespace Operations
{
    internal class OperationException : Exception
    {
        public OperationException(string message) => new Exception(message);
    }
}
using System;
using System.Collections.Generic;

namespace Operations
{
    /// <summary>
    /// Immutable
    /// </summary>
    internal struct Context<T> : IContext<T>
    {
        public T Result { get; }
        public bool Succeeded { get; private set; }
        public IDictionary<string, object> Properties { get; }
        public Exception Error { get; }

        internal Context<T> Fail()
        {
            Succeeded = false;
            return this;
        }

        internal Context(T value, IDictionary<string, object> props = null)
        {
            Result = Throw.IfDefault(value, nameof(value));
            Succeeded = true;
            Error = null;
            Properties = GetProperties(props);
        }

        internal Context(T value, Exception error, IDictionary<string, object> props = null)
        {
            Result = value;
            Succeeded = false;
            Error = error;
            Properties = GetProperties(props);
        }

        internal Context(Exception error, IDictionary<string, object> props = null)
        {
            Result = default(T);
            Succeeded = false;
            Error = error;
            Properties = GetProperties(props);
        }

        private static IDictionary<string, object> GetProperties(
            IDictionary<string, object> props = null)
            => props == null ?
                new Dictionary<string, object>() :
                new Dictionary<string, object>(props);
    }
}
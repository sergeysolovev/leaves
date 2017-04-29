using System;
using System.Collections.Generic;

namespace Operations
{
    internal sealed class Result<T> : IResult<T>
    {
        public T Value { get; }
        public bool Succeeded { get; private set; }
        public IDictionary<string, object> Properties { get; }
        public Exception Error { get; }

        internal Result<T> Fail()
        {
            Succeeded = false;
            return this;
        }

        internal Result()
        {
            Properties = GetProperties();
        }

        internal Result(T value, IDictionary<string, object> props = null)
        {
            Value = Throw.IfDefault(value, nameof(value));
            Properties = GetProperties(props);
            Succeeded = true;
        }

        internal Result(T value, Exception error, IDictionary<string, object> props = null)
        {
            Value = value;
            Error = error;
            Properties = GetProperties(props);
        }

        internal Result(Exception error, IDictionary<string, object> props = null)
        {
            Error = error;
            Properties = GetProperties(props);
        }

        private IDictionary<string, object> GetProperties(
            IDictionary<string, object> props = null)
            => props == null ?
                new Dictionary<string, object>() :
                new Dictionary<string, object>(props);
    }
}
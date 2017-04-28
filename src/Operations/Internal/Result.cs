using System;
using System.Collections.Generic;

namespace Operations
{
    internal sealed class Result<T> : IResult<T>
    {
        public T Value { get; }
        public bool HasValue { get; }
        public IDictionary<string, object> Properties { get; }
        public Exception Error { get; }

        internal Result()
        {
            HasValue = false;
            Properties = GetProperties();
        }

        internal Result(T value, IDictionary<string, object> props = null)
        {
            Value = Throw.IfDefault(value, nameof(value));
            HasValue = true;
            Properties = GetProperties(props);
        }

        internal Result(Exception error, IDictionary<string, object> props = null)
        {
            HasValue = false;
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
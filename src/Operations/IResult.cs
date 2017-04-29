using System;
using System.Collections.Generic;

namespace Operations
{
    public interface IResult<out T>
    {
        T Value { get; }
        bool Succeeded { get; }
        IDictionary<string, object> Properties { get; }
        Exception Error { get; }
    }
}
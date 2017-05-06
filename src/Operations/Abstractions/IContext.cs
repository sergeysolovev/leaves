using System;
using System.Collections.Generic;

namespace Operations
{
    /// <summary>
    /// Immutable operation context
    /// </summary>
    public interface IContext<out TResult>
    {
        TResult Result { get; }
        bool Succeeded { get; }
        IDictionary<string, object> Properties { get; }
        Exception Error { get; }
    }
}
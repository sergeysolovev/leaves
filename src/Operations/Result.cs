using System;
using System.Collections.Generic;

namespace Operations
{
    public static class Result
    {
        public static IResult<T> Just<T>(T value, Dictionary<string, object> props = null)
            => new Result<T>(value, props);

        public static IResult<T> None<T>(Exception error, Dictionary<string, object> props = null)
            => new Result<T>(error, props);

        public static IResult<T> None<T>(string error, Dictionary<string, object> props = null)
            => new Result<T>(new OperationException(error), props);

        public static IResult<T> None<T>()
            => new Result<T>(null);

        public static IResult<T> None<U, T>(IResult<U> source)
            => new Result<T>(source.Error, source.Properties);
    }
}
using System;
using System.Collections.Generic;

namespace Operations
{
    public static class Result
    {
        public static IResult<T> Succeed<T>(T value, Dictionary<string, object> props = null)
            => new Result<T>(value, props);

        public static IResult<T> None<T>()
            => new Result<T>(null);

        public static IResult<T> Fail<T>(Exception error, Dictionary<string, object> props = null)
            => new Result<T>(error, props);

        public static IResult<T> Fail<T>(string error, Dictionary<string, object> props = null)
            => new Result<T>(new InvalidOperationException(error), props);

        public static IResult<T> FailFrom<T>(IResult<T> source)
            => new Result<T>(source.Value, source.Error, source.Properties);

        public static IResult<U> FailFrom<T, U>(IResult<T> source)
            => source is IResult<U> result ?
                FailFrom(result) :
                new Result<U>(source.Error, source.Properties);
    }
}
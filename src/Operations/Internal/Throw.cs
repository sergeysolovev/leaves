using System;
using System.Collections.Generic;

namespace Operations
{
    internal static class Throw
    {
        public static T IfNull<T>(T param, string paramName) where T : class
            => param ?? throw new ArgumentNullException(paramName);

        public static T IfDefault<T>(T param, string paramName)
            => IsDefault(param) ?
                throw new ArgumentException(nameof(paramName)) :
                param;

        private static bool IsDefault<T>(T param)
            => EqualityComparer<T>.Default.Equals(param, default(T));
    }
}
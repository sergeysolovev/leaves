using System;
using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public static class Throw
    {
        public static T IfNull<T>(T param, string paramName)
            => param is Object obj && obj == null ?
                throw new ArgumentNullException(paramName) :
                param;

        public static string IfNullOrEmpty(string param, string paramName)
            => String.IsNullOrEmpty(param) ?
                throw new ArgumentNullException(paramName) :
                param;

        public static T IfDefault<T>(T param, string paramName)
            => IsDefault(param) ?
                throw new ArgumentException(nameof(paramName)) :
                param;

        private static bool IsDefault<T>(T param)
            => EqualityComparer<T>.Default.Equals(param, default(T));
    }
}

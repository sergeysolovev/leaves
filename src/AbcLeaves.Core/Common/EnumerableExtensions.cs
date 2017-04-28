using System.Collections.Generic;

namespace AbcLeaves.Core
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            return new [] { item };
        }
    }
}

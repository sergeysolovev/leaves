using System;

namespace Operations
{
    public static class ContextExtensions
    {
        public static IContext<TResult> With<TResult>(
            this IContext<TResult> source,
            Action<TResult> doAction)
        {
            doAction(source.Result);
            return source;
        }
    }
}
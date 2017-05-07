using System;
using System.Threading.Tasks;

namespace Operations
{
    public interface IBuilder<TResult, out TBuilder> : IOperationBuilder<TResult>
        where TBuilder : IBuilder<TResult, TBuilder>
    {
        TBuilder Return(IOperation<TResult> source);
    }

    internal class OperationBuilder<T> : IOperationBuilder<T>
    {
        private readonly IOperation<T> source;
        internal OperationBuilder(IOperation<T> source) => this.source = source;
        public IOperation<T> Build() => source;
    }

    public static class OperationBuilder
    {
        public static IOperationBuilder<T> Return<T>(IOperation<T> source)
            => new OperationBuilder<T>(source);
    }
}
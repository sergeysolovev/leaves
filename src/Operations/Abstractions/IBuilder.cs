using System;
using System.Threading.Tasks;

namespace Operations
{
    public interface IBuilder<T, out TBuilder> : IOperationBuilder<T>
        where TBuilder : IBuilder<T, TBuilder>
    {
        TBuilder Return(IOperationService<T> source);
    }
}
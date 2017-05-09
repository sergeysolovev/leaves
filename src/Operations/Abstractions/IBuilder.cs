using System;
using System.Threading.Tasks;

namespace Operations
{
    public interface IBuilder<T, TSelf> : IOperationBuilder<T>
    {
        TSelf Return(IOperation<T> source);
    }

    public interface IPreBuilder<T, TSelf> : IOperationPreBuilder<T>
    {
        TSelf Return(IOperationService<T> source);
    }
}
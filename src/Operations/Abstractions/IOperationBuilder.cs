using System;

namespace Operations
{
    public interface IOperationBuilder<TResult>
    {
        IOperation<TResult> Build();
    }
}
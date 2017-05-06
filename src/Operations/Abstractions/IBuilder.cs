namespace Operations
{
    public interface IBuilder<TResult, out TReturn> : IOperationBuilder<TResult>
    {
        TReturn Return(IOperation<TResult> source);
    }
}
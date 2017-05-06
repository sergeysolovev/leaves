namespace Operations
{
    public interface IBuilder<TResult, out TBuilder> : IOperationBuilder<TResult>
        where TBuilder : IBuilder<TResult, TBuilder>
    {
        TBuilder Return(IOperation<TResult> source);
    }
}
namespace Operations
{
    public interface IOperationService<in TSource, TResult>
    {
        IOperation<TResult> Inject(TSource source);
    }

    public interface IOperationService<TResult>
    {
        IOperation<TResult> Inject(TResult source);
    }
}
namespace Operations
{
    public interface IOperationService<in TSource, TResult>
    {
        IOperation<TResult> Inject(TSource value);
    }

    public interface IOperationService<TResult>
    {
        IOperation<TResult> Inject(TResult value);
    }
}
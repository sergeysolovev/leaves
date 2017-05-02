namespace Operations
{
    public interface IOperationBuilder<TResult>
    {
        void Inject(IOperationService<TResult, TResult> service);
        IOperation<TResult> Build();
    }
}
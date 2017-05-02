namespace Operations
{
    public interface IOperationService<in T, U>
    {
        IOperation<U> Return(T value);
    }
}
namespace Operations
{
    public interface IOperationBuilder<T>
    {
        IOperation<T> BuildWith(T injection);
    }
}
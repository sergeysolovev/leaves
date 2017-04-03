namespace AbcLeaves.Core
{
    public interface INotFoundOperationResult : IOperationResult
    {
        bool NotFound { get; }
    }
}

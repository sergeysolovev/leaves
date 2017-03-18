namespace AbcLeaves.Api
{
    public interface INotFoundOperationResult : IOperationResult
    {
        bool NotFound { get; }
    }
}

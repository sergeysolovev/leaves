namespace AbcLeaves.Api.Operations
{
    public interface IFindResult : IOperationResult
    {
        bool NotFound { get; }
    }
}

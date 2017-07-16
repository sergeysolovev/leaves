namespace AbcLeaves.Core
{
    public interface IFindResult : IOperationResult
    {
        bool NotFound { get; }
    }
}

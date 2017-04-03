namespace AbcLeaves.Core
{
    public interface IForbiddenOperationResult : IOperationResult
    {
        bool IsForbidden { get; }
    }
}

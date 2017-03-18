namespace AbcLeaves.Api
{
    public interface IForbiddenOperationResult : IOperationResult
    {
        bool IsForbidden { get; }
    }
}

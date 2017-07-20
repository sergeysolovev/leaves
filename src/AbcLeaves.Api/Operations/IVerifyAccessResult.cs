namespace AbcLeaves.Api.Operations
{
    public interface IVerifyAccessResult : IOperationResult
    {
        bool IsForbidden { get; }
    }
}

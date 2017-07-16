namespace AbcLeaves.Core
{
    public interface IVerifyAccessResult : IOperationResult
    {
        bool IsForbidden { get; }
    }
}

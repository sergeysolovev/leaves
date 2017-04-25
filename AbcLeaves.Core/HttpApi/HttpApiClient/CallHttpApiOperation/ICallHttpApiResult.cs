namespace AbcLeaves.Core
{
    public interface ICallHttpApiResult : IOperationResult<DefaultOperationContext>
    {
        CallHttpApiResponseDetails ApiResponseDetails { get; }
        CallHttpApiRequestDetails ApiRequestDetails { get; }
    }
}

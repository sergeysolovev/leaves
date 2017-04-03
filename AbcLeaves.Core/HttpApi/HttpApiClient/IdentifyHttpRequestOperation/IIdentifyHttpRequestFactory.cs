namespace AbcLeaves.Core
{
    public interface IIdentifyHttpRequestFactory
    {
        HttpApiAuthType AuthType { get; }
        IIdentifyHttpRequestOperation Create();
    }
}

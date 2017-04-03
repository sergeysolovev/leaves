namespace AbcLeaves.Core
{
    public interface IIdentifyHttpApiRequestFactory
    {
        IIdentifyHttpRequestOperation Create(IHttpApiOptions apiOptions);
        void RegisterIdentifyHttpRequestFactory(IIdentifyHttpRequestFactory factory);
        bool IsRegisteredAuthType(HttpApiAuthType authType);
    }
}
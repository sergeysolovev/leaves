namespace AbcLeaves.Core
{
    public interface IHttpApiClientFactory
    {
        IHttpApiClient Create(IHttpApiOptions apiOptions);
    }
}

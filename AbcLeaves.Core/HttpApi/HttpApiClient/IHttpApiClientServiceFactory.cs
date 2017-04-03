namespace AbcLeaves.Core
{
    public interface IHttpApiClientServiceFactory
    {
        IHttpApiClientService Create(IHttpApiOptions apiOptions);
    }
}

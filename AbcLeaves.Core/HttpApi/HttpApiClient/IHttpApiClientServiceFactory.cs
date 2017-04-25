namespace AbcLeaves.Core
{
    public interface IHttpApiClientServiceFactory
    {
        IHttpApiClientService Create(ICallHttpApiOptions apiOptions);
    }
}

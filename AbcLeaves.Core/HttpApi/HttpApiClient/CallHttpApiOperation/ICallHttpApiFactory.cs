namespace AbcLeaves.Core
{
    public interface ICallHttpApiFactory
    {
        ICallHttpApiOperation Create(IHttpApiOptions apiOptions);
    }
}

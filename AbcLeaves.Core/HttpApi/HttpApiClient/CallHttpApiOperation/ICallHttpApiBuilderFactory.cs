namespace AbcLeaves.Core
{
    public interface ICallHttpApiBuilderFactory
    {
        ICallHttpApiBuilder Create(IHttpApiOptions apiOptions);
    }
}

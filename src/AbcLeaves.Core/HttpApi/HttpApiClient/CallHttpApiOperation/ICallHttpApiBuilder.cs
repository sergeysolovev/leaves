namespace AbcLeaves.Core
{
    public interface ICallHttpApiBuilder : ICallHttpApiRequestBuilder
    {
        ICallHttpApiOperation Build();
    }
}
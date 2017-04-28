namespace AbcLeaves.Core
{
    public interface ICallHttpApiBuilderFactory
    {
        ICallHttpApiBuilder Create(ICallHttpApiOptions apiOptions);
    }
}

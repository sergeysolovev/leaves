namespace AbcLeaves.Core
{
    public interface ICallHttpApiOptions
    {
        string Name { get; set; }
        string BaseUrl { get; set; }
        IHttpBackchannel Backchannel { get; set; }
    }
}

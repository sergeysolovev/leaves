namespace AbcLeaves.Core
{
    public interface IHttpApiOptions
    {
        string Name { get; set; }
        string BaseUrl { get; set; }
        HttpApiAuthType AuthType { get; set; }
        IHttpBackchannel Backchannel { get; set; }
    }
}

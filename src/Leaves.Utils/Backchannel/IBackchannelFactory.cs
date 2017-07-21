namespace Leaves.Utils
{
    public interface IBackchannelFactory
    {
        IBackchannel Create(string baseUrl = null);
    }
}

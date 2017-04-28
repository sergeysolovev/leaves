namespace AbcLeaves.Core
{
    internal class IdentifyHttpRequestIdentity :
        IdentityOperation<MapHttpRequestResult, MapHttpRequestContext>,
        IIdentifyHttpRequest
    {
        public static new IdentifyHttpRequestIdentity Create()
        {
            return new IdentifyHttpRequestIdentity();
        }
    }
}

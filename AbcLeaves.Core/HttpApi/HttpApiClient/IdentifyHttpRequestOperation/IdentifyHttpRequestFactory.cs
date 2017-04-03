namespace AbcLeaves.Core
{
    public abstract class IdentifyHttpRequestFactory : IIdentifyHttpRequestFactory
    {
        private static SkipIdentifyingHttpRequestFactory skipIdentifyingFactory;

        public abstract HttpApiAuthType AuthType { get; }
        public abstract IIdentifyHttpRequestOperation Create();

        public static IIdentifyHttpRequestFactory SkipIdentifyingFactory
        {
            get
            {
                if (skipIdentifyingFactory == null)
                {
                    skipIdentifyingFactory = new SkipIdentifyingHttpRequestFactory();
                }
                return skipIdentifyingFactory;
            }
        }

        internal class SkipIdentifyingHttpRequestFactory : IdentifyHttpRequestFactory
        {
            public override HttpApiAuthType AuthType => HttpApiAuthType.None;

            public override IIdentifyHttpRequestOperation Create()
            {
                return IdentifyHttpRequest.None;
            }
        }
    }
}

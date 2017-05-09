using System;
using System.Net.Http;
using Operations;

namespace Operations.Extensions.Http
{
    public interface IHttpMessageHandlerBuilder :
        IBuilder<HttpMessageHandler, IHttpMessageHandlerBuilder>
    {
        IHttpMessageHandlerBuilder AddHandler(DelegatingHandler handler);
    }

    internal class HttpMessageHandlerBuilder :
        Builder<HttpMessageHandler, HttpMessageHandlerBuilder>,
        IHttpMessageHandlerBuilder
    {
        internal HttpMessageHandlerBuilder() : base(new HttpClientHandler()) { }

        public IHttpMessageHandlerBuilder AddHandler(DelegatingHandler handler)
            => With(sourceHandler
                => Context
                    .Succeed(handler)
                    .With(x => x.InnerHandler = sourceHandler));

        IHttpMessageHandlerBuilder IBuilder<HttpMessageHandler, IHttpMessageHandlerBuilder>.Return(
            IOperation<HttpMessageHandler> source)
            => Return(source);
    }
}
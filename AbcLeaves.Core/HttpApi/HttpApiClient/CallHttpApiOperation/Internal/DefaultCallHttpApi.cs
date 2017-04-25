using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    internal class DefaultCallHttpApi : Operation<CallHttpApiResult>, ICallHttpApiOperation
    {
        private IIdentifyHttpRequest identifyRequest;
        private IHttpBackchannel backchannel;

        public string ApiName { get; set; }
        public string ApiEndpoint { get; set; }
        public HttpMethod Method { get; set; }
        public Action<HttpRequestHeaders> AddRequestHeaders { get; set; }
        public Func<HttpContent> GetRequestContent { get; set; }

        public IHttpBackchannel Backchannel
        {
            get { return backchannel; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (backchannel != null)
                {
                    throw new InvalidOperationException();
                }
                backchannel = value;
            }
        }

        public IIdentifyHttpRequest IdentifyRequest
        {
            get { return identifyRequest; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (identifyRequest != null)
                {
                    throw new InvalidOperationException();
                }
                identifyRequest = value;
            }
        }

        public override async Task<CallHttpApiResult> ExecuteAsync(DefaultOperationContext context)
        {
            var request = BuildRequestFromState();
            return await this
                .BeginWith(() => IdentifyRequestAsync(request))
                .ProceedWith(identify => ExecuteIdentifiedAsync(identify.Request))
                .Return();
        }

        private async Task<CallHttpApiResult> ExecuteIdentifiedAsync(HttpRequestMessage request)
        {
            var requestDetails = await GetRequestDetailsAsync(request);
            try
            {
                using (var response = await SendRequestAsync(request))
                {
                    var responseDetails = await GetResponseDetailsAsync(response);
                    return CallHttpApiResult.From(requestDetails, responseDetails, ApiName);
                }
            }
            catch (HttpRequestException ex)
            {
                var errors = new string [] {
                    CallHttpApiResult.DefaultError + " More info:",
                    ex.Message,
                    ex.InnerException?.Message ?? String.Empty
                };
                var error = String.Join(" ", errors);
                return CallHttpApiResult.Fail(requestDetails, error, ApiName);
            }
        }

        private HttpRequestMessage BuildRequestFromState()
        {
            var request = new HttpRequestMessage(Method, ApiEndpoint);
            if (GetRequestContent != null)
            {
                request.Content = GetRequestContent.Invoke();
            }
            if (AddRequestHeaders != null)
            {
                AddRequestHeaders.Invoke(request.Headers);
            }
            return request;
        }

        private async Task<CallHttpApiRequestDetails> GetRequestDetailsAsync(
            HttpRequestMessage request)
        {
            var body = (request.Content != null) ?
                await request.Content?.ReadAsStringAsync() : null;
            var headers = request.Headers.ToString();
            var endpoint = request.RequestUri.ToString();
            var method = request.Method.ToString();
            return new CallHttpApiRequestDetails(endpoint, method, body, headers);
        }

        private async Task<CallHttpApiResponseDetails> GetResponseDetailsAsync(
            HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            var headers = response.Headers.ToString();
            var statusCode = response.StatusCode;
            return new CallHttpApiResponseDetails(body, headers, statusCode);
        }

        private void EnsureBackchannelIsSet()
        {
            if (backchannel == null)
            {
                backchannel = HttpBackchannel.Default;
            }
        }

        private void EnsureRequestIdentifierIsSet()
        {
            if (identifyRequest == null)
            {
                identifyRequest = IdentifyHttpRequestIdentity.Create();
            }
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            EnsureBackchannelIsSet();
            return await backchannel.SendAsync(request);
        }

        private async Task<MapHttpRequestResult> IdentifyRequestAsync(
            HttpRequestMessage request)
        {
            EnsureRequestIdentifierIsSet();
            return await identifyRequest.ExecuteAsync(new MapHttpRequestContext(request));
        }
    }
}

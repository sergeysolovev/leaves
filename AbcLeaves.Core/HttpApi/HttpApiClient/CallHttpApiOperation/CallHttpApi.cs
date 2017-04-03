using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public sealed class CallHttpApi : ICallHttpApiOperation
    {
        private readonly IIdentifyHttpRequestOperation identifyRequest;
        private IHttpBackchannel backchannel;

        private string ApiName => Params.ApiName;
        private string Url => Params.ApiEndpoint;
        private HttpMethod Method => Params.Method;
        private Action<HttpRequestHeaders> AddRequestHeaders => Params.AddRequestHeaders;
        private Func<HttpContent> GetRequestContent => Params.GetRequestContent;
        private ICallHttpApiOperationParams Params => (ICallHttpApiOperationParams)this;

        #region IApiCallOperationParams
        string ICallHttpApiOperationParams.ApiName { get; set; }
        string ICallHttpApiOperationParams.ApiEndpoint { get; set; }
        HttpMethod ICallHttpApiOperationParams.Method { get; set; }
        Action<HttpRequestHeaders> ICallHttpApiOperationParams.AddRequestHeaders { get; set; }
        Func<HttpContent> ICallHttpApiOperationParams.GetRequestContent { get; set; }
        #endregion

        public static CallHttpApi Create(IIdentifyHttpRequestOperation identifyRequest)
        {
            return new CallHttpApi(identifyRequest);
        }

        private CallHttpApi(IIdentifyHttpRequestOperation identifyRequest)
        {
            if (identifyRequest == null)
            {
                throw new ArgumentNullException(nameof(identifyRequest));
            }

            this.identifyRequest = identifyRequest;
        }

        public IHttpBackchannel Backchannel
        {
            get
            {
                return backchannel;
            }
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

        public async Task<CallHttpApiResult> ExecuteAsync()
        {
            var request = BuildRequestFromState();
            return await Operation<CallHttpApiResult>
                .BeginWith(() => identifyRequest.ExecuteAsync(request))
                .ProceedWith(identify => ExecuteIdentifiedAsync(identify.Request))
                .Return();
        }

        private async Task<CallHttpApiResult> ExecuteIdentifiedAsync(HttpRequestMessage request)
        {
            var requestDetails = await GetRequestDetailsAsync(request);
            try
            {
                EnsureBackchannelIsSet();
                using (var response = await backchannel.SendAsync(request))
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
                return CallHttpApiResult.Fail(requestDetails, error, Params.ApiName);
            }
        }

        private HttpRequestMessage BuildRequestFromState()
        {
            var request = new HttpRequestMessage(Method, Url);
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
    }
}

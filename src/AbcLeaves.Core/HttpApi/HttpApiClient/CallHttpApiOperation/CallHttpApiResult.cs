using System;
using Newtonsoft.Json;

namespace AbcLeaves.Core
{
    public sealed class CallHttpApiResult : OperationResultBase, ICallHttpApiResult
    {
        public const string DefaultError = "An error occurred when calling API.";

        [JsonIgnore] public CallHttpApiResponseDetails ApiResponseDetails { get; private set; }
        [JsonIgnore] public CallHttpApiRequestDetails ApiRequestDetails { get; private set; }

        public CallHttpApiResult() : base() {}
        private CallHttpApiResult(bool succeeded) : base(succeeded)
        {
        }

        private Func<string, string> keyRequest = apiName => $"{apiName}Request";
        private Func<string, string> keyResponse = apiName => $"{apiName}Response";

        private CallHttpApiResult(CallHttpApiRequestDetails requestDetails, string error, string apiName)
            : base(error, null)
        {
            if (requestDetails == null)
            {
                throw new ArgumentNullException(nameof(requestDetails));
            }
            ApiRequestDetails = requestDetails;
            Details.Add(keyRequest(apiName), requestDetails.ToDictionary());
        }

        private CallHttpApiResult(CallHttpApiRequestDetails requestDetails, CallHttpApiResponseDetails responseDetails, string apiName)
            : base()
        {
            if (responseDetails == null)
            {
                throw new ArgumentNullException(nameof(responseDetails));
            }
            if (requestDetails == null)
            {
                throw new ArgumentNullException(nameof(requestDetails));
            }
            ApiRequestDetails = requestDetails;
            ApiResponseDetails = responseDetails;
            Succeeded = responseDetails.IsSuccessStatusCode;
            ErrorMessage = Succeeded ? null : DefaultError;
            Details.Add(keyRequest(apiName), requestDetails.ToDictionary());
            Details.Add(keyResponse(apiName), responseDetails.ToDictionary());
        }

        public static CallHttpApiResult Fail(
            CallHttpApiRequestDetails requestDetails,
            string error,
            string apiName)
            => new CallHttpApiResult(requestDetails, error, apiName);

        public static CallHttpApiResult From(
            CallHttpApiRequestDetails requestDetails,
            CallHttpApiResponseDetails responseDetails,
            string apiName)
            => new CallHttpApiResult(requestDetails, responseDetails, apiName);
    }
}

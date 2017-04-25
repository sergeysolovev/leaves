using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System;
using Microsoft.Extensions.Options;

namespace AbcLeaves.Core
{
    public class LeavesApiClient
    {
        private const string ErrorMessage = "An error occurred when requesting Abc Leaves API";
        private readonly IHttpApiClientService clientService;

        public LeavesApiClient(
            IOptions<HttpApiClientOptions<LeavesApiClient>> options,
            IHttpApiClientServiceFactory clientFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Value == null)
            {
                throw new ArgumentNullException(nameof(options.Value));
            }
            if (clientFactory == null)
            {
                throw new ArgumentNullException(nameof(clientFactory));
            }

            this.clientService = clientFactory.Create(options.Value);
        }

        public async Task<ICallHttpApiResult> ApplyLeaveAsync(CreateLeaveContract leave)
        {
            // todo: why it's not clear from CreateLeaveDto
            // that we need to use DateTimeZoneHandling.Utc?
            var leaveJson = JsonConvert.SerializeObject(leave, new JsonSerializerSettings {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            return await clientService.PostAsync("leaves/", x => x
                .UseRequestContent(() => new JsonContent(leaveJson)));
        }

        public async Task<ICallHttpApiResult> ApproveLeaveAsync(string id)
        {
            return await clientService.PatchAsync($"leaves/{id}/approve");
        }

        public async Task<ICallHttpApiResult> DeclineLeaveAsync(string id)
        {
            return await clientService.PatchAsync($"leaves/{id}/decline");
        }

        public async Task<VerifyAccessResult> VerifyGoogleApisAccess()
        {
            return await OperationFlow<VerifyAccessResult>
                .BeginWith(() => clientService.GetAsync("googleapis"))
                .ExitOnFailWith(callApi => {
                    var response = callApi.ApiResponseDetails;
                    if (response != null && response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        return VerifyAccessResult.FailForbidden;
                    }
                    return VerifyAccessResult.FailFrom(callApi);
                })
                .EndWith(callApi => {
                    var response = callApi.ApiResponseDetails;
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        return VerifyAccessResult.Success;
                    }
                    return VerifyAccessResult.FailFrom(callApi);
                })
                .Return();
        }

        public async Task<CallHttpApiResult> GrantGoogleApisAccess(
            string code, string redirectUrl)
        {
            return await clientService.PatchAsync("googleapis", x => x
                .AddRequestUrlParameter("code", code)
                .AddRequestUrlParameter("redirectUrl", redirectUrl));
        }
    }
}

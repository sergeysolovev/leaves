using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System;

namespace AbcLeaves.Core
{
    public class LeavesApiClient : ILeavesApiClient
    {
        private const string ErrorMessage = "An error occurred when requesting Abc Leaves API";
        private readonly IHttpApiClientService clientService;

        public static LeavesApiClient Create(
            IHttpApiClientServiceFactory apiClientServiceFactory,
            IHttpApiOptions apiOptions)
        {
            return new LeavesApiClient(apiClientServiceFactory, apiOptions);
        }

        private LeavesApiClient(
            IHttpApiClientServiceFactory apiClientServiceFactory,
            IHttpApiOptions apiOptions)
        {
            if (apiOptions == null)
            {
                throw new ArgumentNullException(nameof(apiOptions));
            }
            if (apiClientServiceFactory == null)
            {
                throw new ArgumentNullException(nameof(apiClientServiceFactory));
            }

            this.clientService = apiClientServiceFactory.Create(apiOptions);
        }

        public async Task<CallHttpApiResult> ApplyLeaveAsync(CreateLeaveContract leave)
        {
            // todo: why it's not clear from CreateLeaveDto
            // that we need to use DateTimeZoneHandling.Utc?
            var leaveJson = JsonConvert.SerializeObject(leave, new JsonSerializerSettings {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            return await clientService.PostAsync("leaves/", x => x
                .UseRequestContent(() => new JsonContent(leaveJson)));
        }

        public async Task<CallHttpApiResult> ApproveLeaveAsync(string id)
        {
            return await clientService.PatchAsync($"leaves/{id}/approve");
        }

        public async Task<CallHttpApiResult> DeclineLeaveAsync(string id)
        {
            return await clientService.PatchAsync($"leaves/{id}/decline");
        }

        public async Task<VerifyAccessResult> VerifyGoogleApisAccess()
        {
            return await Operation<VerifyAccessResult>
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

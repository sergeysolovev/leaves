using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System;
using Microsoft.Extensions.Options;
using System.Net.Http;
using AbcLeaves.Core;
using AbcLeaves.BasicMvcClient.Domain;

namespace AbcLeaves.BasicMvcClient
{
    public class LeavesApiClient
    {
        private const string ErrorMessage = "An error occurred when requesting leaves API";
        private readonly IBackchannel backchannel;
        private readonly AuthenticationManager authHelper;

        public LeavesApiClient(
            IOptions<LeavesApiClientOptions> options,
            IBackchannelFactory backchannelFactory,
            AuthenticationManager authHelper)
        {
            this.authHelper = Throw.IfNull(authHelper, nameof(authHelper));
            this.backchannel = Throw
                .IfNull(backchannelFactory, nameof(backchannelFactory))
                .Create(Throw.IfNull(options, nameof(options)).Value.BaseUrl);
        }

        public async Task<SendMessageResult> ApplyLeaveAsync(CreateLeaveContract leave)
        {
            // todo: why it's not clear from CreateLeaveContract
            // that we need to use DateTimeZoneHandling.Utc?
            var idToken = await authHelper.GetIdTokenAsync();
            var leaveJson = JsonConvert.SerializeObject(leave, new JsonSerializerSettings {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
            return await backchannel.PostAsync("leaves/", x => x
                .WithBearerToken(idToken)
                .WithJsonContent(leaveJson)
            );
        }

        public async Task<SendMessageResult> ApproveLeaveAsync(string id)
        {
            var idToken = await authHelper.GetIdTokenAsync();
            return await backchannel.PatchAsync($"leaves/{id}/approve", x => x
                .WithBearerToken(idToken)
            );
        }

        public async Task<SendMessageResult> DeclineLeaveAsync(string id)
        {
            var idToken = await authHelper.GetIdTokenAsync();
            return await backchannel.PatchAsync($"leaves/{id}/decline", x => x
                .WithBearerToken(idToken)
            );
        }

        public async Task<VerifyAccessResult> VerifyGoogleApisAccess()
        {
            var idToken = await authHelper.GetIdTokenAsync();
            var apiResult = await backchannel.GetAsync("googleapis/", x => x
                .WithBearerToken(idToken)
            );

            if (!apiResult.Succeeded)
            {
                return VerifyAccessResult.Fail(ErrorMessage);
            }

            switch (apiResult.Response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return VerifyAccessResult.Succeed(isForbidden: false);
                case HttpStatusCode.Forbidden:
                    return VerifyAccessResult.Succeed(isForbidden: true);
                default:
                    return VerifyAccessResult.Fail(ErrorMessage);
            }
        }

        public async Task<SendMessageResult> GrantGoogleApisAccess(
            string code, string redirectUrl)
        {
            var idToken = await authHelper.GetIdTokenAsync();
            return await backchannel.PatchAsync("googleapis/", x => x
                .WithBearerToken(idToken)
                .AddParameter("code", code)
                .AddParameter("redirectUrl", redirectUrl)
            );
        }
    }
}

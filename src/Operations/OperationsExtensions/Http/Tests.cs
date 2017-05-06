using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Operations.Extensions.Http
{
    static class CallHttpApiServiceTests
    {
        public static IOperation<IHttpApiResult> CallApi(
            IRequestBuilder requestBuilder,
            IGetHttpApiResponse getResponse)
        {
            var preBuildRequest = requestBuilder
                .UseBaseUri("http://localhost:8080/api/v1")
                .WithBearerToken("jpoajv9j-30j4v39jv093j43");
            return preBuildRequest
                .WithRelativeRef("leaves", HttpMethod.Post)
                .AddParameter("name", "value")
                .BuildWith(getResponse);
        }
    }
}
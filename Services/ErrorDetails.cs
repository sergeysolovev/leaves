using System;
using System.Net;

namespace ABC.Leaves.Api.Services
{
    public class ErrorDetails
    {
        public ErrorDetails() { }
        public int StatusCode { get; set; } = (int)HttpStatusCode.InternalServerError;
        public string DeveloperMessage { get; set; } = String.Empty;
        public string UserMessage { get; set; } = String.Empty;
    }
}
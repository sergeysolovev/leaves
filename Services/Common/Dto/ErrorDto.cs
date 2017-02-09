using System;
using System.Net;

namespace ABC.Leaves.Api.Services.Dto
{
    public class ErrorDto
    {
        public ErrorDto() { }
        public string DeveloperMessage { get; set; } = String.Empty;
        public string UserMessage { get; set; } = String.Empty;
    }
}
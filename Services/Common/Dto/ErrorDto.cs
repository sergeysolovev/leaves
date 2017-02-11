using System;

namespace ABC.Leaves.Api.Services.Dto
{
    public class ErrorDto
    {
        public ErrorDto()
        {
        }

        public ErrorDto(string message)
        {
            DeveloperMessage = message;
        }

        public string DeveloperMessage { get; set; } = String.Empty;
    }
}
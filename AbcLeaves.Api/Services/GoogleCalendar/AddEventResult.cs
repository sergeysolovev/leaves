namespace ABC.Leaves.Api.Services
{
    public class AddEventResult : IOperationResult
    {
        public static AddEventResult Success(string eventUri)
        {
            return new AddEventResult {
                Succeeded = true,
                EventUri = eventUri
            };
        }

        public static AddEventResult Fail(string message)
        {
            return new AddEventResult { ErrorMessage = message };
        }

        public bool Succeeded { get; set; }
        public string EventUri { get; set; }
        public string ErrorMessage { get; set; }
    }
}

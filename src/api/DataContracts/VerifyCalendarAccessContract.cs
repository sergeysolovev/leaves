namespace Leaves.Api.DataContracts
{
    public class VerifyCalendarAccessContract
    {
        public string UserId { get; set; }
        public bool Authorized { get; set; }
    }
}

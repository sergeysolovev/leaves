namespace ABC.Leaves.Api
{
    public interface IOperationResult
    {
        bool Succeeded { get; }
        string ErrorMessage { get; }
    }
}

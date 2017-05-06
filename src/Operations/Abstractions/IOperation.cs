using System.Threading.Tasks;

namespace Operations
{
    public interface IOperation<TResult>
    {
        Task<IContext<TResult>> ExecuteAsync();
    }
}
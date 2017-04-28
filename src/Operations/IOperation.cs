using System.Threading.Tasks;

namespace Operations
{
    public interface IOperation<T>
    {
        Task<IResult<T>> ExecuteAsync();
    }
}
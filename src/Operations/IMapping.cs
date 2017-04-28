using System;
using System.Net.Http;
using Operations.Linq;

namespace Operations
{
    public interface IMapping<T>
    {
        IOperation<T> Map(IOperation<T> arg);
    }
}
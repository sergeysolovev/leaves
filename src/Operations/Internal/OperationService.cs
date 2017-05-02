using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Operations.Linq;

namespace Operations
{
    internal class OperationService<T, U> : IOperationService<T, U>
    {
        private readonly Func<T, IOperation<U>> closure;

        internal OperationService(Func<T, IOperation<U>> closure)
            => this.closure = Throw.IfNull(closure, nameof(closure));

        public IOperation<U> Return(T value)
            => closure(value);
    }
}
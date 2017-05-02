using System;
using System.Threading.Tasks;

namespace Operations
{
    public sealed class OperationBuilder<TResult> : IOperationBuilder<TResult>
    {
        private readonly TResult value;
        private IOperationService<TResult, TResult> accumulate;

        public OperationBuilder(TResult value)
        {
            this.value = Throw.IfNull(value, nameof(value));
            this.accumulate = OperationService.Id<TResult>();
        }

        public void Inject(IOperationService<TResult, TResult> service)
            => accumulate = accumulate.AddOuter(service);

        public IOperation<TResult> Build()
            => accumulate.Return(value);
    }
}
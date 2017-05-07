using System;
using System.Threading.Tasks;

namespace Operations
{
    public abstract class Builder<TResult, TBuilder> : IBuilder<TResult, TBuilder>
        where TBuilder : Builder<TResult, TBuilder>
    {
        private readonly IOperation<TResult> source;

        public IOperation<TResult> Build()
            => source;

        public abstract TBuilder Return(IOperation<TResult> source);

        protected Builder(IOperation<TResult> source)
            => this.source = Throw.IfNull(source, nameof(source));

        protected Builder(TResult source)
            : this(Operation.Return(source)) { }

        protected TBuilder With(IOperationService<TResult> service)
            => this.With(service);

        protected TBuilder With(Func<TResult, Task<IContext<TResult>>> closure)
            => this.With(closure);

        protected TBuilder With(Func<TResult, IContext<TResult>> closure)
            => this.With(closure);

        protected TBuilder With(Func<TResult, TResult> selector)
            => this.With(selector);

        protected TBuilder When(Func<TResult, bool> predicate)
            => this.When(predicate);
    }
}
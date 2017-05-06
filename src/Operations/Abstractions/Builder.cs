using System;
using System.Threading.Tasks;

namespace Operations
{
    public abstract class Builder<TResult, TReturn> : IBuilder<TResult, TReturn>
    {
        private readonly IOperation<TResult> source;

        public IOperation<TResult> Build()
            => source;

        public abstract TReturn Return(IOperation<TResult> source);

        protected Builder(IOperation<TResult> source)
            => this.source = Throw.IfNull(source, nameof(source));

        protected Builder(TResult source)
            : this(Operation.Return(source)) { }

        protected TReturn With(IOperationService<TResult> service)
            => this.With(service);

        protected TReturn With(Func<TResult, Task<IContext<TResult>>> closure)
            => this.With(closure);

        protected TReturn With(Func<TResult, IContext<TResult>> closure)
            => this.With(closure);

        protected TReturn With(Func<TResult, TResult> selector)
            => this.With(selector);

        protected TReturn When(Func<TResult, bool> predicate)
            => this.When(predicate);
    }
}
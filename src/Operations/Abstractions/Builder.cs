using System;
using System.Threading.Tasks;

namespace Operations
{
    public abstract class Builder<T, TSelf> : IBuilder<T, TSelf>
        where TSelf : Builder<T, TSelf>
    {
        private IOperation<T> source;

        protected Builder(T source)
            : this(Operation.Return(source)) { }

        protected Builder(IOperation<T> source)
            => this.source = Throw.IfNull(source, nameof(source));

        public IOperation<T> Build()
            => source;

        public virtual TSelf Return(IOperation<T> source)
        {
            TSelf builder = this is TSelf ?
                MemberwiseClone() as TSelf :
                throw new InvalidOperationException(
                    $"The type {typeof(TSelf)} has to be {this}");
            builder.source = source;
            return builder;
        }

        protected TSelf With(IOperationService<T> service)
            => Builder.With(this, service);

        protected TSelf With(Func<T, IOperation<T>> closure)
            => Builder.With(this, closure);

        protected TSelf With(Func<T, Task<IContext<T>>> closure)
            => Builder.With(this, closure);

        protected TSelf With(Func<T, IContext<T>> closure)
            => Builder.With(this, closure);
    }
}
using System;
using System.Threading.Tasks;

namespace Operations
{
    public class Builder<T> : Builder<T, Builder<T>>
    {
    }

    public abstract class Builder<T, TBuilder> : IBuilder<T, TBuilder>
        where TBuilder : Builder<T, TBuilder>, new()
    {
        private IOperationService<T> service;

        protected Builder()
            : this(OperationService.Id<T>()) { }

        protected Builder(IOperationService<T> source)
            => service = source;

        protected Builder(Func<T, IContext<T>> closure)
            => service = OperationService.Return<T>(closure);

        protected Builder(Func<T, Task<IContext<T>>> closure)
            => service = OperationService.Return<T>(closure);

        protected Builder(Func<T, IOperation<T>> closure)
            => service = OperationService.Return<T>(closure);

        public IOperation<T> BuildWith(T injection)
            => service.Inject(injection);

        public TBuilder Return(IOperationService<T> source)
        {
            var builder = new TBuilder();
            builder.service = source;
            return builder;
        }

        protected TBuilder With(IOperationService<T> service)
            => this.With(service);

        protected TBuilder With(Func<T, IOperation<T>> closure)
            => this.With(closure);

        protected TBuilder With(Func<T, Task<IContext<T>>> closure)
            => this.With(closure);

        protected TBuilder With(Func<T, IContext<T>> closure)
            => this.With(closure);
    }
}
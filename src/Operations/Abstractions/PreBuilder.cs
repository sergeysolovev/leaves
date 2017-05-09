using System;
using System.Threading.Tasks;

namespace Operations
{
    public abstract class PreBuilder<T, TSelf> : IPreBuilder<T, TSelf>
        where TSelf : PreBuilder<T, TSelf>
    {
        private IOperationService<T> source;

        protected PreBuilder() { }

        protected PreBuilder(IOperationService<T> source)
            => source = Throw.IfNull(source, nameof(source));

        public IOperationBuilder<T> Inject(T injection)
            => new OperationBuilder<T>(
                source == null ?
                    Operation.Return(injection) :
                    source.Inject(injection));

        public virtual TSelf Return(IOperationService<T> source)
        {
            TSelf builder = this is TSelf ?
                MemberwiseClone() as TSelf :
                throw new InvalidOperationException(
                    $"The type {typeof(TSelf)} has to be {this}");
            builder.source = source;
            return builder;
        }

        protected TSelf With(IOperationService<T> service)
            => PreBuilder.With(this, service);

        protected TSelf With(Func<T, IOperation<T>> closure)
            => PreBuilder.With(this, closure);

        protected TSelf With(Func<T, Task<IContext<T>>> closure)
            => PreBuilder.With(this, closure);

        protected TSelf With(Func<T, IContext<T>> closure)
            => PreBuilder.With(this, closure);
    }
}
namespace Operations
{
    public interface IOperationBuilder<T>
    {
        IOperation<T> Build();
    }

    internal class OperationBuilder<T> : IOperationBuilder<T>
    {
        private readonly IOperation<T> source;

        internal OperationBuilder(IOperation<T> source)
            => this.source = Throw.IfNull(source, nameof(source));

        public IOperation<T> Build()
            => source;
    }

    public interface IOperationPreBuilder<T>
    {
        IOperationBuilder<T> Inject(T injection);
    }
}
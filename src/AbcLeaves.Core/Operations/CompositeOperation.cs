using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbcLeaves.Core
{
    public class CompositeOperation<TResult, TContext> : Operation<TResult, TContext>, ICompositeOperation<TResult, TContext>
        where TResult : IOperationResult<TContext>, new()
        where TContext : IOperationContext
    {
        private readonly List<IOperation<TResult, TContext>> operations;

        protected CompositeOperation()
        {
            operations = new List<IOperation<TResult, TContext>>();
        }

        public static CompositeOperation<TResult, TContext> Create()
        {
            return new CompositeOperation<TResult, TContext>();
        }

        public IReadOnlyCollection<IOperation<TResult, TContext>> Operations
        {
            get { return operations.AsReadOnly(); }
        }

        public void Add(IOperation<TResult, TContext> operation)
        {
            if (operation == this)
            {
                throw new InvalidOperationException();
            }
            operations.Add(operation);
        }

        public void Clear()
        {
            operations.Clear();
        }

        public override async Task<TResult> ExecuteAsync(TContext context)
        {
            return await operations.ExecuteSequence(context).Return();
        }
    }
}
